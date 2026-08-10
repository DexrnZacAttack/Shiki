using System.Collections.Immutable;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Shiki.Common.Factory;
using Shiki.Common.Result;
using Shiki.Common.Serialization;
using Shiki.Common.Serialization.Converters;
using Shiki.Common.Util;

namespace Shiki.Common.Identity;

/// <summary>
/// A simple namespaced path type, used for preventing conflicts in modules, or other data driven instances
///
/// Similar in practice to Minecraft's Identifier/ResourceLocation
/// </summary>
[Serializable]
[DataContract]
[JsonConverter(typeof(StringJsonConverter<Identifier>))]
[TypeConverter(typeof(StringTypeConverter<Identifier>))]
[DebuggerDisplay("{IdentifierString}")]
public sealed partial record Identifier :
    ISerializable,
    IResultParsable<Identifier>,
    IFactoryConstructable<Identifier, string>,
    ICachedHashCode
{
    /// <summary>
    /// Namespace of the Identifier
    /// </summary>
    [property: DataMember(Order = 1)]
    public string Namespace { get; }
    /// <summary>
    /// Path of the Identifier
    /// </summary>
    [property: DataMember(Order = 2)]
    public ImmutableArray<string> Path { get; }

    /// <summary>
    /// Full Identifier in string form
    /// </summary>
    [IgnoreDataMember] public string IdentifierString { get; }
    /// <summary>
    /// Identifier Path in string form
    /// </summary>
    [IgnoreDataMember] public string PathString { get; }
    /// <inheritdoc/>
    [IgnoreDataMember] public int HashCode { get; }
    
    /// <summary>
    /// Creates a new Identifier
    /// </summary>
    /// <param name="nmsp">The namespace of the Identifier</param>
    /// <param name="path">The path of the Identifier</param>
    [FactoryConstructable]
    public Identifier(string nmsp, params IReadOnlyCollection<string> path)
    {
        Namespace = nmsp.Replace(":", "");
        Path = [.. path];

        if (string.IsNullOrEmpty(Namespace)) throw new ArgumentException("Namespace cannot be empty", nameof(nmsp));
        if (Path.IsDefaultOrEmpty) throw new ArgumentException("Path cannot be empty", nameof(path));

        this.PathString = string.Join('/', Path);
        this.IdentifierString = $"{Namespace}:{PathString}";

        this.HashCode = GenerateHashCode();
        
        Console.WriteLine(this.ToString());
    }

    /// <summary>
    /// Creates a new Identifier
    /// </summary>
    /// <param name="nmsp">The namespace of the Identifier</param>
    /// <param name="path">The path of the Identifier</param>
    [FactoryConstructable]
    public Identifier(string nmsp, string[] path) : this(nmsp, path.ToImmutableArray())
    {
    }

    /// <summary>
    /// Creates a new Identifier
    /// </summary>
    /// <param name="nmsp">The namespace of the Identifier</param>
    /// <param name="path">The string path of the Identifier, delimited with /</param>
    [FactoryConstructable]
    public Identifier(string nmsp, string path) : this(nmsp, path.Split('/', StringSplitOptions.RemoveEmptyEntries 
                                                                           | StringSplitOptions.TrimEntries).ToImmutableArray())
    {
    }
    
    #region Implementations
    /// <inheritdoc/>
    public int GenerateHashCode()
    {
        return IdentifierString.GetHashCode();
    }

    /// <inheritdoc />
    public override string ToString() => IdentifierString;

    /// <inheritdoc />
    public static Identifier CreateInstance(string id) => Identifier.TryParseIntoResult(id).ExpectDefault();

    /// <inheritdoc />
    public override int GetHashCode() => HashCode;

    /// <inheritdoc />
    public bool Equals(Identifier? other)
    {
        if (other is null) return false;
        if (other.HashCode != HashCode) return false;
        if (other.IdentifierString != IdentifierString) return false;

        return true;
    }

    /// <summary>
    /// Parses a string into a new Identifier
    /// </summary>
    /// <param name="id">The Identifier string, for example: <c>shiki:tile/dirt</c>, or <c>Shiki.Voxel:tile/dirt</c></param>
    /// <param name="provider"></param>
    public static Result<Identifier, Exception> TryParseIntoResult(string? id, IFormatProvider? provider = null)
    {
        if (string.IsNullOrEmpty(id))
            return new Result<Identifier, Exception>(new ArgumentException("Given string is null or empty",
                                                                           nameof(id)));

        string[] parts = id.Split(':');
        if (parts.Length < 2)
            return new
                Result<Identifier, Exception>(new FormatException("No namespace-path separator found in given string"));

        if (parts.Length > 2)
            return new
                Result<Identifier, Exception>(new
                                                  FormatException("Too many namespace-path separators found in given string"));

        if (string.IsNullOrEmpty(parts[0]))
            return new Result<Identifier, Exception>(new FormatException("Namespace part is null/empty"));

        if (string.IsNullOrEmpty(parts[1]))
            return new Result<Identifier, Exception>(new FormatException("Path is null/empty"));

        string nmsp = parts[0];
        string[] path = parts[1].Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (path.Length == 0)
            return new Result<Identifier, Exception>(new FormatException("Path is empty"));

        return new Result<Identifier, Exception>(new Identifier(nmsp, path));
    }
    #endregion

    /// <summary>
    /// Creates a new Identifier with the FullName of T as the Namespace
    /// </summary>
    /// <param name="path">The path of the Identifier</param>
    /// <typeparam name="T">The type to use for the Namespace</typeparam>
    /// <returns>The new Identifier</returns>
    public static Identifier WithTypeAsNamespace<T>(params string[] path)
        where T : class
        => new(typeof(T).FullName!, path);


    /// <summary>
    /// Creates a new Identifier with the FullName of T as the Namespace
    /// </summary>
    /// <param name="path">The string path of the Identifier</param>
    /// <typeparam name="T">The type to use for the Namespace</typeparam>
    /// <returns>The new Identifier</returns>
    public static Identifier WithTypeAsNamespace<T>(string path)
        where T : class
        => new(typeof(T).FullName!, path);


    /// <summary>
    /// Creates a new Identifier with the Namespace of T as the Identifier Namespace
    /// </summary>
    /// <param name="path">The path of the Identifier</param>
    /// <typeparam name="T">The type to use for the Namespace</typeparam>
    /// <returns>The new Identifier</returns>
    public static Identifier WithNamespaceOfType<T>(params string[] path)
        where T : class
        => new(typeof(T).Namespace!, path);


    /// <summary>
    /// Creates a new Identifier with the Namespace of T as the Identifier Namespace
    /// </summary>
    /// <param name="path">The string path of the Identifier</param>
    /// <typeparam name="T">The type to use for the Namespace</typeparam>
    /// <returns>The new Identifier</returns>
    public static Identifier WithNamespaceOfType<T>(string path)
        where T : class
        => new(typeof(T).Namespace!, path);
    
    /// <summary>
    /// Creates a new Identifier with a namespace and path derived from another Identifier
    /// </summary>
    /// <param name="id">The Identifier to derive from</param>
    /// <param name="path">The path to append</param>
    /// <returns>The new Identifier</returns>
    public static Identifier DerivedFrom(Identifier id, params IReadOnlyCollection<string> path) => id.Derived(path);
    
    /// <summary>
    /// Creates a new Identifier with a namespace and path derived from this Identifier
    /// </summary>
    /// <param name="path">The path to append</param>
    /// <returns>The new Identifier</returns>
    public Identifier Derived(params IReadOnlyCollection<string> path) => new(Namespace, Path.AddRange(path));
    
    /// <summary>
    /// Creates a new Identifier with a derived path and the given namespace
    /// </summary>
    /// <param name="nmsp">The namespace</param>
    /// <returns>The new Identifier</returns>
    public Identifier WithNamespace(string nmsp) => new(nmsp, Path);
    
    /// <summary>
    /// Creates a new Identifier with a given path and derived namespace
    /// </summary>
    /// <param name="path">The path</param>
    /// <returns>The new Identifier</returns>
    public Identifier WithPath(params ImmutableArray<string> path) => new(Namespace, path);

    #region Conversion

    /// <summary>
    /// Creates a new Identifier for ISerializable
    /// </summary>
    private Identifier(SerializationInfo info, StreamingContext context) : this(info.GetString(nameof(Namespace))!,
                                                                                    (ImmutableArray<string>)info
                                                                                       .GetValue(nameof(Path),
                                                                                            typeof(ImmutableArray<
                                                                                                string>))!)
    {
    }

    /// <inheritdoc />
    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue(nameof(Namespace), Namespace, typeof(string));
        info.AddValue(nameof(Path), Path, typeof(ImmutableArray<string>));
    }

    #endregion
}