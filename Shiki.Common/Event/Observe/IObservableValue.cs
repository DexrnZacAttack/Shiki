namespace Shiki.Common.Event.Observe;

/// <summary>
/// Interface to aid with our Observable implementation
/// </summary>
/// <typeparam name="T">The type to store</typeparam>
public interface IObservableValue<out T> : IObservable<T>
{
    /// <summary>
    /// The value
    /// </summary>
    public T Value
    {
        get;
    }
}