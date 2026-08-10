using System.Text.Json;
using System.Text.Json.Serialization;
using Shiki.Common.Result.Serialization.Types;

namespace Shiki.Common.Result.Serialization;

/// <summary>
/// Converts a TransportableBooleanResult to and from JSON
/// </summary>
public class BooleanResultJsonConverter : JsonConverter<TransportableBooleanResult>
{
    /// <inheritdoc/>
    public override TransportableBooleanResult Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using JsonDocument doc = JsonDocument.ParseValue(ref reader);
        JsonElement root = doc.RootElement;

        if (!root.TryGetProperty("success", out JsonElement success))
            return default;
        
        bool s = success.GetBoolean();

        if (s) return new TransportableBooleanResult(null);
            
        if (!root.TryGetProperty("exception", out JsonElement ex))
        {
            return default;
        }
            
        ResultExceptionDto? exception = ex.Deserialize<ResultExceptionDto>(options);
        return exception == null ? default : new TransportableBooleanResult(exception);

    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, TransportableBooleanResult value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteBoolean("success", value.Success);
 
        if (!value.Success)
        {
            writer.WritePropertyName("exception");
            JsonSerializer.Serialize(writer, value.Error, options);
        }
        
        writer.WriteEndObject();
    }
}