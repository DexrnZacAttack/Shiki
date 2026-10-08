using System.Runtime.CompilerServices;
using Shiki.FileSystem.Abstractions;

namespace Shiki.FileSystem.Implementations.Native;

/// <summary>
/// Native implementation for directory management
/// </summary>
public class NativeDirectoryManager : IPlatformDirectoryManager
{
    /// <inheritdoc/>
    public Task<bool> ExistsAsync(string path) => Task.FromResult(Directory.Exists(path));

    /// <inheritdoc/>
    public Task CreateDirectoryAsync(string path) => Task.FromResult(Directory.CreateDirectory(path));
    
    /// <inheritdoc/>
    public IAsyncEnumerable<string> EnumerateAsync(string path, string glob = "*", IPlatformDirectoryManager.DirectoryEnumerationType type = IPlatformDirectoryManager.DirectoryEnumerationType.All, CancellationToken ct = default)
    => type switch {
        IPlatformDirectoryManager.DirectoryEnumerationType.All         => Directory.EnumerateFileSystemEntries(path, glob).ToAsyncEnumerable(),
        IPlatformDirectoryManager.DirectoryEnumerationType.Files       => Directory.EnumerateFiles(path, glob).ToAsyncEnumerable(),
        IPlatformDirectoryManager.DirectoryEnumerationType.Directories => Directory.EnumerateDirectories(path, glob).ToAsyncEnumerable(),
        _                                                              => throw new ArgumentOutOfRangeException(nameof(type), type, null)
    };

    /// <inheritdoc/>
    public Task DeleteAsync(string path, bool recursive = false)
    {
        Directory.Delete(path, recursive);
        return Task.CompletedTask;
    }
}