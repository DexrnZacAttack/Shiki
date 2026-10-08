using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Shiki.Common.Factory;
using Shiki.Common.Identity.Slug.Formatting;
using Shiki.Common.Identity.Slug.Serialization.Converters;

namespace Shiki.Common.Identity.Slug;

/// <summary>
/// Simple interface for accessing the Value regardless of the Slug's TFormatter
/// </summary>
public interface ISlug
{
    /// <summary>
    /// The string stored in the slug
    /// </summary>
    string Value { get; }
}

/// <summary>
/// A simple string wrapper that enforces the format provided by the given formatter
/// </summary>
[Serializable]
[DataContract]
[JsonConverter(typeof(SlugJsonConverterFactory))]
[TypeConverter(typeof(SlugTypeConverterFactory))]
[DebuggerDisplay("{Value}")]
public readonly partial struct Slug<TFormatter> : ISlug, ISerializable, IEquatable<Slug<TFormatter>>, IComparable<Slug<TFormatter>>, IComparable
where TFormatter : ISlugFormatter
{
    /// <summary>
    /// The string
    /// </summary>
    private readonly string? _value;
    
    /// <inheritdoc/>
    [JsonPropertyName("value")]
    public string Value => _value ?? string.Empty;
    
    /// <summary>
    /// Creates a new Slug
    /// </summary>
    /// <param name="value">The value</param>
    [FactoryConstructable]
    public Slug(string value)
    {
        _value = TFormatter.Format(value);
    }

    /// <inheritdoc />
    public override string ToString() => Value;


    /// Converts the Slug to a String
    public static implicit operator string(Slug<TFormatter> slug) => slug.Value;

    /// <inheritdoc />
    public override int GetHashCode() => Value.GetHashCode();
    
    /// <inheritdoc />
    public int CompareTo(object? obj) => obj switch
    {
        null                   => 1,
        string str             => Value.CompareTo(str, StringComparison.Ordinal),
        Slug<TFormatter> other => CompareTo(other),
        _                      => throw new ArgumentException("Object is not a Slug")
    };
    
    /// <inheritdoc />
    public int CompareTo(Slug<TFormatter> other) => string.Compare(Value, other.Value, StringComparison.Ordinal);
    

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is Slug<TFormatter> other && Equals(other);
    }
    
    /// <inheritdoc />
    public bool Equals(Slug<TFormatter> other) => Value == other.Value;

    /// <summary>
    /// Compares two Slug instances for equality
    /// </summary>
    /// <param name="left">The first Slug</param>
    /// <param name="right">The first Slug</param>
    /// <returns>Whether both instances are equal</returns>
    public static bool operator ==(Slug<TFormatter> left, Slug<TFormatter> right) => left.Equals(right);

    /// <summary>
    /// Compares two Slug instances for inequality
    /// </summary>
    /// <param name="left">The first Slug</param>
    /// <param name="right">The first Slug</param>
    /// <returns>Whether both instances are not equal</returns>
    public static bool operator !=(Slug<TFormatter> left, Slug<TFormatter> right) => !left.Equals(right);

    #region Conversion
    
    /// <summary>
    /// Creates a new Slug for ISerializable
    /// </summary>
    private Slug(SerializationInfo info, StreamingContext context) : this(info.GetString(nameof(Value)) ?? "")
    {
    }
    
    /// <inheritdoc />
    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue(nameof(Value), Value, typeof(string));
    }
    
    #endregion
}