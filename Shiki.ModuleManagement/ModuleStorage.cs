using Shiki.ModuleManagement.Loader;
using Shiki.ModuleManagement.Source;

namespace Shiki.ModuleManagement;

/// <summary>
/// Stores module instances and sources
/// </summary>
/// <typeparam name="TModule">The module type that is stored</typeparam>
/// <typeparam name="TArgs">The type of args that modules are expected to receive during initialization</typeparam>
public class ModuleStorage<TModule, TArgs>
    where TArgs : EventArgs
    where TModule : IModule<TArgs>
{
    /// <summary>
    /// Stored modules
    /// </summary>
    private readonly List<TModule> _modules = [];
    
    /// <summary>
    /// All modules loaded by the ModuleStorage
    /// </summary>
    public IReadOnlyList<TModule> Modules => _modules;
    
    /// <summary>
    /// Stored module sources, need to keep it around just in case the source must hold and keep alive extra data
    /// </summary>
    private readonly List<IModuleSource> _moduleSources = []; // for some sources, we may need to keep data around, such as AssemblyLoadContexts.

    /// <summary>
    /// Creates an empty ModuleStorage
    /// </summary>
    public ModuleStorage() {}

    /// <summary>
    /// Creates a ModuleStorage and loads all modules from the provided module source
    /// </summary>
    /// <param name="moduleSource">The module source</param>
    /// <param name="args">The args to pass during module initialization</param>
    public ModuleStorage(IModuleSource moduleSource, TArgs args)
    {
        this.LoadFrom(moduleSource, args);
    }

    /// <summary>
    /// Loads modules from the given source, builder style.
    /// </summary>
    /// <param name="moduleSource">The module source</param>
    /// <param name="args">The args to pass during module initialization</param>
    /// <returns>Self</returns>
    public ModuleStorage<TModule, TArgs> WithModulesFrom(IModuleSource moduleSource, TArgs args)
    {
        this.LoadFrom(moduleSource, args);

        return this;
    }

    /// <summary>
    /// Loads modules from the given source
    /// </summary>
    /// <param name="moduleSource">The module source</param>
    /// <param name="args">The args to pass during module initialization</param>
    public void LoadFrom(IModuleSource moduleSource, TArgs args)
    {
        this._modules.AddRange(ModuleLoader.Load<TModule, TArgs>(moduleSource, args));

        this._moduleSources.Add(moduleSource);
    }
}