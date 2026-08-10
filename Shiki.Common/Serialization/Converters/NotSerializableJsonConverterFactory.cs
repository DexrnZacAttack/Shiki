using System.Text.Json;
using System.Text.Json.Serialization;

namespace Shiki.Common.Serialization.Converters;

/// <summary>
/// Generic JSON factory that always spits out an instance of NotSerializableJsonConverter with your type
/// </summary>
public class NotSerializableJsonConverterFactory : JsonConverterFactory
{
    /// <inheritdoc/>
    public override bool CanConvert(Type typeToConvert) => true;

    /// <inheritdoc/>
    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        => (JsonConverter?)Activator.CreateInstance(typeof(NotSerializableJsonConverter<>).MakeGenericType(typeToConvert));
}