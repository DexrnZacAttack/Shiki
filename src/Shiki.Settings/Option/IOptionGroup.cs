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