using OneOf;
using Shiki.ModuleManagement.Module;
using Shiki.ModuleManagement.Source;

namespace Shiki.ModuleManagement;

/// <summary>
/// Builder for ModuleStorage
/// </summary>
/// <typeparam name="TModule">The base module type</typeparam>
/// <typeparam name="TArgs">The args that would be provided to each module if initializable</typeparam>
public class ModuleStorageBuilder<TModule, TArgs>
    where TArgs : EventArgs
    where TModule : IModule
{
    private readonly record struct PendingSource(IModuleSource Source, TArgs Args);
    private readonly record struct PendingModule(TArgs? InitArgs, OneOf<TModule, Type> Module);
    
    /// <summary>
    /// List of sources to build the ModuleStorage with
    /// </summary>
    private readonly List<PendingSource> _sources = [];

    /// <summary>
    /// List of pre-added modules
    /// </summary>
    private readonly List<PendingModule> _preAdded = [];
    
    /// <summary>
    /// Instantiates, initializes (if args are provided), and adds a module of the given type
    /// </summary>
    /// <param name="args">The args used to initialize the module, or null if not to be initialized</param>
    /// <typeparam name="TNewModule">The module type</typeparam>
    /// <returns>Self</returns>
    public ModuleStorageBuilder<TModule, TArgs> WithModuleOfType<TNewModule>(TArgs? args) where TNewModule : TModule, new()
    {
        _preAdded.Add(new PendingModule(args, typeof(TNewModule)));

        return this;
    }
    
    /// <summary>
    /// Instantiates, initializes (if args are provided), and adds a module of the given type
    /// </summary>
    /// <param name="type">The module type</param>
    /// <param name="args">The args used to initialize the module, or null if not to be initialized</param>
    /// <returns>Self</returns>
    public ModuleStorageBuilder<TModule, TArgs> WithModuleOfType(Type type, TArgs? args)
    {
        _preAdded.Add(new PendingModule(args, type));

        return this;
    }
    
    /// <summary>
    /// Initializes (if args are provided), and adds the given module
    /// </summary>
    /// <param name="module">The module</param>
    /// <param name="args">The args used to initialize the module, or null if not to be initialized</param>
    /// <returns>Self</returns>
    public ModuleStorageBuilder<TModule, TArgs> WithModule(TModule module, TArgs? args)
    {
        _preAdded.Add(new PendingModule(args, module));

        return this;
    }
    
    /// <summary>
    /// Loads modules from the given source, builder style.
    /// </summary>
    /// <param name="moduleSource">The module source</param>
    /// <param name="args">The args to pass during module initialization</param>
    /// <returns>Self</returns>
    public ModuleStorageBuilder<TModule, TArgs> WithSource(IModuleSource moduleSource, TArgs args)
    {
        _sources.Add(new PendingSource(moduleSource, args));

        return this;
    }

    /// <summary>
    /// Builds the ModuleStorage
    /// </summary>
    /// <returns>The built ModuleStorage</returns>
    public async Task<ModuleStorage<TModule, TArgs>> BuildAsync()
    {
        var storage = new ModuleStorage<TModule, TArgs>();

        foreach ((IModuleSource src, TArgs args) in _sources)
        {
            await storage.LoadFromAsync(src, args);
        }

        foreach ((TArgs? initArgs, var module) in _preAdded)
        {
            switch (module.Value)
            {
                case TModule mod when initArgs is not null:
                    await storage.TryAddInitializeNoCleanupAsync(mod, initArgs);
                    break;
                case TModule mod when initArgs is null:
                    storage.TryAddNoCleanup(mod);
                    break;
                case Type type when initArgs is not null:
                    await storage.TryAddOfTypeInitializeAsync(type, initArgs);
                    break;
                case Type type when initArgs is null:
                    await storage.TryAddOfTypeAsync(type);
                    break;
            }
        }

        return storage;
    }
}