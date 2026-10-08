using Serilog;
using Shiki.Common.Identity;
using Shiki.ModuleManagement.Module;
using Shiki.ModuleManagement.Source;

namespace Shiki.ModuleManagement.Loader;

/// <summary>
/// Instantiates (or "loads") modules
/// </summary>
public static class ModuleLoader
{
    /// <summary>
    /// Instantiates all modules in the provided IModuleSource
    /// </summary>
    /// <param name="source">The module source to source module types from</param>
    /// <returns>A list of loaded modules</returns>
    public static IEnumerable<KeyValuePair<Identifier, IModule>> Load(IModuleSource source)
        => Load<IModule>(source.GetTypes<IModule>());
    
    /// <summary>
    /// Instantiates all modules in the provided IModuleSource
    /// </summary>
    /// <param name="source">The module source to source module types from</param>
    /// <typeparam name="TModule">The module type</typeparam>
    /// <returns>A list of loaded modules</returns>
    public static IEnumerable<KeyValuePair<Identifier, TModule>> Load<TModule>(IModuleSource source)
        where TModule : IModule => Load<TModule>(source.GetTypes<TModule>());

    /// <summary>
    /// Instantiates all modules that fit the provided module types
    /// </summary>
    /// <param name="types">The module types</param>
    /// <typeparam name="TModule">The module type</typeparam>
    /// <returns>A list of loaded modules</returns>
    public static IEnumerable<KeyValuePair<Identifier, TModule>> Load<TModule>(IEnumerable<Type> types)
        where TModule : IModule
    {
        ILogger logger = Log.ForContext("SourceContext", $"ModuleLoader for {typeof(TModule).Name}");

        return types
              .Where(t => typeof(TModule).IsAssignableFrom(t) && !t.IsAbstract)
              .Select<Type, TModule?>(t =>
               {
                   try
                   {
                       return (TModule?)Activator.CreateInstance(t);
                   }
                   catch (MissingMethodException ex)
                   {
                       logger.Error(ex, "Failed to load module {TypeName} because no parameterless constructor was found", t.Name);

                       return default;
                   }
                   catch (Exception ex)
                   {
                       logger.Error(ex, "Exception thrown while loading module {TypeName}", t.FullName);

                       return default;
                   }
               })
              .OfType<TModule>()
              .Select(m => new KeyValuePair<Identifier, TModule>(m.Id, m));
    }
}