using R3;
using Shiki.Common.Result;

namespace Shiki.Settings.Option;

/// <summary>
/// A basic untyped option interface
/// </summary>
public interface IOption : IReadOnlyOption
{
    /// <summary>
    /// Sets the option value
    /// </summary>
    /// <param name="value">The value</param>
    /// <returns>The result returned when setting</returns>
    BooleanResult<Exception> SetValue(object? value);
}

/// <summary>
/// A basic option interface
/// </summary>
public interface IOption<T> : IOption, IReadOnlyOption<T>
{
    /// <inheritdoc/>
    Observable<Unit> IReadOnlyOption.ValueChanged => Property.AsObservable().Select(_ => Unit.Default).Skip(1);
    
    /// <summary>
    /// The reactive property
    /// </summary>
    IBindableReactiveProperty<T> Property { get; }

    /// <inheritdoc/>
    Type IReadOnlyOption.Type => typeof(T);

    /// <inheritdoc/>
    BooleanResult<Exception> IOption.SetValue(object? value) => value is T v ? SetValue(v) : new InvalidOperationException("Given value is not of type " + typeof(T));
    
    /// <summary>
    /// Sets the option value
    /// </summary>
    /// <param name="value">The value</param>
    /// <returns>The result returned when setting</returns>
    BooleanResult<Exception> SetValue(T value);
    
    /// <inheritdoc/>
    object? IReadOnlyOption.GetValue() => GetValue();
    /// <inheritdoc/>
    T IReadOnlyOption<T>.GetValue() => Property.Value;
}