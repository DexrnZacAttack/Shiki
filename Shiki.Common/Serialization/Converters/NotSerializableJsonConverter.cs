using System.Text.Json;
using System.Text.Json.Serialization;

namespace Shiki.Common.Serialization.Converters;

/// <summary>
/// JSON converter that always throws NotSupportedException when trying to serialize/deserialize.
/// </summary>
public class NotSerializableJsonConverter<TUnused> : JsonConverter<TUnused>
{
    /// <inheritdoc/>
    public override TUnused? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotSupportedException("This object is not deserializable.");
    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, TUnused value, JsonSerializerOptions options) => throw new NotSupportedException("This object is not serializable.");
}