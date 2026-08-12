using MessagePack;
using MessagePack.Formatters;
using Shiki.Common.Identity.Slug;

namespace Shiki.Extensions.MessagePack.Formatter.Identity.Slug;

//todo do I still need it
public sealed class SlugFormatterResolver : IFormatterResolver
{
    public static readonly SlugFormatterResolver Instance = new();

    public IMessagePackFormatter<T>? GetFormatter<T>() => Cache<T>.Formatter;

    private static class Cache<T>
    {
        public static readonly IMessagePackFormatter<T>? Formatter;

        static Cache()
        {
            Type type = typeof(T);
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Slug<>))
            {
                Type formatterType = typeof(SlugFormatter<>).MakeGenericType(type.GetGenericArguments());
                Formatter = (IMessagePackFormatter<T>?)Activator.CreateInstance(formatterType);
            }
        }
    }
}