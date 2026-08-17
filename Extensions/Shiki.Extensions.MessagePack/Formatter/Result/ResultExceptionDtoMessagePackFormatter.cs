using MessagePack;
using MessagePack.Formatters;
using Shiki.Common.Result.Serialization.Types;

namespace Shiki.Extensions.MessagePack.Formatter.Result;

public class ResultExceptionDtoMessagePackFormatter : IMessagePackFormatter<ResultExceptionDto?>
{
    private const int MAX_DEPTH = 10;
    private const int ARRAY_CT = 3;
    
    public static readonly ResultExceptionDtoMessagePackFormatter Instance = new();
    
    public void Serialize(ref MessagePackWriter writer, ResultExceptionDto? value, MessagePackSerializerOptions options)
    {
        int loopCt = 0;
        
        while (true)
        {
            if (value == null || loopCt >= MAX_DEPTH)
            {
                writer.WriteNil();
                return;
            }
            
            writer.WriteArrayHeader(ARRAY_CT);

            writer.Write(value.Type);
            writer.Write(value.Message);
            
            value = value.InnerException;
            loopCt++;
        }
    }

    public ResultExceptionDto? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        return InternalDeserialize(ref reader, options, 0);
    }

    private ResultExceptionDto? InternalDeserialize(ref MessagePackReader reader, MessagePackSerializerOptions options,
                                                    int loopCt)
    {
        if (reader.TryReadNil()) return null;

        if (loopCt >= MAX_DEPTH)
        {
            reader.Skip();
            return null;
        }
        
        int c = reader.ReadArrayHeader();
        if (c < ARRAY_CT)
        {
            throw new
                MessagePackSerializationException($"Invalid array length");
        }
        
        string? type = reader.ReadString();
        if (type == null)
        {
            throw new
                MessagePackSerializationException($"No type string present");
        }
        
        string? message = reader.ReadString();
        if (message == null)
        {
            throw new
                MessagePackSerializationException($"No message string present");
        }

        ResultExceptionDto? inner = InternalDeserialize(ref reader, options, loopCt + 1);
        for (int ect = ARRAY_CT; ect < c; ect++)
        {
            reader.Skip();
        }
        
        return new ResultExceptionDto(type, message, inner);
    }
}