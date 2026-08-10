using System.Text.RegularExpressions;
using Unidecode.NET;

namespace Shiki.Common.Identity.Slug.Formatting.Formatters;

/// <summary>
/// Formats strings to contain only <c>a-z, 0-9, _</c>.
///
/// e.g. <c>Hello, world!</c> -> <c>hello_world</c>
/// </summary>
public abstract partial class SnakeSlugFormatter : ISlugFormatter
{
    // partly based on https://stackoverflow.com/a/19937132
    /// <inheritdoc/>
    public static string Format(string str)
    {
        str = str.ToLowerInvariant(); //lowercase
        str = str.Unidecode(); //decode unicode chars to regular ascii-compatible ones
        str = DelimiterRegex().Replace(str, "_"); //remove word delimiters like spaces
        str = InvalidCharactersRegex().Replace(str, ""); //remove invalid chars
        str = DelimiterDeduplicatorRegex().Replace(str, "_"); //remove duplicate underscores
        str = str.Trim('_'); //trim start and end underscores

        return str;
    }
    
    [GeneratedRegex(@"[^a-z0-9_]", RegexOptions.Compiled)]
    private static partial Regex InvalidCharactersRegex();

    [GeneratedRegex(@"[\s-]+", RegexOptions.Compiled)]
    private static partial Regex DelimiterRegex();

    [GeneratedRegex(@"_+", RegexOptions.Compiled)]
    private static partial Regex DelimiterDeduplicatorRegex();
}
