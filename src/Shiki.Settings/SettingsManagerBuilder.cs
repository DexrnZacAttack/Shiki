using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using Shiki.Common.Extensions;
using Shiki.Common.Identity;
using Shiki.Common.Result;
using Shiki.Settings.Category;
using Shiki.Settings.Option;
using Shiki.Settings.Option.Storage;
using Shiki.Settings.Util;

namespace Shiki.Settings;

/// <summary>
/// Single-use builder for SettingsManager
/// </summary>
public class SettingsManagerBuilder
{
    /// <summary>
    /// Groups
    /// </summary>
    private readonly ConcurrentDictionary<Identifier, OptionGroupMetadata> _groups = [];
    /// <summary>
    /// Categories
    /// </summary>
    private readonly ConcurrentDictionary<Identifier, SettingsCategory> _categories = [];
    /// <summary>
    /// Storages
    /// </summary>
    private readonly ConcurrentDictionary<Identifier, IOptionStorage> _storages = [];
    /// <summary>
    /// Whether the builder has been built already
    /// </summary>
    private bool _consumed;
    
    /// <summary>
    /// Whether the builder has been built already
    /// </summary>
    public bool Consumed => Volatile.Read(ref _consumed);

    /// <summary>
    /// Adds a category, throwing if it already exists
    /// </summary>
    /// <param name="category">The category</param>
    /// <returns>The builder</returns>
    public SettingsManagerBuilder AddCategory(SettingsCategory category)
    {
        ThrowIfConsumed();
        
        TryAddCategory(category, out var result);
        result.ThrowIfError();

        return this;
    }

    /// <summary>
    /// Tries to add a category
    /// </summary>
    /// <param name="category">The category</param>
    /// <param name="result">The result</param>
    /// <returns>The builder</returns>
    public SettingsManagerBuilder TryAddCategory(SettingsCategory category, out BooleanResult<Exception> result)
    {
        result = BooleanResult<Exception>.Successful;

        if (SetIfConsumed(ref result))
            return this;
        
        result = BooleanResult<Exception>.Successful;
        if (!_categories.TryAdd(category.Id, category))
            result = new InvalidOperationException("Category already exists").WithCurrentStackTrace();

        return this;
    }
    
    /// <summary>
    /// Tries to add a category, ignoring the result if it fails
    ///
    /// This is best used for when a category already existing is expected/okay with the caller
    ///
    /// Additionally, this method DOES throw if this builder has already been built (as there is no other way to communicate to the caller that they shouldn't be using this object)
    /// </summary>
    /// <param name="category">The category</param>
    /// <returns>The builder</returns>
    public SettingsManagerBuilder TryAddCategory(SettingsCategory category)
    {
        ThrowIfConsumed();
        
        _categories.TryAdd(category.Id, category);

        return this;
    }
    
    /// <summary>
    /// Adds a group, throwing if it already exists or if the storage does not exist
    /// </summary>
    /// <param name="id">The group's id</param>
    /// <param name="group">The group</param>
    /// <param name="storage">The id of the storage for the group</param>
    /// <returns>The builder</returns>
    public SettingsManagerBuilder AddGroup(Identifier id, IOptionGroup group, Identifier storage)
    {
        ThrowIfConsumed();
        
        TryAddGroup(id, group, storage, out var result);
        result.ThrowIfError();

        return this;
    }

    /// <summary>
    /// Tries to add a group
    /// </summary>
    /// <param name="id">The group's id</param>
    /// <param name="group">The group</param>
    /// <param name="storage">The id of the storage for the group</param>
    /// <param name="result">The result</param>
    /// <returns>The builder</returns>
    public SettingsManagerBuilder TryAddGroup(Identifier id, IOptionGroup group, Identifier storage, out BooleanResult<Exception> result)
    {
        result = BooleanResult<Exception>.Successful;
        if (SetIfConsumed(ref result))
            return this;
        
        if (!_storages.TryGetValue(storage, out IOptionStorage? s))
        {
            result = new KeyNotFoundException("Storage not found").WithCurrentStackTrace();
            return this;
        }

        if (!s.CanAccept(group))
        {
            result = new InvalidOperationException("Storage cannot store this group").WithCurrentStackTrace();
            return this;
        }

        if (!_groups.TryAdd(id, new OptionGroupMetadata(group, storage)))
        {
            result = new InvalidOperationException("Group already exists").WithCurrentStackTrace();
        }
        
        return this;
    }

