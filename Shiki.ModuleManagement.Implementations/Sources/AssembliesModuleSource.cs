using System.Reflection;
using System.Runtime.Loader;
using Serilog;
using Shiki.ModuleManagement.Source;

namespace Shiki.ModuleManagement.Implementations.Sources;

using AssemblyNameType = string;

/// <summary>
/// Loads module assemblies from a directory containing directories with the same name as the DLL file to load
/// </summary>
/// <param name="path">The directory to search in</param>
public class AssembliesModuleSource(string path) : IModuleSource
{
    /// <summary>
    /// The assembly load contexts, we need to keep this to prevent dotnet from unloading them due to disposal
    /// </summary>
    public Dictionary<AssemblyNameType, AssemblyLoadContext> LoadContexts { get; } = [];

    /// <inheritdoc/>
    public ILogger Logger { get; } = Log.Logger.ForContext<AssembliesModuleSource>();

    /// <inheritdoc/>
    public IEnumerable<Type> GetTypes<TModule>()
        where TModule : IModule
    {
        List<Type> types = [];

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        foreach (string directory in Directory.EnumerateDirectories(path))
        {
            string directoryName = Path.GetFileName(directory);

            AssemblyLoadContext asmLoadCtx = new(directoryName, true);
            asmLoadCtx.Resolving += (ctx, assemblyName) =>
            {
                string assemblyPath = Path.GetFullPath(Path.Combine(directoryName, $"{assemblyName.Name}.dll"));

                return File.Exists(assemblyPath) ? ctx.LoadFromAssemblyPath(assemblyPath) : null;
            };

            string fullPath = Path.Combine(Path.GetFullPath(directory),
                                           $"{directoryName}.dll");
            try
            {
                Assembly assembly =
                    asmLoadCtx.LoadFromAssemblyPath(fullPath); // load by dir name (UniScan.Uniden/UniScan.Uniden.dll)

                types.AddRange(GetTypesFromAssembly<TModule>(assembly));
            }
            catch (BadImageFormatException ex)
            {
                Logger.Error("Failed to load assembly module at '{Name}'", fullPath, ex);
                Logger.Warning("Skipping module '{Name}'", directoryName);
                
                asmLoadCtx.Unload();
                continue;
            }

            LoadContexts.Add(directoryName, asmLoadCtx);
        }

        return types;
    }

    /// <summary>
    /// Returns a list of types in an assembly that are derived from the given TModule type
    /// </summary>
    /// <param name="assembly">The assembly</param>
    /// <typeparam name="TModule">The module type</typeparam>
    /// <returns>All derivatives of TModule in the given assembly</returns>
    private static IEnumerable<Type> GetTypesFromAssembly<TModule>(Assembly assembly)
    {
        return assembly.GetTypes().Where(t => typeof(TModule).IsAssignableFrom(t) && t is { IsAbstract: false, IsInterface: false });
    }
}