using System.Reflection;
using Shiki.Common.Util;

namespace Shiki.Common.Serialization.Registry;

/// <summary>
/// TypeRegistry with a built-in singleton instance
/// </summary>
/// <typeparam name="TSelf">Self</typeparam>
/// <typeparam name="TValidator">Type validator</typeparam>
/// <typeparam name="TScope">Type scope</typeparam>
public abstract class SingletonTypeRegistry<TSelf, TValidator, TScope> : TypeRegistry<TValidator, TScope>, ISingleton<TSelf>
    where TSelf : SingletonTypeRegistry<TSelf, TValidator, TScope>
    where TValidator : IGlobalTypeRegistryValidator
{
    /// <summary>
    /// The lazy instance
    /// </summary>
    private static readonly Lazy<TSelf> _instance = new(() =>
    {
        ConstructorInfo? ctor = typeof(TSelf).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null, Type.EmptyTypes, null);
        if (ctor == null)
            throw new InvalidOperationException($"No no-args constructor found for registry {nameof(TSelf)}");
        
        return (TSelf)ctor.Invoke(null);
    });
    
    /// <inheritdoc/>
    public static TSelf Instance => _instance.Value;
}

/// <summary>
/// TypeRegistry with a built-in singleton instance
/// </summary>
/// <typeparam name="TSelf">Self</typeparam>
/// <typeparam name="TValidator">Type validator</typeparam>
public abstract class SingletonTypeRegistry<TSelf, TValidator> : TypeRegistry<TValidator>, ISingleton<TSelf>
    where TSelf : SingletonTypeRegistry<TSelf, TValidator>
    where TValidator : IGlobalTypeRegistryValidator
{
    /// <summary>
    /// The lazy instance
    /// </summary>
    private static readonly Lazy<TSelf> _instance = new(() =>
    {
        ConstructorInfo? ctor =
            typeof(TSelf).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null,
                                         Type.EmptyTypes, null);
        if (ctor == null)
            throw new InvalidOperationException($"No no-args constructor found for registry '{nameof(TSelf)}'");

        return (TSelf)ctor.Invoke(null);
    });

    /// <inheritdoc/>
    public static TSelf Instance => _instance.Value;

    /// <summary>
    /// Constructs the SingletonTypeRegistry with the given scope
    /// </summary>
    /// <param name="scope">The scope</param>
    protected SingletonTypeRegistry(Type scope) : base(scope)
    {
    }
}