global using PolymorphicTypeStorage = System.Collections.Generic.Dictionary<System.Type, Shiki.Common.Serialization.Polymorphism.PolymorphicSerializableAttribute>;
namespace Shiki.Common.Serialization.Polymorphism.Source;

/// <summary>
/// Source for polymorphic types
/// </summary>
public interface IPolymorphicTypeSource
{
    /// <summary>
    /// Creates and loads a new PolymorphicTypeStorage
    /// </summary>
    /// <returns>A new PolymorphicTypeStorage</returns>
    public static abstract PolymorphicTypeStorage Load();
}