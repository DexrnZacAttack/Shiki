using MessagePack;
using MessagePack.Formatters;
using Shiki.Common.Identity.Slug;
using Shiki.Common.Identity.Slug.Formatting;

namespace Shiki.Extensions.MessagePack.Formatter.Identity.Slug;

public class SlugMessagePackFormatter<TFormatter> : IMessagePackFormatter<Slug<TFormatter>>
where TFormatter : ISlugFormatter
{
    public void Serialize(ref MessagePackWriter writer, Slug<TFormatter> value, MessagePackSerializerOptions options)
    {
        writer.Write(value.Value);
    }

    public Slug<TFormatter> Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil()) throw new InvalidOperationException("Tried to deserialize null Slug");

        string? s = reader.ReadString();
        if (s == null)
        {
            throw new InvalidOperationException("Tried to deserialize null Slug");
        }
        
        return new Slug<TFormatter>(s);
    }
}