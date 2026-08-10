namespace Shiki.Common.Identity.Slug.Formatting;

/// <summary>
/// Sanitizes the string into a valid Slug
/// </summary>
/// <param name="str">The string</param>
/// <returns>The sanitized string</returns>
public delegate string SlugFormatter(string str);

/// <summary>
/// An interface that allows for formatting a string into a slug. 
/// </summary>
public interface ISlugFormatter
{
    /// <summary>
    /// Sanitizes the string into a valid Slug
    /// </summary>
    /// <param name="str">The string</param>
    /// <returns>The sanitized string</returns>
    static abstract string Format(string str);
}