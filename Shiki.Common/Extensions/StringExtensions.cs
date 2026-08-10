using Shiki.Common.Identity.Slug;
using Shiki.Common.Identity.Slug.Formatting;

namespace Shiki.Common.Extensions;

/// <summary>
/// Extensions for System.String
/// </summary>
public static class StringExtensions
{
    extension(string str)
    {
        /// <summary>
        /// Creates a new Slug from the string
        /// </summary>
        /// <typeparam name="TFormatter">The formatter to use</typeparam>
        /// <returns>The new Slug</returns>
        public Slug<TFormatter> ToSlug<TFormatter>()
            where TFormatter : ISlugFormatter => new(str);
    }
}