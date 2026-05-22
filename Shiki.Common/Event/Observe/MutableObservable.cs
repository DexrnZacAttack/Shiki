using System.Diagnostics;

namespace Shiki.Common.Event.Observe;

/// <summary>
/// Class that notifies all observers when it's held value is changed
/// </summary>
/// <param name="initialValue">The initial value to set</param>
/// <typeparam name="T">The type to store</typeparam>
[DebuggerDisplay("Mutable Observable for {Value}")]
public class MutableObservable<T>(T initialValue) : Observable<T>(initialValue), IMutableObservableValue<T>
{
    /// <summary>
    /// The value
    /// </summary>
    public new T Value
    {
        get => base.Value;
        set => Set(value);
    }

    /// <inheritdoc/>
    public void Set(T value) => InternalSet(value);
}