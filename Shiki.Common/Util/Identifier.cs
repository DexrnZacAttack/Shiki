using System.Collections.Immutable;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Shiki.Common.Util;

/// <summary>
/// A simple namespaced path type, used for preventing conflicts in modules, or other data driven instances
///
/// Similar in practice to Minecraft's Identifier/ResourceLocation
/// </summary>
[Serializable]
[DataContract]
[JsonConverter(typeof(JsonConverter))]
[TypeConverter(typeof(TypeConverter))]
[DebuggerDisplay("{FullPath}")]
public sealed record Identifier : ISerializable
{
    /// <summary>
    /// Namespace of the Identifier
    /// </summary>
    [property: DataMember(Order = 1)] public string Namespace { get; }

    /// <summary>
    /// Path of the Identifier
    /// </summary>
    [property: DataMember(Order = 2)] public ImmutableArray<string> Path { get; }

    /// <summary>
    /// Full Identifier in string form
    /// </summary>
    [IgnoreDataMember] public string FullPath => $"{Namespace}:{PathString}";

    /// <summary>
    /// Identifier Path in string form
    /// </summary>
    [IgnoreDataMember] public string PathString => string.Join('/', Path);

    /// <inheritdoc />
    public override string ToString() => FullPath;

    /// <inheritdoc />
    public override int GetHashCode()
    {
        HashCode hc = new();
        hc.Add(Namespace);

        if (!Path.IsDefault)
        {
            foreach (string part in Path)
            {
                hc.Add(part);
            }
        }

        return hc.ToHashCode();
    }

    /// <inheritdoc />
    public bool Equals(Identifier? other)
    {
        if (other is null) return false;

        if (EqualityContract != other.EqualityContract) return false;
        if (Namespace        != other.Namespace) return false;
        
        if (Path.IsDefault   && other.Path.IsDefault) return true;
        if (Path.IsDefault   || other.Path.IsDefault) return false;

        return Path.SequenceEqual(other.Path);
    }

    /// <summary>
    /// Creates a new Identifier
    /// </summary>
    /// <param name="nmsp">The namespace of the Identifier</param>
    /// <param name="path">The path of the Identifier</param>
    public Identifier(string nmsp, params ImmutableArray<string> path)
    {
        Namespace = nmsp.Replace(":", "");
        Path = path;
    }

    /// <summary>
    /// Creates a new Identifier
    /// </summary>
    /// <param name="nmsp">The namespace of the Identifier</param>
    /// <param name="path">The path of the Identifier</param>
    public Identifier(string nmsp, string[] path) : this(nmsp, path.ToImmutableArray())
    {
    }

    /// <summary>
    /// Creates a new Identifier
    /// </summary>
    /// <param name="nmsp">The namespace of the Identifier</param>
    /// <param name="path">The string path of the Identifier, delimited with /</param>
    public Identifier(string nmsp, string path) : this(nmsp,
                                                       path.Split('/', StringSplitOptions.RemoveEmptyEntries |
                                                                       StringSplitOptions.TrimEntries))
    {
    }

    /// <summary>
    /// Parses a string into a new Identifier
    /// </summary>
    /// <param name="id">The Identifier string, for example: <c>shiki:tile/dirt</c>, or <c>Shiki.Voxel:tile/dirt</c></param>
    /// <returns>The new Identifier</returns>
    /// <exception cref="ArgumentException">If the given string is an invalid Identifier</exception>
    public static Identifier FromString(string id)
    {
        string[] parts = id.Split(':', 2);
        if (parts.Length > 1)
        {
            string nmsp = parts[0];
            string[] path = parts[1].Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            return new Identifier(nmsp, path);
        }

        throw new ArgumentException("Invalid identifier string");
    }

    /// <summary>
    /// Creates a new Identifier with the FullName of T as the Namespace
    /// </summary>
    /// <param name="path">The path of the Identifier</param>
    /// <typeparam name="T">The type to use for the Namespace</typeparam>
    /// <returns>The new Identifier</returns>
    public static Identifier WithTypeAsNamespace<T>(params string[] path)
        where T : class
    {
        return new Identifier(typeof(T).FullName!, path);
    }

    /// <summary>
    /// Creates a new Identifier with the FullName of T as the Namespace
    /// </summary>
    /// <param name="path">The string path of the Identifier</param>
    /// <typeparam name="T">The type to use for the Namespace</typeparam>
    /// <returns>The new Identifier</returns>
    public static Identifier WithTypeAsNamespace<T>(string path)
        where T : class
    {
        return new Identifier(typeof(T).FullName!, path);
    }

    /// <summary>
    /// Creates a new Identifier with the Namespace of T as the Identifier Namespace
    /// </summary>
    /// <param name="path">The path of the Identifier</param>
    /// <typeparam name="T">The type to use for the Namespace</typeparam>
    /// <returns>The new Identifier</returns>
    public static Identifier WithNamespaceOfType<T>(params string[] path)
        where T : class
    {
        return new Identifier(typeof(T).Namespace!, path);
    }

    /// <summary>
    /// Creates a new Identifier with the Namespace of T as the Identifier Namespace
    /// </summary>
    /// <param name="path">The string path of the Identifier</param>
    /// <typeparam name="T">The type to use for the Namespace</typeparam>
    /// <returns>The new Identifier</returns>
    public static Identifier WithNamespaceOfType<T>(string path)
        where T : class
    {
        return new Identifier(typeof(T).Namespace!, path);
    }

#region Conversion
    /// <summary>
    /// Creates a new Identifier for ISerializable
    /// </summary>
    private Identifier(SerializationInfo info, StreamingContext context) : this(info.GetString(nameof(Namespace))!,
             (ImmutableArray<string>)info.GetValue(nameof(Path), typeof(ImmutableArray<string>))!)
    {
    }

    /// <inheritdoc />
    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue(nameof(Namespace), Namespace, typeof(string));
        info.AddValue(nameof(Path), Path, typeof(ImmutableArray<string>));
    }

    /// <summary>
    /// JSON Converter for Identifier
    /// </summary>
    public class JsonConverter : JsonConverter<Identifier>
    {
        /// <inheritdoc />
        public override Identifier? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? id = reader.GetString();
            if (id == null)
            {
                return null;
            }

            return FromString(id);
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, Identifier value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }

    /// <summary>
    /// TypeConverter for Identifier
    /// </summary>
    public class TypeConverter : System.ComponentModel.TypeConverter
    {
        /// <inheritdoc />
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType) =>
            sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);

        /// <inheritdoc />
        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            if (value is string str)
            {
                return FromString(str);
            }

            return base.ConvertFrom(context, culture, value);
        }

        /// <inheritdoc />
        public override bool CanConvertTo(ITypeDescriptorContext? context, [NotNullWhen(true)] Type? destinationType) =>
            destinationType == typeof(string) || base.CanConvertTo(context, destinationType);

        /// <inheritdoc />
        public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value,
                                          Type destinationType)
        {
            if (destinationType == typeof(string) && value is Identifier id)
            {
                return id.ToString();
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
#endregion
}