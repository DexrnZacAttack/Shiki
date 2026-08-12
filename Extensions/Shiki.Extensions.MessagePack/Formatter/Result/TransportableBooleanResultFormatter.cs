using MessagePack;
using MessagePack.Formatters;
using Shiki.Common.Result;

namespace Shiki.Extensions.MessagePack.Formatter.Result;

public class TransportableBooleanResultFormatter : IMessagePackFormatter<TransportableBooleanResult>
{
    private const int ARRAY_CT = 1;
    
    public void Serialize(ref MessagePackWriter writer, TransportableBooleanResult value, MessagePackSerializerOptions options)
    {
        if (value.Equals(default))//if result is default, which is automatically unsuccessful really
        {
            writer.WriteNil();
            return;
        }

        writer.WriteArrayHeader(ARRAY_CT);

        if (value.Success)
        {
            writer.WriteNil();
        }
        else
        {
            ResultExceptionDtoFormatter.Instance.Serialize(ref writer, value.Error, options);
        }
    }

    public TransportableBooleanResult Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            throw new
                MessagePackSerializationException($"Invalid result type");
        }
        
        int c = reader.ReadArrayHeader();
        if (c < ARRAY_CT)
        {
            throw new
                MessagePackSerializationException($"Invalid array length");
        }

        TransportableBooleanResult res = reader.TryReadNil() 
                                             ? new TransportableBooleanResult()
                                             : new TransportableBooleanResult(ResultExceptionDtoFormatter.Instance.Deserialize(ref reader, options));
        
        
        for (int ect = ARRAY_CT; ect < c; ect++)
        {
            reader.Skip();
        }

        return res;
    }
}