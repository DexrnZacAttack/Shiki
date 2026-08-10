using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Schema;

namespace Shiki.Tests.Util;

public class SchemaGenerator(JsonSerializerOptions options)
{
    public async Task<string> GenerateSchema<T>() => await GenerateSchema(typeof(T));
    
    public async Task<string> GenerateSchema(Type type)
    {
        JsonNode schema = options.GetJsonSchemaAsNode(type);
        if (schema is JsonObject sc)
            sc.Insert(0, "$schema", "https://json-schema.org/draft/2020-12/schema");

        await File.WriteAllTextAsync($"{nameof(type)}.schema.json", schema.ToString());
        return schema.ToString();
    }
}