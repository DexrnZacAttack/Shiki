namespace Shiki.Common.Serialization.Registry;

/// <summary>
/// Validator for types in a TypeRegistry
/// </summary>
public interface ITypeRegistryValidator
{
    /// <summary>
    /// Validates whether a type can be used in the TypeRegistry
    /// </summary>
    /// <param name="t">The type</param>
    /// <returns>Whether it can be used</returns>
    static abstract bool Validate(Type t);
    
    /// <summary>
    /// Gets the type key
    /// </summary>
    /// <param name="t">The type</param>
    /// <returns>The type key</returns>
    static abstract string GetKey(Type t);
}