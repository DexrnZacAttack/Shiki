using Shiki.Common.Identity;
using Shiki.Settings.Option;

namespace Shiki.Settings.Util;

/// <summary>
/// Simple struct to 'link' a group with its storage (by-id)
/// </summary>
/// <param name="Group">The group</param>
/// <param name="Storage">The storage</param>
internal readonly record struct OptionGroupMetadata(IOptionGroup Group, Identifier Storage);