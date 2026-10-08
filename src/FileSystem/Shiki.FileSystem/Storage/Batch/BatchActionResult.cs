namespace Shiki.FileSystem.Storage.Batch;

/// <summary>
/// Simple result interface containing a list of errors thrown by a batch of actions
///
/// It is common for things such as file operations to pass a string as TItem to act as a file path in the ActionErrors
/// </summary>
/// <typeparam name="TItem">The (usually simple) type being acted on</typeparam>
public interface IBatchActionResult<TItem>
{
    /// <summary>
    /// The errors
    /// </summary>
    List<ActionError<TItem>> Errors { get; }
    
    /// <summary>
    /// Whether there are any errors present
    /// </summary>
    bool HasErrors => Errors.Count > 0;
}

/// <summary>
/// Simple result type containing a list of errors thrown by a batch of actions
///
/// It is common for things such as file operations to pass a string as TItem to act as a file path in the ActionErrors
/// </summary>
/// <typeparam name="T">The type being acted on</typeparam>
/// <typeparam name="TItem">The (usually simple) type being acted on</typeparam>
public record BatchActionResult<T, TItem>(T Items, List<ActionError<TItem>> Errors) : BatchActionResult<TItem>(Errors);

/// <summary>
/// Simple result type containing a list of errors thrown by a batch of actions
///
/// It is common for things such as file operations to pass a string as TItem to act as a file path in the ActionErrors
/// </summary>
/// <typeparam name="TItem">The (usually simple) type being acted on</typeparam>
public record BatchActionResult<TItem>(List<ActionError<TItem>> Errors) : IBatchActionResult<TItem>;