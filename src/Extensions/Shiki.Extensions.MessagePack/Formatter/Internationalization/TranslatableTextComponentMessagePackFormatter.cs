using MessagePack;
using MessagePack.Formatters;
using Shiki.Common.Identity;
using Shiki.Internationalization;
using Shiki.Internationalization.Component;

namespace Shiki.Extensions.MessagePack.Formatter.Internationalization;

public class TranslatableTextComponentMessagePackFormatter : IMessagePackFormatter<TranslatableTextComponent?>
{
    public void Serialize(ref MessagePackWriter writer, TranslatableTextComponent? value, MessagePackSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNil();
            return;
        }
        
        writer.WriteArrayHeader(2);
        MessagePackSerializer.Serialize(ref writer, value.TranslationId, options);
        writer.WriteArrayHeader(value.Arguments.Length);

        foreach (ITextComponent arg in value.Arguments)
        {
            MessagePackSerializer.Serialize(ref writer, arg, options);
        }
    }

    public TranslatableTextComponent? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil()) return null;

        if (!reader.TryReadArrayHeader(out int ct) || ct != 2)
        {
            throw new MessagePackSerializationException("Invalid array header for this object");
        }

        Identifier id = MessagePackSerializer.Deserialize<Identifier>(ref reader, options);

        if (!reader.TryReadArrayHeader(out int argsCt))
        {
            throw new MessagePackSerializationException("Failed to read array header");
        }

        ITextComponent[] args = new ITextComponent[argsCt];
        for (int i = 0; i < argsCt; i++)
        {
            args[i] = MessagePackSerializer.Deserialize<ITextComponent>(ref reader, options);
        }
        
        return new TranslatableTextComponent(id, args);
    }
}