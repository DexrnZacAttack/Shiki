namespace Shiki.FileSystem.Abstractions;

/// <summary>
/// Interface for simple nonspecific filesystem node operations
/// </summary>
public interface IPlatformFileSystemNodeManager
{
    /// <summary>
    /// Checks whether a filesystem node exists
    /// </summary>
    /// <param name="path">The path to look for</param>
    /// <returns>Whether there is a node at the given path</returns>
    public Task<bool> ExistsAsync(string path);
}