using System.Text.Json;
using System.Text.Json.Serialization;
using Shiki.Common.Serialization.Converters;

namespace Shiki.Common.Identity.Slug.Serialization.Converters;

/// <summary>
/// Converts a Slug to and from JSON
/// </summary>
public class SlugJsonConverterFactory : JsonConverterFactory
{
    /// <inheritdoc/>
    public override bool CanConvert(Type typeToConvert) => typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(Slug<>);

    /// <inheritdoc/>
    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        => (JsonConverter?)Activator.CreateInstance(typeof(StringJsonConverter<>).MakeGenericType(typeToConvert));
}