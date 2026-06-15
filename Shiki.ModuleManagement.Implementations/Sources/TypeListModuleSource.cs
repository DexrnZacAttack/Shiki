using Serilog;
using Shiki.ModuleManagement.Source;

namespace Shiki.ModuleManagement.Implementations.Sources;

/// <summary>
/// Returns a list of types provided during class creation
/// </summary>
/// <param name="modules">The modules to provide</param>
public class TypeListModuleSource(params List<Type> modules) : IModuleSource
{
    /// <inheritdoc/>
    public ILogger Logger => Log.Logger.ForContext<TypeListModuleSource>();

    /// <inheritdoc/>
    public IEnumerable<Type> GetTypes<TModule>() where TModule : IModule => modules;
}