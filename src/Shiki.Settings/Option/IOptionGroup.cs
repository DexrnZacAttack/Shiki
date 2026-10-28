using System.Text.Json.Serialization;
using Shiki.Common.Identity;

namespace Shiki.Settings.Option;

/// <summary>
/// Interface for a group of options
///
/// This is supposed to be implemented by option holder classes for indexing and storage.
/// </summary>
public interface IOptionGroup
{
    /// <summary>
    /// Gets all options in this group
    /// </summary>
    /// <returns>The options in this group</returns>
    IEnumerable<IOption> GetOptions();
}

/// <summary>
/// Simple option group interface that adds a forced Identifier to identify the group
/// </summary>
public interface IIdentifiableOptionGroup : IOptionGroup
{
    /// <summary>
    /// Constant group identifier
    /// </summary>
    static abstract Identifier Identifier { get; }
}