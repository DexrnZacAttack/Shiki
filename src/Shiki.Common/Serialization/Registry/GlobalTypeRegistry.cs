using System.Collections.Concurrent;
using System.Reflection;

namespace Shiki.Common.Serialization.Registry;

/// <summary>
/// Global registry of types
/// </summary>
/// <typeparam name="TValidator">Type validator</typeparam>
public static class GlobalTypeRegistry<TValidator> 
    where TValidator : IGlobalTypeRegistryValidator
{
    /// <summary>
    /// Dict of all global types for this GlobalTypeRegistry
    /// </summary>
    public static IReadOnlyDictionary<Type, IReadOnlyBidirectionalDictionary<string, Type>> GlobalTypes
    {
        get
        {
            RunHydrate();//TODO can't I avoid this whole rehydrating by instead running on assembly load event???
            return _globalTypes.ToDictionary(kvp => kvp.Key, IReadOnlyBidirectionalDictionary<string, Type> (kvp) => kvp.Value);
        }
    }

    /// <summary>
    /// Dict of all global types for this GlobalTypeRegistry
    /// </summary>
    private static readonly ConcurrentDictionary<Type, ConcurrentBidirectionalDictionary<string, Type>> _globalTypes =
        [];

    /// <summary>
    /// All loaded and processed assemblies
    /// </summary>
    private static readonly HashSet<Assembly> _loaded = [];
#if !SHIKI_GTR_SKIP_LENCHECK
    private static int _loadedCt = 0;
#endif
    
    /// <summary>
    /// Lock for hydration
    /// </summary>
    private static readonly Lock _lock = new();
    
    /// <summary>
    /// Hydrates the type registry
    /// </summary>
    public static void RunHydrate()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        //technically this can cause issues if the developer unloads an assembly and then loads a new one before the registry is hydrated...
        //though I don't know how often this will happen, so it's not too bad of an idea to task a fast exit if possible I think
        //itll be replaced soon anyway :shrug:
        #if !SHIKI_GTR_SKIP_LENCHECK
        if (assemblies.Length == Volatile.Read(ref _loadedCt))//I think I need volatile for this????
            return;
        #endif
        
        lock (_lock)
        {
#if !SHIKI_GTR_SKIP_LENCHECK
            bool any = false;
#endif
            foreach (Assembly assembly in assemblies)
            {
                if (!_loaded.Add(assembly)) continue;

#if !SHIKI_GTR_SKIP_LENCHECK
                any = true;
#endif
                
                try
                {
                    HydrateTypes(assembly.GetTypes());
                }
                catch (ReflectionTypeLoadException e)
                {
                    HydrateTypes(e.Types);
                }
            }
#if !SHIKI_GTR_SKIP_LENCHECK
            if (any)
            {
                Volatile.Write(ref _loadedCt, _loaded.Count);
            }
#endif
        }
    }

    /// <summary>
    /// Hydrates a set of types (I think shit name?)
    /// </summary>
    /// <param name="types">types to hydrate</param>
    private static void HydrateTypes(IEnumerable<Type?> types)
    {
        foreach (Type? t in types)
        {
            if (t == null) continue;
            if (!TValidator.Validate(t)) continue;
            
            var d = _globalTypes.GetOrAdd(TValidator.GetScope(t),
                                          _ => new ConcurrentBidirectionalDictionary<string,
                                              Type>());
            d[TValidator.GetKey(t)] = t;
        }
    }

    /// <summary>
    /// Gets the storage for a given scope type
    /// </summary>
    /// <param name="scope">The scope type</param>
    /// <returns>The scope storage</returns>
    public static ConcurrentBidirectionalDictionary<string, Type> GetScope(Type scope)
    {
        RunHydrate();
        return _globalTypes.GetOrAdd(scope, _ => new ConcurrentBidirectionalDictionary<string, Type>());
    }
}