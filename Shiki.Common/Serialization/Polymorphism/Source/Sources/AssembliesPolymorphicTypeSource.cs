using System.Reflection;
using Microsoft.Extensions.DependencyModel;

namespace Shiki.Common.Serialization.Polymorphism.Source.Sources;

/// <summary>
/// Sources polymorphic types from all loaded assemblies
/// </summary>
public class AssembliesPolymorphicTypeSource : IPolymorphicTypeSource
{
    /// <inheritdoc/>
    public static PolymorphicTypeStorage Load()
    {
        var loaded = AppDomain.CurrentDomain.GetAssemblies()
                              .ToDictionary(a => a.GetName().FullName);

        var referenced =
            DependencyContext.Default?.RuntimeLibraries.SelectMany(l => l.GetDefaultAssemblyNames(DependencyContext
                                                                      .Default)) ?? [];

        foreach (var assembly in referenced)
        {
            if (!loaded.ContainsKey(assembly.FullName))
            {
                try
                {
                    Assembly.Load(assembly);
                } catch {}
            }
        }
        
        return AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(assembly =>
                         {
                             try
                             {
                                 return assembly.GetTypes();
                             }
                             catch (ReflectionTypeLoadException ex)
                             {
                                 return ex.Types.Where(t => t != null).Select(t => t!); 
                             }
                             catch
                             {
                                 return [];
                             }
                         })
                        .Select(t => new { Type = t, Attribute = t!.GetCustomAttribute<PolymorphicSerializableAttribute>() })
                        .Where(x => x.Attribute != null)
                        .ToDictionary(t => t.Type, t => t.Attribute!);
    }
}