using Serilog;
using Shiki.ModuleManagement.Module;
using Shiki.ModuleManagement.Source;

namespace Shiki.ModuleManagement.Implementations.Sources;

/// <summary>
/// Returns a list of types provided during class creation
/// </summary>
/// <param name="modules">The modules to provide</param>
[Obsolete("Please manually add your modules to either the ModuleStorageBuilder or the ModuleStorage directly.")]
public class TypeListModuleSource(params List<Type> modules) : IModuleSource
{
    /// <inheritdoc/>
    public ILogger Logger => Log.ForContext<TypeListModuleSource>();

    /// <inheritdoc/>
    public IEnumerable<Type> GetTypes<TModule>() where TModule : IModule => modules;
}