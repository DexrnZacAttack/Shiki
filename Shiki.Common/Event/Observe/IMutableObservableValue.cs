namespace Shiki.Common.Event.Observe;

/// <summary>
/// Interface to aid with our Observable implementation, specifically to allow for a mutable observable.
/// </summary>
/// <typeparam name="T">The type to store</typeparam>
public interface IMutableObservableValue<T> : IObservableValue<T>
{
    /// <summary>
    /// The value
    /// </summary>
    new T Value
    {
        get;
        set;
    }

    /// <summary>
    /// Sets the current value with a new provided value and notifies all observers
    /// </summary>
    /// <param name="value">The new value</param>
    public void Set(T value);
}