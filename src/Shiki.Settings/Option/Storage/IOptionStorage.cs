using R3;
using Shiki.Common.Result;
using Shiki.FileSystem.Storage;
using Shiki.Settings.Util;

namespace Shiki.Settings.Option.Storage;

/// <summary>
/// Loads,saves options
///
/// Saving is implemented inside the storage by listening to the option value
/// </summary>
public interface IOptionStorage : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Loads the values for all the options in the group
    /// </summary>
    /// <param name="group">The group</param>
    /// <returns>For each option, the result</returns>
    ValueTask<IReadOnlyCollection<BooleanResult<Exception>>> LoadAsync(IOptionGroup group);
    
    /// <param name="group">The group</param>
    /// <returns>Whether the given group can be successfully stored in this storage instance</returns>
    bool CanAccept(IOptionGroup group);
}

/// <summary>
/// Simple option storage
/// </summary>
/// <param name="reader">The option reader</param>
/// <param name="writer">The option writer</param>
/// <typeparam name="T">The option type</typeparam>
public class OptionStorage<T>(
    IStorageReader<Result<T, Exception>> reader,
    IStorageWriter<T, BooleanResult<Exception>> writer)
    : IOptionStorage where T : class, IOptionGroup
{
    /// <summary>
    /// The option store
    /// </summary>
    private StoredObject<T>? _store;
    
    /// <summary>
    /// Creates a new OptionStorage
    /// </summary>
    /// <param name="storage">The storage</param>
    /// <typeparam name="TStorage">The storage type, must match both of the OptionStorage's reader,writer types</typeparam>
    /// <returns>The OptionStorage</returns>
    public static OptionStorage<T> Create<TStorage>(TStorage storage)
        where TStorage : IStorageReader<Result<T, Exception>>, IStorageWriter<T, BooleanResult<Exception>> =>
        new(storage, storage);
    
    /// <inheritdoc/>
    public async ValueTask<IReadOnlyCollection<BooleanResult<Exception>>> LoadAsync(IOptionGroup group)
    {
        if (group is not T settings)
            return [new ArgumentException($"Got wrong option group tpye")];
        
        var res = await reader.LoadAsync();
        switch (res.HasValue)
        {
            case true:
            {
                var o = res.Value.GetOptions().ToDictionary(opt => opt.Identifier);
                foreach (IOption option in settings.GetOptions())
                {
                    if (o.TryGetValue(option.Identifier, out IOption? oo))
                    {
                        option.SetValue(oo.GetValue());
                    }
                }

                break;
            }
            case false when res.Error is FileNotFoundException:
                await writer.SaveAsync(settings);
                break;
            case false:
                return [res.Error];
        }

        if (_store != null)
        {
            await _store.DisposeAsync();
        }

        _store = new StoredObject<T>(settings, [
                                         s => s.GetOptions().Select(opt => opt.ValueChanged.Select(_ => s)).Merge()
                                     ],
                                     reader,
                                     writer);
        
        return [BooleanResult<Exception>.Successful];
    }

    /// <inheritdoc/>
    public bool CanAccept(IOptionGroup group) => group is T;

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_store is IDisposable storeDisposable)
            storeDisposable.Dispose();
        else if (_store != null)
            _ = _store.DisposeAsync().AsTask();
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        if (_store != null) await _store.DisposeAsync();
    }
}

/// <summary>
/// Simple option storage
/// </summary>
/// <param name="storage">The option storage</param>
/// <typeparam name="T">The option type</typeparam>
/// <typeparam name="TStorage">The option storage type</typeparam>
public class OptionStorage<T, TStorage>(TStorage storage) : OptionStorage<T>(storage, storage)
where TStorage : IStorageReader<Result<T, Exception>>, IStorageWriter<T, BooleanResult<Exception>>
where T : class, IOptionGroup;