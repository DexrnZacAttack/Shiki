namespace Shiki.FileSystem.Storage.Batch;

/// <summary>
/// A simple error type containing what is present of the item and an exception explaining why the action failed
///
/// It is common for things such as file operations to pass a string as TItem to act as a file path
/// </summary>
/// <param name="Item">The item being acted on</param>
/// <param name="Exception">An exception</param>
/// <typeparam name="TItem">The type of the item being acted on</typeparam>
public readonly record struct ActionError<TItem>(TItem Item, Exception Exception);