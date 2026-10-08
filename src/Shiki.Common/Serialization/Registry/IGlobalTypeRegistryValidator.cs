namespace Shiki.Common.Serialization.Registry;

/// <summary>
/// Validator for types in the GlobalTypeRegistry
/// </summary>
public interface IGlobalTypeRegistryValidator : ITypeRegistryValidator
{
    /// <summary>
    /// Gets the scope type
    /// </summary>
    /// <param name="t">The type</param>
    /// <returns>The scope type</returns>
    static abstract Type GetScope(Type t);
}