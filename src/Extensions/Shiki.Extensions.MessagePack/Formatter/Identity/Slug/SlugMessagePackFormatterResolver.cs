using MessagePack;
using MessagePack.Formatters;
using Shiki.Common.Identity.Slug;

namespace Shiki.Extensions.MessagePack.Formatter.Identity.Slug;

public sealed class SlugMessagePackFormatterResolver : IFormatterResolver
{
    public static readonly SlugMessagePackFormatterResolver Instance = new();

    public IMessagePackFormatter<T>? GetFormatter<T>() => Cache<T>.Formatter;

    private static class Cache<T>
    {
        public static readonly IMessagePackFormatter<T>? Formatter;

        static Cache()
        {
            Type type = typeof(T);
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Slug<>))
            {
                Type formatterType = typeof(SlugMessagePackFormatter<>).MakeGenericType(type.GetGenericArguments());
                Formatter = (IMessagePackFormatter<T>?)Activator.CreateInstance(formatterType);
            }
        }
    }
}