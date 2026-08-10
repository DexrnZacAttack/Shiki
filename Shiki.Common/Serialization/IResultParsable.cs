using System.Diagnostics.CodeAnalysis;
using Shiki.Common.Result;

namespace Shiki.Common.Serialization;

#nullable enable

/// <summary>
/// Interface for parsing a string into an instance of TSelf
/// </summary>
/// <typeparam name="TSelf">The class that is implementing this interface</typeparam>
public interface IResultParsable<TSelf> : IParsable<TSelf> 
    where TSelf : IResultParsable<TSelf>?
{
    /// <inheritdoc/>
    static TSelf IParsable<TSelf>.Parse(string s, IFormatProvider? provider)
    {
        var r = TSelf.TryParseIntoResult(s, provider);
        if (r.HasValue)
        {
            return r.Value;
        }

        throw r.Error;
    }

    /// <inheritdoc/>
    static bool IParsable<TSelf>.TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider,
                                          [MaybeNullWhen(false)] out TSelf result)
    {
        var r = TSelf.TryParseIntoResult(s, provider);
        result = r.HasValue ? r.Value : default;

        return r.HasValue;
    }

    /// <summary>
    /// Parses a string into a Result, containing either the value, or an Exception if parsing failed.
    /// </summary>
    /// <param name="s">The string to parse</param>
    /// <param name="provider">The format provider</param>
    /// <returns>The result</returns>
    static abstract Result<TSelf, Exception> TryParseIntoResult(string? s, IFormatProvider? provider);
}