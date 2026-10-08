using Shiki.Common.Serialization.Registry;

namespace Shiki.Common.Serialization.Polymorphism;

/// <summary>
/// Simple accessor class for the correct GlobalTypeRegistry with PolymorphicTypeRegistryValidator
/// </summary>
public static class PolymorphicTypeRegistry
{
    public static IReadOnlyDictionary<Type, IReadOnlyBidirectionalDictionary<string, Type>> GlobalTypes =>
        GlobalTypeRegistry<PolymorphicTypeRegistryValidator>.GlobalTypes;
}

/// <summary>
/// Registry/Cache of types with the PolymorphicSerializableAttribute
/// </summary>
/// <typeparam name="TBase"></typeparam>
public class PolymorphicTypeRegistry<TBase> : SingletonTypeRegistry<PolymorphicTypeRegistry<TBase>, PolymorphicTypeRegistryValidator, TBase>;