using System.Text.Json;
using System.Text.Json.Serialization;

namespace Shiki.Common.Result.Serialization;

/// <summary>
/// Creates a JsonConverterFactory based on the TransportableResult type
/// </summary>
public class ResultJsonConverterFactory : JsonConverterFactory
{
    /// <inheritdoc/>
    public override bool CanConvert(Type typeToConvert)
    {
        if (!typeToConvert.IsGenericType) return false;

        return typeToConvert.GetGenericTypeDefinition() == typeof(TransportableResult<>);
    }

    /// <inheritdoc/>
    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        Type tValue = typeToConvert.GetGenericArguments()[0];

        Type ct = typeof(ResultJsonConverter<,>).MakeGenericType(tValue, typeToConvert);
        return (JsonConverter?)Activator.CreateInstance(ct);
    }
}