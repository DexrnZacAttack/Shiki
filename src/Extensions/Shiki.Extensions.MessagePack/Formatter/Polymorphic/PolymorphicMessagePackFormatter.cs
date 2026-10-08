using MessagePack;
using MessagePack.Formatters;
using Shiki.Common.Serialization.Polymorphism;

namespace Shiki.Extensions.MessagePack.Formatter.Polymorphic;

public class PolymorphicMessagePackFormatter<TBase> : IMessagePackFormatter<TBase?>
{
    public void Serialize(ref MessagePackWriter writer, TBase? value, MessagePackSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNil();
            return;
        }

        Type t = value.GetType();
        string? id = PolymorphicTypeRegistry<TBase>.Instance.GetKey(t);
        if (id is null)
        {
            throw new KeyNotFoundException("Cannot serialize object of type '" + t.FullName + "' due to it not being registered in PolymorphicTypeRegistry for " + typeof(TBase).FullName);
        }

        writer.WriteArrayHeader(2);
        writer.Write(id);
        MessagePackSerializer.Serialize(t, ref writer, value, options);
    }

    public TBase? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil()) return default;

        int ct = reader.ReadArrayHeader();
        if (ct != 2) throw new MessagePackSerializationException("Invalid array header ct");

        string? id = reader.ReadString();
        if (id is null) throw new MessagePackSerializationException("id is null");

        Type? t = PolymorphicTypeRegistry<TBase>.Instance.GetType(id);
        if (t is null) throw new MessagePackSerializationException("Type not found in polymorphic registry");

        return (TBase?)MessagePackSerializer.Deserialize(t, ref reader, options);
    }
}