using Shiki.FileSystem.Abstractions;

namespace Shiki.FileSystem.Implementations.Native;

/// <summary>
/// Native implementation for file management
/// </summary>
public class NativeFileManager : IPlatformFileManager
{
    public static NativeFileManager Instance { get; } = new();
    
    protected NativeFileManager() {}
    
    /// <inheritdoc/>
    public Task<bool> ExistsAsync(string path) => Task.FromResult(File.Exists(path));

    /// <inheritdoc/>
    public Task CopyAsync(string source, string destination, bool overwrite)
    {
        File.Copy(source, destination, overwrite);
        
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task MoveAsync(string from, string to, bool overwrite)
    {
        File.Move(from, to, overwrite);
        
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task DeleteAsync(string path)
    {
        File.Delete(path);
        
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task<Stream> GetStreamAsync(string path, FileMode mode, FileAccess access, FileShare share) => Task.FromResult<Stream>(File.Open(path, mode, access, share));
}