using System.Collections.Immutable;
using Shiki.Settings.Option;

namespace Shiki.Settings.Category;

/// <summary>
/// A category node, containing the category, child categories, and all options associated with this category
/// </summary>
/// <param name="category">The category</param>
/// <param name="options">All options in this category</param>
/// <param name="children">Child categories</param>
public class SettingsCategoryNode(SettingsCategory category, ImmutableArray<IOption> options, ImmutableArray<SettingsCategoryNode> children)
{
    /// <summary>
    /// Builder for category nodes
    /// </summary>
    public static class Builder
    {
        /// <summary>
        /// Builds options into root SettingsCategoryNode trees
        /// </summary>
        /// <param name="options">The options</param>
        /// <param name="categories">The categories</param>
        /// <returns>The category node tree</returns>
        public static ImmutableArray<SettingsCategoryNode> Build(IEnumerable<IOption> options,
                                                                 IEnumerable<SettingsCategory> categories)
        {
            var c = categories.ToLookup(c => c.Parent);
            var o = options.ToLookup(o => o.CategoryIdentifier);

            return [.. c[null].Select(BuildNode)];
            
            SettingsCategoryNode BuildNode(SettingsCategory category)
            {
                return new SettingsCategoryNode(category, [.. o[category.Id]], [.. c[category.Id].Select(BuildNode)]);
            }
        }
    }
    
    /// <summary>
    /// The category for this node
    /// </summary>
    public SettingsCategory Category { get; } = category;

    /// <summary>
    /// The children of this node
    /// </summary>
    public ImmutableArray<SettingsCategoryNode> Children { get; } = children;
    /// <summary>
    /// The options in this node
    /// </summary>
    public ImmutableArray<IOption> Options { get; } = options;
}