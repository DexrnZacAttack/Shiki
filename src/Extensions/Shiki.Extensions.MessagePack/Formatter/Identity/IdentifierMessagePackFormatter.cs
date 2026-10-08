using MessagePack;
using MessagePack.Formatters;
using Shiki.Common.Identity;
using Shiki.Common.Util;

namespace Shiki.Extensions.MessagePack.Formatter.Identity;

/// <summary>
/// A functionally equivalent interface to IMessagePackFormatter
///
/// Used to allow for variations based on e.g. a variable when the actual IMessagePackFormatter is constructed
/// </summary>
/// <typeparam name="T">The type to format</typeparam>
public interface IMessagePackFormatterVariation<T>
{
    void Serialize(ref MessagePackWriter writer, T value, MessagePackSerializerOptions options);
    T Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options);
}

/// <summary>
/// IMessagePackFormatterVariation but with a singleton property
/// </summary>
/// <typeparam name="T">The type to format</typeparam>
/// <typeparam name="TSelf">Self</typeparam>
public interface IMessagePackFormatterVariation<out TSelf, T> : IMessagePackFormatterVariation<T>, ISingleton<TSelf>
where TSelf : IMessagePackFormatterVariation<T>, ISingleton<TSelf>;

public class IdentifierMessagePackFormatter(IMessagePackFormatterVariation<Identifier?> variation) : IMessagePackFormatter<Identifier?>
{
    public IdentifierMessagePackFormatter() : this(IdentifierDefaultMessagePackFormatterVariation.Instance) {}
    
    
    /// <inheritdoc/>
    public void Serialize(ref MessagePackWriter writer, Identifier? value, MessagePackSerializerOptions options) => variation.Serialize(ref writer, value, options);
    
    /// <inheritdoc/>
    public Identifier? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options) => variation.Deserialize(ref reader, options);
}

public class IdentifierDefaultMessagePackFormatterVariation : IMessagePackFormatterVariation<IdentifierDefaultMessagePackFormatterVariation, Identifier?>
{
    public static IdentifierDefaultMessagePackFormatterVariation Instance { get; } = new();
 
    private const int IdentifierPropertyCount = 2;
    
    private IdentifierDefaultMessagePackFormatterVariation() {}
    
    /// <inheritdoc/>
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


public class IdentifierTextMessagePackFormatterVariation : IMessagePackFormatterVariation<IdentifierTextMessagePackFormatterVariation, Identifier?>
{
    public static IdentifierTextMessagePackFormatterVariation Instance { get; } = new();
    
    private IdentifierTextMessagePackFormatterVariation() {}
    
    /// <inheritdoc/>
    public void Serialize(ref MessagePackWriter writer, Identifier? value, MessagePackSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNil();
            return;
        }

        writer.Write(value.ToString());
    }
    
    public Identifier? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil()) return null;
        
        string? id = reader.ReadString();
        if (id == null)
        {
            throw new MessagePackSerializationException("Identifier string is null");
        }
        
        return Identifier.CreateInstance(id);
    }
}
