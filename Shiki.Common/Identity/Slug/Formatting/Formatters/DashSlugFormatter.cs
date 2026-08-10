using System.Text.RegularExpressions;
using Unidecode.NET;

namespace Shiki.Common.Identity.Slug.Formatting.Formatters;

/// <summary>
/// Formats strings to contain only <c>a-z, 0-9, -</c>.
///
/// e.g. <c>Hello, world!</c> -> <c>hello-world</c>
/// </summary>
public abstract partial class DashSlugFormatter : ISlugFormatter
{
    // partly based on https://stackoverflow.com/a/19937132
    /// <inheritdoc/>
    public static string Format(string str)
    {
        str = str.ToLowerInvariant();                         //lowercase
        str = str.Unidecode();                                //decode unicode chars to regular ascii-compatible ones
        str = DelimiterRegex().Replace(str, "-");             //remove word delimiters like spaces
        str = InvalidCharactersRegex().Replace(str, "");      //remove invalid chars
        str = DelimiterDeduplicatorRegex().Replace(str, "-"); //remove duplicate dashes
        str = str.Trim('-');                                  //trim start and end dashes

        return str;
    }
    
    [GeneratedRegex(@"[^a-z0-9\-]", RegexOptions.Compiled)]
    private static partial Regex InvalidCharactersRegex();

    [GeneratedRegex(@"[\s_]", RegexOptions.Compiled)]
    private static partial Regex DelimiterRegex();

    [GeneratedRegex(@"-+", RegexOptions.Compiled)]
    private static partial Regex DelimiterDeduplicatorRegex();
}