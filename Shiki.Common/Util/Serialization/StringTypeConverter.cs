using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Shiki.Common.Util.Serialization;

/// <summary>
/// Basic string type converter for types that can be converted to and from a String
/// </summary>
/// <typeparam name="TConstructable">The constructable</typeparam>
public class StringTypeConverter<TConstructable> : TypeConverter
where TConstructable : IFactoryConstructable<TConstructable, string>
{
    /// <inheritdoc />
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType) =>
        sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);

    /// <inheritdoc />
    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is string str)
        {
            return TConstructable.CreateInstance(str);
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
        if (destinationType == typeof(string) && value is TConstructable c)
        {
            return c.ToString();
        }

        return base.ConvertTo(context, culture, value, destinationType);
    }
}