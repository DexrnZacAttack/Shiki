using System.Text.Json;
using System.Text.Json.Serialization;
using Shiki.Common.Factory;

namespace Shiki.Common.Serialization.Converters;

/// <summary>
/// Basic string type converter for types that can be converted to and from a String
/// </summary>
/// <typeparam name="TSelf">The constructable</typeparam>
public class StringJsonConverter<TSelf> : JsonConverter<TSelf>
    where TSelf : IFactoryConstructable<TSelf, string>
{
    /// <inheritdoc />
    public override TSelf? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? value = reader.GetString();
        if (value == null)
        {
            return default;
        }

        return TSelf.CreateInstance(value);
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, TSelf value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }

    /// <inheritdoc />
    public override TSelf ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert,
                                             JsonSerializerOptions options) =>
        Read(ref reader, typeToConvert, options) ?? throw new JsonException("Failed to decode string property");

    /// <inheritdoc />
    public override void
        WriteAsPropertyName(Utf8JsonWriter writer, TSelf value, JsonSerializerOptions options) =>
        writer.WritePropertyName(value.ToString() ?? string.Empty);
}