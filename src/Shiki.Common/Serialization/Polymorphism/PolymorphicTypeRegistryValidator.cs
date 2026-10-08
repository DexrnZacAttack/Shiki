using System.Reflection;
using Shiki.Common.Serialization.Registry;
using Shiki.Common.Serialization.Registry.Validators;

namespace Shiki.Common.Serialization.Polymorphism;

public abstract class PolymorphicTypeRegistryValidator : IHasAttributeTypeRegistryValidator<PolymorphicSerializableAttribute>
{
    static Type IGlobalTypeRegistryValidator.GetScope(Type t) => t.GetCustomAttribute<PolymorphicSerializableAttribute>()!.BaseType;
    static string ITypeRegistryValidator.GetKey(Type t) => t.GetCustomAttribute<PolymorphicSerializableAttribute>()!.Id;
}