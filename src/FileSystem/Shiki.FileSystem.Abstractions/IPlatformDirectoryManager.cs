using System.Runtime.CompilerServices;

namespace Shiki.FileSystem.Abstractions;

/// <summary>
/// Interface for directory management
/// </summary>
public interface IPlatformDirectoryManager : IPlatformFileSystemNodeManager
{
    /// <summary>
    /// Creates a directory at the given path
    /// </summary>
    /// <param name="path">The path to create the directory at</param>
    public Task CreateDirectoryAsync(string path);

    /// <summary>
    /// Enumeration type
    /// </summary>
    public enum DirectoryEnumerationType
    {
        /// <summary>
        /// Enumerate all filesystem nodes
        /// </summary>
        All,
        /// <summary>
        /// Enumerate only files
        /// </summary>
        Files,
        /// <summary>
        /// Enumerate only directories
        /// </summary>
        Directories
    }
    
    /// <summary>
    /// Enumerates all nodes of the chosen type in the directory
    /// </summary>
    /// <param name="path">The directory path</param>
    /// <param name="glob">The glob to match every filename to</param>
    /// <param name="type">The type of filesystem node to enumerate</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>All nodes of the chosen type</returns>
    public IAsyncEnumerable<string> EnumerateAsync(string path, string glob = "*", DirectoryEnumerationType type = DirectoryEnumerationType.All, [EnumeratorCancellation] CancellationToken ct = default);

    /// <summary>
    /// Deletes a directory
    ///
    /// By default, does not delete if not empty when recursive is false
    /// </summary>
    /// <param name="path">The path of the directory to delete</param>
    /// <param name="recursive">Whether to delete the directory and all files and directories under it</param>
    public Task DeleteAsync(string path, bool recursive = false);
}