namespace Shiki.Common.Event;

/// <summary>
/// Defines an event handler with typed event args
/// </summary>
/// <typeparam name="TSender">The sender</typeparam>
/// <typeparam name="TEventArgs">The event args</typeparam>
public delegate void TypedEventHandler<in TSender, in TEventArgs>(TSender sender, TEventArgs eventArgs)
    where TEventArgs : EventArgs;
    
/// <summary>
/// Defines an async event handler with typed event args
/// </summary>
/// <typeparam name="TSender">The sender</typeparam>
/// <typeparam name="TEventArgs">The event args</typeparam>
public delegate Task TypedAsyncEventHandler<in TSender, in TEventArgs>(TSender sender, TEventArgs eventArgs)
    where TEventArgs : EventArgs;