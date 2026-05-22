namespace Shiki.Common.Event.Observe;

/// <summary>
/// Generates an accessible publicly immutable Observable property without having to duplicate the field manually
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class PubliclyImmutableObservableAttribute : Attribute;