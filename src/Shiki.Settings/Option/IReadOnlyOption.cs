using R3;
using Shiki.Common.Identity;
using Shiki.Internationalization.Component;

namespace Shiki.Settings.Option;

/// <summary>
/// A basic read-only option interface
/// </summary>
public interface IReadOnlyOption<out T> : IReadOnlyOption
{
    /// <inheritdoc/>
    object? IReadOnlyOption.GetValue() => GetValue();

    /// <summary>
    /// Gets the option value
    /// </summary>
    /// <returns>The option value</returns>
    new T GetValue();
}

/// <summary>
/// A basic untyped read-only option interface
/// </summary>
public interface IReadOnlyOption
{
    /// <summary>
    /// The option ID
    /// </summary>
    Identifier Identifier { get; }
    /// <summary>
    /// The option category
    /// </summary>
    Identifier CategoryIdentifier { get; }

    /// <summary>
    /// The option display name
    /// </summary>
    ITextComponent Name { get; }
    /// <summary>
    /// The option description
    /// </summary>
    ITextComponent Description { get; }

    /// <summary>
    /// An observable for when the option value changes
    /// </summary>
    Observable<Unit> ValueChanged { get; }

    /// <summary>
    /// The option type
    /// </summary>
    Type Type { get; }

    /// <summary>
    /// Gets the option value
    /// </summary>
    /// <returns>The option value</returns>
    object? GetValue();
}