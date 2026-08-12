using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Shiki.Common.Factory;
using Shiki.Common.Serialization.Converters;

namespace Shiki.Common.Identity;

/// <summary>
/// Represents a root namespace that can be derived from, ideally used as a constant
/// </summary>
[Serializable]
[DataContract]
[JsonConverter(typeof(StringJsonConverter<IdentifierNamespace>))]
[TypeConverter(typeof(StringTypeConverter<IdentifierNamespace>))]
[DebuggerDisplay("{Namespace}")]
public readonly partial struct IdentifierNamespace : ISerializable, IEquatable<IdentifierNamespace>
{
    /// <summary>
    /// The namespace
    /// </summary>
    public string Namespace { get; }

    /// <summary>
    /// Creates a new IdentifierNamespace
    /// </summary>
    /// <param name="nmsp">The namespace</param>
    /// <exception cref="ArgumentException">If the given namespace is empty, entirely whitespace, or null.</exception>
    [FactoryConstructable]
    public IdentifierNamespace(string nmsp)
    {
        if (string.IsNullOrWhiteSpace(nmsp))
            throw new ArgumentException("Namespace cannot be empty", nameof(nmsp));
        
        Namespace = nmsp;
    }

    /// <summary>
    /// Creates a new Identifier derived from this IdentifierNamespace
    /// </summary>
    /// <param name="path">The path</param>
    /// <returns>A new derived Identifier</returns>
    public Identifier Derived(params IReadOnlyCollection<string> path) => new(Namespace, path);
    
    /// <inheritdoc/>
    public override string ToString() => Namespace;
    
    /// <summary>
    /// "Converts" an IdentifierNamespace to a String
    /// </summary>
    /// <param name="nmsp">The IdentifierNamespace</param>
    /// <returns>The IdentifierNamespace's value</returns>
    public static explicit operator string(IdentifierNamespace nmsp) => nmsp.Namespace;
    
    /// <inheritdoc/>
    public bool Equals(IdentifierNamespace other) => Namespace == other.Namespace;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is IdentifierNamespace other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => Namespace.GetHashCode();

    /// <summary>
    /// Compares two IdentifierNamespace instances for equality
    /// </summary>
    /// <param name="left">The first IdentifierNamespace</param>
    /// <param name="right">The first IdentifierNamespace</param>
    /// <returns>Whether both instances are equal</returns>
    public static bool operator ==(IdentifierNamespace left, IdentifierNamespace right) => left.Equals(right);

    /// <summary>
    /// Compares two IdentifierNamespace instances for inequality
    /// </summary>
    /// <param name="left">The first IdentifierNamespace</param>
    /// <param name="right">The first IdentifierNamespace</param>
    /// <returns>Whether both instances are NOT equal</returns>
    public static bool operator !=(IdentifierNamespace left, IdentifierNamespace right) => !left.Equals(right);
    
    #region Conversion
    
    /// <summary>
    /// Creates a new IdentifierNamespace for ISerializable
    /// </summary>
    private IdentifierNamespace(SerializationInfo info, StreamingContext context) : this(info.GetString(nameof(Namespace)) ?? "")
    {
    }
    
    /// <inheritdoc />
    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue(nameof(Namespace), Namespace, typeof(string));
    }
    
    #endregion
}