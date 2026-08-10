using System.Diagnostics.CodeAnalysis;

namespace Shiki.Common.Extensions;

/// <summary>
/// Extensions for parsing with IParsable
/// </summary>
public static class ParsableExtensions
{
    extension(string s)
    {
        /// <summary>
        /// Parses a string into a class instance
        /// </summary>
        /// <param name="provider">The format provider</param>
        /// <typeparam name="T">The class to parse into</typeparam>
        /// <returns>The class instance</returns>
        public T ParseAs<T>(IFormatProvider? provider = null)
            where T : IParsable<T>? => T.Parse(s, provider);
        
        /// <summary>
        /// Attempts to parse a string into a class instance
        /// </summary>
        /// <param name="provider">The format provider</param>
        /// <param name="result">The class instance if successful</param>
        /// <typeparam name="T">The class to parse into</typeparam>
        /// <returns>Whether parsing was successful</returns>
        public bool TryParseAs<T>(IFormatProvider? provider, [MaybeNullWhen(false)] out T result)
            where T : IParsable<T>? => T.TryParse(s, provider, out result);
    }
}