    /// <summary>
    /// Adds a category, throwing if it already exists
    /// </summary>
    /// <param name="id">The storage's ID</param>
    /// <param name="storage">The storage</param>
    /// <returns>The builder</returns>
    public SettingsManagerBuilder AddStorage(Identifier id, IOptionStorage storage)
    {
        ThrowIfConsumed();
        
        TryAddStorage(id, storage, out var result);
        result.ThrowIfError();

        return this;
    }

    /// <summary>
    /// Tries to add a category
    /// </summary>
    /// <param name="id">The storage's ID</param>
    /// <param name="storage">The storage</param>
    /// <param name="result">The result</param>
    /// <returns>The builder</returns>
    public SettingsManagerBuilder TryAddStorage(Identifier id, IOptionStorage storage, out BooleanResult<Exception> result)
    {
        result = BooleanResult<Exception>.Successful;
        if (SetIfConsumed(ref result))
            return this;
        
        if (!_storages.TryAdd(id, storage))
            result = new InvalidOperationException("Storage already exists").WithCurrentStackTrace();
        
        return this;
    }

    /// <summary>
    /// Tries to add a category, ignoring the result if it fails
    /// 
    /// This is best used for when a storage already existing is expected/okay with the caller
    /// However, this does NOT mean the storage is of the expected, correct type
    ///
    /// Additionally this method DOES throw if this builder has already been built (as there is no other way to communicate to the caller that they shouldn't be using this object)
    /// </summary>
    /// <param name="id">The storage's ID</param>
    /// <param name="storage">The storage</param>
    /// <returns>The builder</returns>
    public SettingsManagerBuilder TryAddStorage(Identifier id, IOptionStorage storage)
    {
        ThrowIfConsumed();
        
        _storages.TryAdd(id, storage);
        
        return this;
    }

    /// <summary>
    /// Builds the SettingsManager, but does not load it.
    /// </summary>
    /// <returns>The newly built SettingsManager</returns>
    public SettingsManager Build()
    {
        if (Interlocked.Exchange(ref _consumed, true))
            ThrowIfConsumed();
        
        return new SettingsManager(_groups, _categories, _storages);
    }
    
    /// <summary>
    /// Tries to build the SettingsManager, but does not load it.
    ///
    /// Used to avoid exceptions when already disposed
    /// </summary>
    /// <returns>The newly built SettingsManager, or an ObjectDisposedException</returns>
    public Result<SettingsManager, Exception> TryBuild()
    {
        if (Interlocked.Exchange(ref _consumed, true))
            return GetConsumedException();
        
        return new SettingsManager(_groups, _categories, _storages);
    }
    
    /// <summary>
    /// Builds and loads the SettingsManager
    /// </summary>
    /// <returns>The newly built SettingsManager, alongside a list of results for each stored option</returns>
    public async ValueTask<(SettingsManager settingsManager, IReadOnlyCollection<BooleanResult<Exception>> Results)> BuildAndLoadAsync()//todo we can create another result type later that holds the value with errors
    {
        if (Interlocked.Exchange(ref _consumed, true))
            ThrowIfConsumed();
        
        SettingsManager b = new(_groups, _categories, _storages);
        var results = await b.LoadAsync();
        
        return (b, results);
    }
    
    /// <summary>
    /// Builds and loads the SettingsManager
    /// </summary>
    /// <returns>The newly built SettingsManager, alongside a list of results for each stored option</returns>
    public async ValueTask<Result<(SettingsManager settingsManager, IReadOnlyCollection<BooleanResult<Exception>> Results), Exception>> TryBuildAndLoadAsync()//todo we can create another result type later that holds the value with errors
    {
        if (Interlocked.Exchange(ref _consumed, true))
            return GetConsumedException();

        SettingsManager b = new(_groups, _categories, _storages);
        var results = await b.LoadAsync();
        
        return (b, results);
    }

    /// <summary>
    /// Throws an exception if the builder is consumed
    /// </summary>
    /// <exception cref="InvalidOperationException">When the builder is consumed</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ThrowIfConsumed()
    {
        if (Consumed)
            throw GetConsumedException();
    }
    
    /// <summary>
    /// Sets the result with the given exception if the builder is consumed
    /// </summary>
    /// <param name="result">The result</param>
    /// <returns>Whether the builder is consumed</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool SetIfConsumed(ref BooleanResult<Exception> result)
    {
        if (!Consumed) return false;
        
        result = GetConsumedException();
        return true;

    }
    
    /// <returns>The exception</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static InvalidOperationException GetConsumedException()
    {
        return new InvalidOperationException("This builder has been consumed").WithCurrentStackTrace();
    }
}