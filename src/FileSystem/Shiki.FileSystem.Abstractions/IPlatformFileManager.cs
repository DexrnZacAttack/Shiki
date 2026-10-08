namespace Shiki.FileSystem.Abstractions;

/// <summary>
/// Interface for file management
/// </summary>
public interface IPlatformFileManager : IPlatformFileSystemNodeManager
{
    /// <summary>
    /// Copies a file to the given path
    /// </summary>
    /// <param name="source">The path of the source file to copy</param>
    /// <param name="destination">Where the source file will be copied to</param>
    /// <param name="overwrite">Whether an existing file at the destination path should be overwritten if required</param>
    public Task CopyAsync(string source, string destination, bool overwrite);
    
    /// <summary>
    /// Moves a file to the given path
    /// </summary>
    /// <param name="from">The path of the file to move</param>
    /// <param name="to">Where the file will be moved to</param>
    /// <param name="overwrite">Whether an existing file at the destination path should be overwritten if required</param>
    public Task MoveAsync(string from, string to, bool overwrite);

    /// <summary>
    /// Deletes a file at the given path
    /// </summary>
    /// <param name="path">The file path</param>
    Task DeleteAsync(string path);

    /// <summary>
    /// Opens a stream to access file data
    /// </summary>
    /// <param name="path">The file path</param>
    /// <param name="mode">The file open mode</param>
    /// <param name="access">The file access permissions</param>
    /// <param name="share">The file lock mode</param>
    /// <returns>The file data stream</returns>
    public Task<Stream> GetStreamAsync(string path, FileMode mode, FileAccess access, FileShare share);
}