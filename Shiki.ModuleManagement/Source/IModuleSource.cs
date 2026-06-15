using Serilog;

namespace Shiki.ModuleManagement.Source;

/// <summary>
/// Base module source interface, to be implemented by other sources.
/// </summary>
public interface IModuleSource
{
    /// <summary>
    /// The module source logger
    /// </summary>
    protected ILogger Logger { get; }
    
    /// <summary>
    /// Returns a list of module types which will be instantiated in the ModuleLoader.
    /// </summary>
    /// <typeparam name="TModule">The module type</typeparam>
    /// <returns>The module types</returns>
    IEnumerable<Type> GetTypes<TModule>() where TModule : IModule;
}