using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using Shiki.Common.Identity;
using Shiki.Common.Result;
using Shiki.ModuleManagement.Loader;
using Shiki.ModuleManagement.Module;
using Shiki.ModuleManagement.Source;

namespace Shiki.ModuleManagement;

/// <summary>
/// Stores module instances and sources
/// </summary>
/// <typeparam name="TModule">The module type that is stored</typeparam>
/// <typeparam name="TArgs">The args that would be provided to each module if initializable</typeparam>
public class ModuleStorage<TModule, TArgs> : IAsyncDisposable, IDisposable
    where TArgs : EventArgs
    where TModule : IModule
{
    /// <summary>
    /// Whether the storage has been disposed
    /// </summary>
    private bool _disposed = false;
    
    /// <summary>
    /// Stored modules
    /// </summary>
    private readonly ConcurrentDictionary<Identifier, TModule> _modules = [];
    
    /// <summary>
    /// All modules loaded by the ModuleStorage
    /// </summary>
    public IReadOnlyDictionary<Identifier, TModule> Modules => _modules;
    
    /// <summary>
    /// Stored module sources, need to keep it around just in case the source must hold and keep alive extra data
    /// </summary>
    private readonly ConcurrentBag<IModuleSource> _moduleSources = []; // for some sources, we may need to keep data around, such as AssemblyLoadContexts.

    /// <summary>
    /// Creates an empty ModuleStorage
    /// </summary>
    public ModuleStorage() {}

    /// <summary>
    /// Loads modules from the given source
    /// </summary>
    /// <param name="moduleSource">The module source</param>
    /// <param name="args">The args to pass during module initialization</param>
    public async Task LoadFromAsync(IModuleSource moduleSource, TArgs args)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        bool anyAdded = false;//so we can dispose entire modulesource if nothing was ever actually loaded for it (because why would we want to keep it around)
        
        var modules = ModuleLoader.Load<TModule>(moduleSource);
        foreach (var kvp in modules)
        {
            if (await TryAddInitializeAsync(kvp.Value, args) == null)
            {
                anyAdded = true;
            }
        }

        if (!anyAdded)
        {
            await CleanupModuleSourceAsync(moduleSource);
        }
        else
        {
            this._moduleSources.Add(moduleSource);
        }
    }
    
    /// <summary>
    /// Instantiates and adds a module of given type
    ///
    /// On failure, the module is correctly disposed (requiring async)
    ///
    /// This method does NOT initialize it
    /// </summary>
    /// <typeparam name="TNewModule">The module type to instantiate and add</typeparam>
    /// <returns>A result containing either the added module or an exception if it wasn't added successfully</returns>
    public async ValueTask<Result<TNewModule, Exception>> TryAddOfTypeAsync<TNewModule>() where TNewModule : TModule, new()
    {
        if (_disposed)
            return new ObjectDisposedException(nameof(ModuleStorage<,>));

        TNewModule? module = default;
        try
        {
            module = ModuleInstantiator.Instantiate<TNewModule>();
            
            if (_modules.TryAdd(module.Id, module))
            {
                return module;
            }

            await CleanupModuleAsync(module);
            return new InvalidOperationException("Module with same key already exists");
        }
        catch (Exception ex)
        {
            await CleanupModuleAsync(module);
            return ex;
        }
    }

    
    /// <summary>
    /// Instantiates and adds a module of given type
    ///
    /// On failure, the module is correctly disposed (requiring async)
    ///
    /// This method does NOT initialize it
    /// </summary>
    /// <param name="type">The module type to instantiate and add</param>
    /// <returns>A result containing either the added module or an exception if it wasn't added successfully</returns>
    public async ValueTask<Result<TModule, Exception>> TryAddOfTypeAsync(Type type)
    {
        if (_disposed)
            return new ObjectDisposedException(nameof(ModuleStorage<,>));

        TModule? module = default;
        try
        {
            module = ModuleInstantiator.Instantiate<TModule>(type);
            
            if (_modules.TryAdd(module.Id, module))
            {
                return module;
            }

            await CleanupModuleAsync(module);
            return new InvalidOperationException("Module with same key already exists");
        }
        catch (Exception ex)
        {
            await CleanupModuleAsync(module);
            return ex;
        }
    }
    
    /// <summary>
    /// Instantiates, initializes, and adds a module of given type
    ///
    /// On failure, the module is correctly disposed
    /// </summary>
    /// <typeparam name="TNewModule">The module type to instantiate and add</typeparam>
    /// <param name="args">The arguments to pass to the module</param>
    /// <returns>A result containing either the added module or an exception if it wasn't added successfully</returns>
    public async ValueTask<Result<TNewModule, Exception>> TryAddOfTypeInitializeAsync<TNewModule>(TArgs args) where TNewModule : TModule, new()
    {
        if (_disposed)
            return new ObjectDisposedException(nameof(ModuleStorage<,>));

        TNewModule? module = default;
        try
        {
            module = ModuleInstantiator.Instantiate<TNewModule>();
            await InitializeModuleAsync(module, args);
            
            if (_modules.TryAdd(module.Id, module))
            {
                return module;
            }

            await CleanupModuleAsync(module);
            return new InvalidOperationException("Module with same key already exists");
        }
        catch (Exception ex)
        {
            await CleanupModuleAsync(module);
            return ex;
        }
    }
    
    /// <summary>
    /// Instantiates, initializes, and adds a module of given type
    /// 
    /// On failure, the module is correctly disposed
    /// </summary>
    /// <param name="type">The module type to instantiate and add</param>
    /// <param name="args">The arguments to pass to the module</param>
    /// <returns>A result containing either the added module or an exception if it wasn't added successfully</returns>
    public async ValueTask<Result<TModule, Exception>> TryAddOfTypeInitializeAsync(Type type, TArgs args)
    {
        if (_disposed)
            return new ObjectDisposedException(nameof(ModuleStorage<,>));

        TModule? module = default;
        try
        {
            module = ModuleInstantiator.Instantiate<TModule>(type);
            await InitializeModuleAsync(module, args);
            
            if (_modules.TryAdd(module.Id, module))
            {
                return module;
            }

            await CleanupModuleAsync(module);
            return new InvalidOperationException("Module with same key already exists");
        }
        catch (Exception ex)
        {
            await CleanupModuleAsync(module);
            return ex;
        }
    }
    
    /// <summary>
    /// Adds the given module
    ///
    /// On failure, the module is correctly disposed (requiring async)
    /// </summary>
    /// <returns>An exception if the module was not added successfully</returns>
    public async ValueTask<Exception?> TryAddAsync(TModule module)
    {
        if (_disposed)
            return new ObjectDisposedException(nameof(ModuleStorage<,>));

        try
        {
            if (_modules.TryAdd(module.Id, module))
            {
                return null;
            }

            await CleanupModuleAsync(module);
            return new InvalidOperationException("Module with same key already exists");
        }
        catch (Exception ex)
        {
            await CleanupModuleAsync(module);
            return ex;
        }
    }
    
    /// <summary>
    /// Initializes and adds the given module instance
    /// 
    /// On failure, the module is correctly disposed
    /// </summary>
    /// <param name="module">The module to add</param>
    /// <param name="args">The arguments to pass to the module</param>
    /// <returns>An exception if the module was not added successfully</returns>
    public async ValueTask<Exception?> TryAddInitializeAsync(TModule module, TArgs args)
    {
        if (_disposed)
            return new ObjectDisposedException(nameof(ModuleStorage<,>));

        try
        {
            await InitializeModuleAsync(module, args);
            
            if (_modules.TryAdd(module.Id, module))
            {
                return null;
            }

            await CleanupModuleAsync(module);
            return new InvalidOperationException("Module with same key already exists");
        }
        catch (Exception ex)
        {
            await CleanupModuleAsync(module);
            return ex;
        }
    }
    
    /// <summary>
    /// Adds the given module
    ///
    /// On failure, the module is NOT disposed
    /// This is useful if the module needs to be used by the caller afterward
    /// </summary>
    /// <returns>An exception if the module was not added successfully</returns>
    public Exception? TryAddNoCleanup(TModule module)
    {
        if (_disposed)
            return new ObjectDisposedException(nameof(ModuleStorage<,>));

        try
        {
            return _modules.TryAdd(module.Id, module) ? null : new InvalidOperationException("Module with same key already exists");
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    /// <summary>
    /// Initializes and adds the given module instance
    /// 
    /// On failure, the module is NOT disposed
    /// This is useful if the module needs to be used by the caller afterward
    /// </summary>
    /// <param name="module">The module to add</param>
    /// <param name="args">The arguments to pass to the module</param>
    /// <returns>An exception if the module was not added successfully</returns>
    public async ValueTask<Exception?> TryAddInitializeNoCleanupAsync(TModule module, TArgs args)
    {
        if (_disposed)
            return new ObjectDisposedException(nameof(ModuleStorage<,>));

        try
        {
            await InitializeModuleAsync(module, args);
            
            return _modules.TryAdd(module.Id, module) ? null : new InvalidOperationException("Module with same key already exists");
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    /// <summary>
    /// Tries to get a module by id
    /// </summary>
    /// <param name="identifier">The module id</param>
    /// <param name="module">The module</param>
    /// <returns>Whether the module was found</returns>
    public bool TryGetModule(Identifier identifier, [NotNullWhen(true)] out TModule? module)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        
        return _modules.TryGetValue(identifier, out module);
    }

    /// <summary>
    /// Tries to get a module by id with given type
    /// </summary>
    /// <param name="identifier">The module id</param>
    /// <param name="module">The module</param>
    /// <typeparam name="T">The expected type of the module</typeparam>
    /// <returns>Whether the module was found</returns>
    public bool TryGetModule<T>(Identifier identifier, [NotNullWhen(true)] out T? module)
        where T : TModule
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        
        if (_modules.TryGetValue(identifier, out TModule? m) && m is T mod)
        {
            module = mod;
            return true;
        }
        
        module = default;
        return false;
    }
    
    /// <summary>
    /// Tries to remove the module, providing the removed module to the caller
    ///
    /// Note that to remove and dispose, you should call TryRemoveDisposeAsync
    /// </summary>
    /// <param name="identifier">The module id</param>
    /// <param name="module">The module</param>
    /// <returns>Whether the module was found and removed</returns>
    public bool TryRemove(Identifier identifier, [NotNullWhen(true)] out TModule? module)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        
        return _modules.TryRemove(identifier, out module);
    }

    /// <summary>
    /// Tries to remove a module by id, then disposes it if disposable
    ///
    /// This is the correct way to 'unload' a module
    /// </summary>
    /// <param name="identifier">The module id</param>
    /// <returns>Whether the module was found and removed</returns>
    public async Task<bool> TryRemoveDisposeAsync(Identifier identifier)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        
        if (TryRemove(identifier, out TModule? module))
        {
            await CleanupModuleAsync(module);
            
            return true;
        }
        
        return false;
    }
    
    /// <summary>
    /// Removes all modules, disposing any that implement IDisposable/IAsyncDisposable
    /// </summary>
    public async Task RemoveAllAsync()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        
        var tasks = _modules.Keys.Select(TryRemoveDisposeAsync);
        await Task.WhenAll(tasks);
    }
    
    /// <summary>
    /// Removes all modules and unloads all sources, disposing any that implement IDisposable/IAsyncDisposable
    /// </summary>
    public async Task UnloadAllAsync()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        
        await RemoveAllAsync();
        await RemoveDisposeAllModuleSourcesAsync();
    }

    /// <summary>
    /// Does what it says on the tin
    ///
    /// Removes and disposes all module sources.
    /// For module sources that store required contexts (such as assembly contexts), this will destroy those contexts, and this may cause issues.
    /// </summary>
    private async Task RemoveDisposeAllModuleSourcesAsync()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        var tasks = _moduleSources.Select(async src =>
        {
            try
            {
                await CleanupModuleSourceAsync(src);
            }
            catch (Exception)
            {
                // ignored
            }
        });
        
        await Task.WhenAll(tasks);
        _moduleSources.Clear();
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref _disposed, true))
        {
            return;
        }
        
        foreach (var kvp in _modules)
        {
            if (TryRemove(kvp.Key, out TModule? module))
            {
                await CleanupModuleAsync(module);
            }
        }
        
        foreach (IModuleSource src in _moduleSources)
        {
            await CleanupModuleSourceAsync(src);
        }
        _moduleSources.Clear();
        
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, true))
        {
            return;
        }
        
        foreach (var kvp in _modules)
        {
            if (TryRemove(kvp.Key, out TModule? module) && module is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
        
        foreach (IModuleSource src in _moduleSources)
        {
            if (src is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
        _moduleSources.Clear();
        
        GC.SuppressFinalize(this);
    }
    
    //I could literally turn this into like some stupid global helper but idc
    
    /// <summary>
    /// Disposes a module
    /// </summary>
    /// <param name="module">The module to dispose</param>
    private static async ValueTask CleanupModuleAsync(TModule? module)
    {
        switch (module)
        {
            case IAsyncDisposable asyncDisposable:
                await asyncDisposable.DisposeAsync();
                break;
            case IDisposable disposable:
                disposable.Dispose();
                break;
        }
    }
    
    /// <summary>
    /// Disposes a given module source
    /// </summary>
    /// <param name="moduleSource">The module source to dispose</param>
    private static async ValueTask CleanupModuleSourceAsync(IModuleSource? moduleSource)
    {
        switch (moduleSource)
        {
            case IAsyncDisposable asyncDisposable:
                await asyncDisposable.DisposeAsync();
                break;
            case IDisposable disposable:
                disposable.Dispose();
                break;
        }
    }
    
    /// <summary>
    /// Initializes a module with the given args
    /// </summary>
    /// <param name="module">The module to initialize</param>
    /// <param name="args">The args to pass into the module initializer</param>
    private static async ValueTask InitializeModuleAsync(TModule module, TArgs args)
    {
        switch (module)
        {
            case IAsyncInitializableModule<TArgs> asyncInitializableModule:
                await asyncInitializableModule.OnInitializeAsync(args);
                break;
            case IInitializableModule<TArgs> initializableModule:
                initializableModule.OnInitialize(args);
                break;
        }
    }
}