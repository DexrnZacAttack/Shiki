namespace Shiki.Common.Serialization.Polymorphism;

/// <summary>
/// Marks a class as a polymorphic serializable class
/// </summary>
/// <param name="baseType">The base class that this class should be deserialized as, will be added to list of derived types under the baseType when serializing/deserializing.</param>
/// <param name="id">The id written to JSON for this class</param>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true, Inherited = false)]
public class PolymorphicSerializableAttribute(Type baseType, string id) : System.Attribute
{
    /// <summary>
    /// The base class type, will be added to list of types when serializing/deserializing.
    /// </summary>
    public Type BaseType { get; } = baseType;
    /// <summary>
    /// The type id, will be serialized as <code>"$type": "{id}"</code>
    /// </summary>
    public string Id { get; } = id;
}

/// <summary>
/// Marks a class as a polymorphic serializable class
/// </summary>
/// <param name="id">The id written to JSON for this class</param>
/// <typeparam name="TBase">The base class type that this class should be deserialized as, will be added to list of derived types under TBase when serializing/deserializing.</typeparam>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true, Inherited = false)]
public class PolymorphicSerializableAttribute<TBase>(string id) : PolymorphicSerializableAttribute(typeof(TBase), id);

/// <summary>
/// Marks a class as a polymorphic serializable class
///
/// This constructor automatically uses the name of TBase as the type id.
/// </summary>
/// <typeparam name="TBase">The base class type that this class should be deserialized as, will be added to list of derived types under TBase when serializing/deserializing.</typeparam>
/// <typeparam name="TSelf">Self</typeparam>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true, Inherited = false)]
public class PolymorphicSerializableAttribute<TSelf, TBase>() : PolymorphicSerializableAttribute<TBase>(typeof(TSelf).Name)
{
}