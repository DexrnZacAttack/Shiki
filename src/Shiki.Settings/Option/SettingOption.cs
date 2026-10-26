using R3;
using Shiki.Common.Identity;
using Shiki.Common.Result;
using Shiki.Internationalization.Component;

namespace Shiki.Settings.Option;

/// <summary>
/// Simple option implementation
///
/// This implementation is enough for most stored options
/// </summary>
/// <param name="id">The option ID</param>
/// <param name="category">The option category</param>
/// <param name="defaultValue">The default/initial value for this option</param>
/// <typeparam name="T">The option type</typeparam>
public class SettingOption<T>(Identifier id, Identifier category, T defaultValue) : IOption<T>
{
    /// <inheritdoc/>
    public Identifier Identifier { get; } = id;

    /// <inheritdoc/>
    public Identifier CategoryIdentifier { get; } = category;

    /// <inheritdoc/>
    public ITextComponent Name { get; init; } = new TranslatableTextComponent(id.Derived("name"));

    /// <inheritdoc/>
    public ITextComponent Description { get; init; } = new TranslatableTextComponent(id.Derived("description"));

    /// <inheritdoc/>
    public IBindableReactiveProperty<T> Property { get; } = new BindableReactiveProperty<T>(defaultValue);

    /// <summary>
    /// Gets/Acts on the property value directly
    /// </summary>
    public T Value
    {
        get => Property.Value;
        set => Property.Value = value;
    }

    /// <inheritdoc/>
    public BooleanResult<Exception> SetValue(T value)
    {
        Property.Value = value;
        return BooleanResult<Exception>.Successful;
    }
}