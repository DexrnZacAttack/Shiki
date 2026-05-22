using System.Diagnostics;

namespace Shiki.Common.Event.Observe;

/// <summary>
/// Class that notifies all observers when it's held value is changed
///
/// Used for PubliclyImmutableObservableAttribute
/// </summary>
/// <param name="initialValue">The initial value to set</param>
/// <typeparam name="T">The type to store</typeparam>
[DebuggerDisplay("Observable for {Value}")]
public class Observable<T>(T initialValue) : IObservableValue<T>
{
    /// <summary>
    /// The value
    /// </summary>
    public T Value { get; private set; } = initialValue;

    /// <summary>
    /// The observers
    /// </summary>
    private readonly List<Subscriber> _subscribers = [];
    /// <summary>
    /// A lock for the observers list
    /// </summary>
    private readonly Lock _lock = new();

    /// <inheritdoc/>
    public IDisposable Subscribe(IObserver<T> observer)
    {
        lock (_lock)
        {
            SynchronizationContext? syncContext = SynchronizationContext.Current;
            Subscriber subscriber = new Subscriber(observer, syncContext);
            
            _subscribers.Add(subscriber);
            return new Unsubscriber(_subscribers, _lock, subscriber);
        }
    }
    
    /// <summary>
    /// Notifies all observers with the cloned state 
    /// </summary>
    protected void NotifyAll()
    {
        List<Subscriber> copy;
        lock (_lock)
        {
            copy = _subscribers.ToList();
        }

        T v = Value;
        copy.ForEach(s => s.Notify(v));
    }
    
    /// <summary>
    /// Sets the value directly
    /// </summary>
    /// <param name="value">The new value</param>
    protected void InternalSet(T value)
    {
        if (!EqualityComparer<T>.Default.Equals(Value, value))
        {
            Value = value;
            NotifyAll();
        }
    }

    /// <inheritdoc/>
    public override string ToString() => Value?.ToString() ?? nameof(Value);
    
    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return obj switch
        {
            Observable<T> other =>  EqualityComparer<T>.Default.Equals(Value, other.Value),
            T             other =>  EqualityComparer<T>.Default.Equals(Value, other),
            _                   => false
        };
    }
    
    /// <inheritdoc/>
    public override int GetHashCode()
        => Value.GetHashCode();

    /// <summary>
    /// Compares one Observable's value with another Observable's value for equality
    /// </summary>
    /// <param name="lhs">The first Observable</param>
    /// <param name="rhs">The second Observable</param>
    /// <returns>true if both values are NOT equal</returns>
    public static bool operator ==(Observable<T>? lhs, Observable<T>? rhs)
    {
        if (ReferenceEquals(lhs, rhs))
            return true;

        if (lhs is null || rhs is null)
            return false;

        return EqualityComparer<T>.Default.Equals(lhs.Value, rhs.Value);
    }

    /// <summary>
    /// Compares one Observable's value with another Observable's value for inequality
    /// </summary>
    /// <param name="lhs">The first Observable</param>
    /// <param name="rhs">The second Observable</param>
    /// <returns>true if both values are NOT equal</returns>
    public static bool operator !=(Observable<T>? lhs, Observable<T>? rhs)
        => !(lhs == rhs);

    /// <summary>
    /// Compares one Observable with an observable's value type for equality
    /// </summary>
    /// <param name="lhs">The Observable</param>
    /// <param name="rhs">The value</param>
    /// <returns>true if both are equal</returns>
    public static bool operator ==(Observable<T>? lhs, T? rhs)
    {
        if (lhs is null)
            return rhs is null;

        return EqualityComparer<T>.Default.Equals(lhs.Value, rhs);
    }

    /// <summary>
    /// Compares one Observable with an observable's value type for inequality
    /// </summary>
    /// <param name="lhs">The Observable</param>
    /// <param name="rhs">The value</param>
    /// <returns>true if both are NOT equal</returns>
    public static bool operator !=(Observable<T>? lhs, T? rhs)
        => !(lhs == rhs);

    /// <summary>
    /// Compares one Observable with an observable's value type for equality
    /// </summary>
    /// <param name="lhs">The value</param>
    /// <param name="rhs">The Observable</param>
    /// <returns>true if both are equal</returns>
    public static bool operator ==(T? lhs, Observable<T>? rhs)
    {
        if (rhs is null)
            return lhs is null;

        return EqualityComparer<T>.Default.Equals(lhs, rhs.Value);
    }
    
    /// <summary>
    /// Compares one Observable with an observable's value type for inequality
    /// </summary>
    /// <param name="lhs">The value</param>
    /// <param name="rhs">The Observable</param>
    /// <returns>true if both are NOT equal</returns>
    public static bool operator !=(T? lhs, Observable<T>? rhs)
        => !(lhs == rhs);

    /// <summary>
    /// A wrapper around IObserver for cleanly notifying while abiding by any synchronization contexts
    /// </summary>
    /// <param name="observer">The observer</param>
    /// <param name="syncContext">The sync context</param>
    protected class Subscriber(IObserver<T> observer, SynchronizationContext? syncContext = default)
    {
        /// <summary>
        /// The observer
        /// </summary>
        public IObserver<T> Observer { get; } = observer;

        /// <summary>
        /// Notifies the Subscriber
        /// </summary>
        /// <param name="value">The value to notify with</param>
        public void Notify(T value)
        {
            //if were within some context we need to abide
            if (syncContext != null)
            {
                //post within context
                syncContext.Post(_ => Observer.OnNext(value), null);
                return;
            }
            
            //notify
            Task.Run(() => Observer.OnNext(value));
        }
    }
    
    /// <summary>
    /// Provides a way to unsubscribe from an Observable by disposing the instance of this class provided by Observable.Subscribe
    /// </summary>
    /// <param name="subscribers">List of subscribers in the Observable</param>
    /// <param name="lck">The lock on the subscribers list, so that it won't incorrectly modify concurrently</param>
    /// <param name="subscriber">The subscriber to unsubscribe at the end</param>
    protected class Unsubscriber(List<Subscriber> subscribers, Lock lck, Subscriber subscriber) : IDisposable
    {
        /// <summary>
        /// The subscribers from the Observable
        /// </summary>
        private readonly List<Subscriber> _subscribers = subscribers;
        /// <summary>
        /// The lock from the Observable
        /// </summary>
        private readonly Lock _lock = lck;
        
        /// <summary>
        /// The subscriber
        /// </summary>
        private readonly Subscriber _subscriber = subscriber;
        
        /// <inheritdoc/>
        public void Dispose()
        {
            lock (_lock)
            {
                _subscribers.Remove(_subscriber);
            }
        }
    }
}