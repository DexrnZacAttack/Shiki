using System.Reflection;

namespace Shiki.Common.Serialization.Registry.Validators;

public interface IHasAttributeTypeRegistryValidator<TAttribute> : IGlobalTypeRegistryValidator
    where TAttribute : Attribute
{
    static Type IGlobalTypeRegistryValidator.GetScope(Type t) => typeof(TAttribute);
    
    static bool ITypeRegistryValidator.Validate(Type t) => t.GetCustomAttribute<TAttribute>() != null;
    static string ITypeRegistryValidator.GetKey(Type t) => nameof(TAttribute);
}