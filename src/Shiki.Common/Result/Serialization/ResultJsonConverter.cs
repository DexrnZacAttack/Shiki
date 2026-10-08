using System.Text.Json;
using System.Text.Json.Serialization;
using Shiki.Common.Factory;
using Shiki.Common.Result.Serialization.Types;

namespace Shiki.Common.Result.Serialization;

/// <summary>
/// Converts a Result type to and from JSON
/// </summary>
public class ResultJsonConverter<TValue, TResult> : JsonConverter<TResult>
where TResult : IResult<TValue, ResultExceptionDto>, IFactoryConstructable<TResult, TValue?, ResultExceptionDto?>
{
    /// <inheritdoc/>
    public override TResult? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using JsonDocument doc = JsonDocument.ParseValue(ref reader);
        JsonElement root = doc.RootElement;

        if (root.TryGetProperty("success", out JsonElement success) && success.GetBoolean() == true)
        {
            if (!root.TryGetProperty("value", out JsonElement v))
            {
                return default;
            }

            TValue? value = v.Deserialize<TValue>(options);
            if (value == null)
            {
                return default;
            }

            return TResult.CreateInstance(value, null);
        }
            
        if (!root.TryGetProperty("exception", out JsonElement ex))
        {
            return default;
        }
            
        ResultExceptionDto? exception = ex.Deserialize<ResultExceptionDto>(options);
        if (exception == null)
        {
            return default;
        }
            
        return TResult.CreateInstance(default, exception);
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, TResult value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteBoolean("success", value.HasValue);

        if (value.HasValue)
        {
            writer.WritePropertyName("value");
            JsonSerializer.Serialize(writer, value.Value, options);
        }
        else
        {
            writer.WritePropertyName("exception");
            JsonSerializer.Serialize(writer, value.Error, options);
        }
        
        writer.WriteEndObject();
    }
}