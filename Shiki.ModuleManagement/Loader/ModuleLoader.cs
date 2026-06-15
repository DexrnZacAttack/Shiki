using Serilog;
using Shiki.ModuleManagement.Source;

namespace Shiki.ModuleManagement.Loader;

/// <summary>
/// Instantiates (or "loads") modules
/// </summary>
public static class ModuleLoader
{
    /// <summary>
    /// Instantiates and initializes all modules in the provided IModuleSource with the provided args
    /// </summary>
    /// <param name="source">The module source to source module types from</param>
    /// <param name="initArgs">The args to provide during module initialization</param>
    /// <typeparam name="TModule">The module type</typeparam>
    /// <typeparam name="TArgs">The args type</typeparam>
    /// <returns>A list of loaded modules</returns>
    public static IEnumerable<TModule> Load<TModule, TArgs>(IModuleSource source, TArgs initArgs)
        where TArgs : EventArgs
        where TModule : IModule<TArgs> => Load<TModule, TArgs>(source.GetTypes<TModule>(), initArgs);

    /// <summary>
    /// Instantiates and initializes all provided module types with the provided args
    /// </summary>
    /// <param name="types">The module types</param>
    /// <param name="initArgs">The args to provide during module initialization</param>
    /// <typeparam name="TModule">The module type</typeparam>
    /// <typeparam name="TArgs">The args type</typeparam>
    /// <returns>A list of loaded modules</returns>
    public static IEnumerable<TModule> Load<TModule, TArgs>(IEnumerable<Type> types, TArgs initArgs)
        where TArgs : EventArgs
        where TModule : IModule<TArgs>
    {
        ILogger logger = Log.Logger.ForContext("SourceContext", $"ModuleLoader for {typeof(TModule).Name}");

        return types
              .Where(t => typeof(TModule).IsAssignableFrom(t) && !t.IsAbstract)
              .Select<Type, TModule?>(t =>
               {
                   try
                   {
                       TModule? module = (TModule?)Activator.CreateInstance(t);
                       logger.Information("Loaded module '{Id}' ({TypeName})", module?.Id, t.Name);

                       module?.OnInitialize(initArgs);

                       return module;
                   }
                   catch (Exception ex)
                   {
                       logger.Error(ex, "Exception thrown while loading module {TypeName}", t.Name);

                       return default;
                   }
               })
              .OfType<TModule>();
    }
}