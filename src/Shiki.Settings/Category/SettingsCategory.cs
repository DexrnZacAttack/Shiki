using Shiki.Common.Identity;
using Shiki.Internationalization.Component;

namespace Shiki.Settings.Category;

/// <summary>
/// A settings category
/// </summary>
/// <param name="id">The category id</param>
/// <param name="name">The category name</param>
public class SettingsCategory(Identifier id, ITextComponent name)
{
    /// <summary>
    /// The category ID
    /// </summary>
    public Identifier Id { get; } = id;
    /// <summary>
    /// The category name
    /// </summary>
    public ITextComponent Name { get; } = name;

    /// <summary>
    /// The category parent ID (if any)
    /// </summary>
    public Identifier? Parent { get; init; }
}