using System.Collections.Concurrent;

namespace Shiki.Common.Serialization.Registry;

/// <summary>
/// Scoped TypeRegistry via typeparam
/// </summary>
/// <typeparam name="TValidator">The type validator</typeparam>
/// <typeparam name="TScope">The scope</typeparam>
public abstract class TypeRegistry<TValidator, TScope> : TypeRegistry<TValidator>
    where TValidator : IGlobalTypeRegistryValidator
{
    /// <summary>
    /// Creates a new TypeRegistry with the scope typeparam
    /// </summary>
    protected TypeRegistry() : base(typeof(TScope)) {}
}

/// <summary>
/// Type registry/cache 
/// </summary>
/// <typeparam name="TValidator">Type validator</typeparam>
public abstract class TypeRegistry<TValidator> : ITypeRegistry
    where TValidator : IGlobalTypeRegistryValidator
{
    private readonly ConcurrentBidirectionalDictionary<string, Type> _types;

    /// <inheritdoc/>
    public IReadOnlyBidirectionalDictionary<string, Type> Types
    {
        get
        {
            GlobalTypeRegistry<TValidator>.RunHydrate();
            return _types;
        }
    }

    /// <summary>
    /// Creates a new TypeRegistry with the given scope
    /// </summary>
    /// <param name="scope">The scope</param>
    protected TypeRegistry(Type scope)
    {
        _types = GlobalTypeRegistry<TValidator>.GetScope(scope);
    }

    /// <inheritdoc/>
    public Type? GetType(string key)
    {
        //if we have already, return it.
        if (_types.TryGetValue(key, out Type? type)) return type;

        //otherwise, hydrate, maybe we missed new assembly load
        GlobalTypeRegistry<TValidator>.RunHydrate();

        //grab again
        _types.TryGetValue(key, out type);
        return type;
    }

    /// <inheritdoc/>
    public string? GetKey(Type type)
    {
        //if we have already, return it.
        if (_types.Inverse.TryGetValue(type, out string? key)) return key;

        //otherwise, hydrate, maybe we missed new assembly load
        GlobalTypeRegistry<TValidator>.RunHydrate();

        //grab again
        _types.Inverse.TryGetValue(type, out key);
        return key;
    }
}