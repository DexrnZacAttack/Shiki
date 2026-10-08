namespace Shiki.Common.Serialization.Registry;

/// <summary>
/// Type registry interface
/// </summary>
public interface ITypeRegistry
{
    /// <summary>
    /// Types stored in the type registry
    /// </summary>
    IReadOnlyBidirectionalDictionary<string, Type> Types { get; }

    /// <summary>
    /// Gets a type from the type registry
    /// </summary>
    /// <param name="key">The key</param>
    /// <returns>The type, if present</returns>
    Type? GetType(string key);
    
    /// <summary>
    /// Gets a key of a given type in the type registry
    /// </summary>
    /// <param name="type">The type</param>
    /// <returns>The key to that type, if present</returns>
    string? GetKey(Type type);
}