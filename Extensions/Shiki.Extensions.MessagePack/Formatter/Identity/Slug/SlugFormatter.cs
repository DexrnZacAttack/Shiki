using MessagePack;
using MessagePack.Formatters;
using Shiki.Common.Identity.Slug;
using Shiki.Common.Identity.Slug.Formatting;

namespace Shiki.Extensions.MessagePack.Formatter.Identity.Slug;

public class SlugFormatter<TFormatter> : IMessagePackFormatter<Slug<TFormatter>?>
where TFormatter : ISlugFormatter
{
    public void Serialize(ref MessagePackWriter writer, Slug<TFormatter>? value, MessagePackSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNil();
            return;
        }
        
        writer.Write(value.Value.Value);
    }

    public Slug<TFormatter>? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil()) return null;

        string? s = reader.ReadString();
        if (s == null)
        {
            return null;
        }
        
        return new Slug<TFormatter>(s);
    }
}