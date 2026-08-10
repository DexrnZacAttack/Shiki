using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Shiki.Common.Serialization.Converters;

namespace Shiki.Common.Identity.Slug.Serialization.Converters;

/// <summary>
/// Converts a slug
/// </summary>
public class SlugTypeConverterFactory : TypeConverter
{
    /// <summary>
    /// The inner typeconverter
    /// </summary>
    private readonly TypeConverter _inner;

    /// <summary>
    /// Creates a new factory instance with the given inner typeconverter Type 
    /// </summary>
    /// <param name="type">The converter type</param>
    public SlugTypeConverterFactory(Type type)
    {
        Type g = typeof(StringTypeConverter<>).MakeGenericType(type);
        _inner = (TypeConverter)Activator.CreateInstance(g)!;
    }
    
    /// <inheritdoc />
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType) => _inner.CanConvertFrom(context, sourceType);

    /// <inheritdoc />
    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value) => _inner.ConvertFrom(context, culture, value);

    /// <inheritdoc />
    public override bool CanConvertTo(ITypeDescriptorContext? context, [NotNullWhen(true)] Type? destinationType) => _inner.CanConvertTo(context, destinationType);

    /// <inheritdoc />
    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType) => _inner.ConvertTo(context, culture, value, destinationType);
}