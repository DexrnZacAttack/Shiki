using MessagePack;
using MessagePack.Formatters;
using Shiki.Internationalization;
using Shiki.Internationalization.Component;

namespace Shiki.Extensions.MessagePack.Formatter.Internationalization;

public class AbsoluteTextComponentMessagePackFormatter : IMessagePackFormatter<AbsoluteTextComponent?>
{
    public void Serialize(ref MessagePackWriter writer, AbsoluteTextComponent? value, MessagePackSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNil();
            return;
        }
        
        writer.Write(value.Value);
    }

    public AbsoluteTextComponent? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil()) return null;

        string? s = reader.ReadString();
        return s == null ? throw new MessagePackSerializationException("invalid string") : new AbsoluteTextComponent(s);
    }
}