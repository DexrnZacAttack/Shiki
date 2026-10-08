using MessagePack;
using MessagePack.Formatters;
using Shiki.Common.Extensions;
using Shiki.Common.Result;

namespace Shiki.Extensions.MessagePack.Formatter.Result;

public class TransportableBooleanResultMessagePackFormatter : IMessagePackFormatter<TransportableBooleanResult>
{
    private const int ARRAY_CT = 1;
    
    public void Serialize(ref MessagePackWriter writer, TransportableBooleanResult value, MessagePackSerializerOptions options)
    {
        writer.WriteArrayHeader(ARRAY_CT);

        if (value.IsSuccessful)
        {
            writer.WriteNil();
        }
        else
        {
            ResultExceptionDtoMessagePackFormatter.Instance.Serialize(ref writer, value.Error, options);
        }
    }

    public TransportableBooleanResult Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            return new BooleanResult<MessagePackSerializationException>(new MessagePackSerializationException("Nil written with no array count").WithCurrentStackTrace());
        }
        
        int c = reader.ReadArrayHeader();
        if (c < ARRAY_CT)
        {
            return new BooleanResult<MessagePackSerializationException>(new MessagePackSerializationException("Invalid array length").WithCurrentStackTrace());
        }

        TransportableBooleanResult res = reader.TryReadNil() 
                                             ? TransportableBooleanResult.Successful
                                             : new TransportableBooleanResult(ResultExceptionDtoMessagePackFormatter.Instance.Deserialize(ref reader, options));
        
        
        for (int ect = ARRAY_CT; ect < c; ect++)
        {
            reader.Skip();
        }

        return res;
    }
}