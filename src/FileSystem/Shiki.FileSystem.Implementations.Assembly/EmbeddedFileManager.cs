using Shiki.FileSystem.Abstractions;

namespace Shiki.FileSystem.Implementations.Assembly;

/// <summary>
/// Assembly 'Embedded' read-only implementation for file management
/// </summary>
public class EmbeddedFileManager(System.Reflection.Assembly assembly) : EmbeddedFileSystemNodeManager(assembly), IPlatformFileManager
{
    /// <summary>
    /// Gets the EmbeddedFileManager for the currently executing assembly
    /// </summary>
    public static EmbeddedFileManager OfCurrentlyExecuting { get; } = new(System.Reflection.Assembly.GetExecutingAssembly());
    
    /// <inheritdoc/>
    public Task CopyAsync(string source, string destination, bool overwrite) =>
        Task.FromException(Constants.CannotModifyException);

    /// <inheritdoc/>
    public Task MoveAsync(string from, string to, bool overwrite) =>
        Task.FromException(Constants.CannotModifyException);

    /// <inheritdoc/>
    public Task DeleteAsync(string path) => Task.FromException(Constants.CannotModifyException);
    
    /// <inheritdoc/>
    public async Task<Stream> GetStreamAsync(string path, FileMode mode, FileAccess access, FileShare share) => assembly.GetManifestResourceStream(await GetPath(path)) ?? throw new FileNotFoundException();
}