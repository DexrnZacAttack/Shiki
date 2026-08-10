using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Shiki.Common.Serialization.Polymorphism;

/// <summary>
/// Resolves classes in the PolymorphicTypeStorage into a JsonTypeInfo object
/// </summary>
/// <param name="types"></param>
public class PolymorphicTypeInfoResolver(PolymorphicTypeStorage types) : DefaultJsonTypeInfoResolver
{
    /// <inheritdoc/>
    public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
    {
        JsonTypeInfo ti = base.GetTypeInfo(type, options);

        var derived = types.Where(kvp => kvp.Value.BaseType == ti.Type).ToList();

        if (derived.Count > 0)
        {
            ti.PolymorphismOptions = new JsonPolymorphismOptions
            {
                TypeDiscriminatorPropertyName = "$type",
                IgnoreUnrecognizedTypeDiscriminators = true,
                UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization
            };

            foreach (var pt in derived)
            {
                ti.PolymorphismOptions.DerivedTypes.Add(new JsonDerivedType(pt.Key, pt.Value.Id));
            }
        }

        return ti;
    }
}