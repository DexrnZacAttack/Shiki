using System.Collections.Immutable;
using Shiki.Common.Identity;
using Shiki.Common.Result;
using Shiki.Settings.Category;
using Shiki.Settings.Option;
using Shiki.Settings.Option.Storage;
using Shiki.Settings.Util;

namespace Shiki.Settings;

/// <summary>
/// Holds and manages settings
/// </summary>
public class SettingsManager : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Option groups, associated with their storage's ID
    /// </summary>
    private readonly ImmutableDictionary<Identifier, OptionGroupMetadata> _groups;
    /// <summary>
    /// Settings categories
    /// </summary>
    private readonly ImmutableDictionary<Identifier, SettingsCategory> _categories;
    /// <summary>
    /// Option storages
    /// </summary>
    private readonly ImmutableDictionary<Identifier, IOptionStorage> _storages;
    /// <summary>
    /// Whether the SettingsManager is disposed
    /// </summary>
    private bool _disposed;
    
    /// <summary>
    /// All categories
    /// </summary>
    public IReadOnlyDictionary<Identifier, SettingsCategory> Categories => _categories;
    /// <summary>
    /// All options
    /// </summary>
    public IReadOnlyDictionary<Identifier, IOption> Options { get; }
    
    /// <summary>
    /// The root category nodes
    ///
    /// Used to interact with categories and options in a tree-style manner
    /// </summary>
    public ImmutableArray<SettingsCategoryNode> RootCategories { get; }

    /// <summary>
    /// Whether the SettingsManager is disposed
    /// </summary>
    public bool Disposed => Volatile.Read(ref _disposed);

    /// <summary>
    /// Creates a new SettingsManager
    /// </summary>
    /// <param name="groups">Groups</param>
    /// <param name="categories">Categories</param>
    /// <param name="storages">Storages</param>
    internal SettingsManager(IReadOnlyDictionary<Identifier, OptionGroupMetadata> groups, IReadOnlyDictionary<Identifier, SettingsCategory> categories, IReadOnlyDictionary<Identifier, IOptionStorage> storages)
    {
        _groups = groups.ToImmutableDictionary();
        _categories = categories.ToImmutableDictionary();
        _storages = storages.ToImmutableDictionary();

        Options = _groups.Values.SelectMany(m => m.Group.GetOptions()).ToDictionary(opt => opt.Identifier);
        RootCategories = SettingsCategoryNode.Builder.Build(Options.Values, Categories.Values);
    }

    /// <summary>
    /// Tries to get an option by its ID
    /// </summary>
    /// <param name="identifier">The option ID</param>
    /// <typeparam name="T">The option type</typeparam>
    /// <returns>The option, if found</returns>
    public IOption<T>? TryGetOption<T>(Identifier identifier)
    {
        if (Disposed)
            return null;
        
        return Options.TryGetValue(identifier, out IOption? value) && value is IOption<T> t ? t : null;
    }

    /// <summary>
    /// Tries to get a group by its ID
    /// </summary>
    /// <param name="identifier">The group ID</param>
    /// <typeparam name="T">The group type</typeparam>
    /// <returns>The group, if found</returns>
    public T? TryGetGroup<T>(Identifier identifier)
        where T : class, IOptionGroup
    {
        if (Disposed)
            return null;
        
        return _groups.TryGetValue(identifier, out OptionGroupMetadata value) && value.Group is T t ? t : null;
    }
    
    /// <summary>
    /// Tries to get an identifiable group by its type
    /// </summary>
    /// <typeparam name="T">The group type</typeparam>
    /// <returns>The group, if found</returns>
    public T? TryGetGroup<T>()
        where T : class, IIdentifiableOptionGroup
    {
        if (Disposed)
            return null;
        
        return _groups.TryGetValue(T.Identifier, out OptionGroupMetadata value) && value.Group is T t ? t : null;
    }
    
    /// <summary>
    /// Loads all options in the SettingsManager
    ///
    /// This also initializes all storages to start tracking their respective option groups
    /// </summary>
    /// <returns>The result of each option load</returns>
    public async ValueTask<IReadOnlyCollection<BooleanResult<Exception>>> LoadAsync()
    {
        if (Disposed)
            return [new ObjectDisposedException(nameof(SettingsManager))];
        
        var tasks = _groups.Select(async s =>
        {
            if (!_storages.TryGetValue(s.Value.Storage, out IOptionStorage? storage))
            {
                return [new KeyNotFoundException("Storage not found")];
            }
            
            return await storage.LoadAsync(s.Value.Group);
        });

        return (await Task.WhenAll(tasks)).SelectMany(x => x).ToList();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, true))
            return;
        
        foreach (IOptionStorage s in _storages.Values)
            s.Dispose();
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref _disposed, true))
            return;
        
        foreach (IOptionStorage s in _storages.Values)
            await s.DisposeAsync();
    }
}