using MessagePack;
using MessagePack.Formatters;
using Shiki.Common.Identity;

namespace Shiki.Extensions.MessagePack.Formatter.Identity;

public class IdentifierFormatter : IMessagePackFormatter<Identifier?>
{
    private const int IdentifierPropertyCount = 2;
    
    public void Serialize(ref MessagePackWriter writer, Identifier? value, MessagePackSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNil();
            return;
        }

        writer.WriteArrayHeader(IdentifierPropertyCount); // namespace, path
        writer.Write(value.Namespace);
        
        writer.WriteArrayHeader(value.Path.Length);
        foreach (string p in value.Path)
        {
            writer.Write(p);
        }
    }

    public Identifier? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil()) return null;

        int propertiesCount = reader.ReadArrayHeader();
        if (propertiesCount != IdentifierPropertyCount)
        {
            throw new MessagePackSerializationException($"Properties count does not match expected for Identifier: {propertiesCount}/{IdentifierPropertyCount}");
        }
        
        string? nmsp = reader.ReadString();
        if (nmsp == null)
        {
            throw new MessagePackSerializationException("Missing namespace in expected Identifier!!!");
        }

        List<string> path = [];
        
        int len = reader.ReadArrayHeader();
        for (int i = 0; i < len; i++)
        {
            string? p = reader.ReadString();
            if (p == null)
            {
                throw new
                    MessagePackSerializationException($"Expected part of path string at position {i} in array of length {p}, but got null????");
            }
            
            path.Add(p);
        }
        
        return new Identifier(nmsp, path);
    }
}