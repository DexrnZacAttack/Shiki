using System.Runtime.Versioning;

namespace Shiki.FileSystem.Implementations.Web.Stream;

/// <summary>
/// OPFS worker service interface
/// </summary>
[SupportedOSPlatform("browser")]
public interface IOPFSWorkerService
{
    /// <summary>
    /// Opens a file and registers a handle
    /// </summary>
    /// <param name="path">The file path</param>
    /// <param name="mode">The file open mode</param>
    /// <returns>The file handle</returns>
    Task<int> OpenAsync(string path, FileMode mode);
    
    /// <summary>
    /// Reads a given amount of bytes from the file at the given offset
    /// </summary>
    /// <param name="id">The file handle</param>
    /// <param name="offset">The offset to read at</param>
    /// <param name="count">How many bytes to read</param>
    /// <returns>The read data</returns>
    Task<(byte[] Buffer, long Length)> ReadAsync(int id, long offset, int count);
    
    /// <summary>
    /// Writes the given data at the given offset
    /// </summary>
    /// <param name="id">The file handle</param>
    /// <param name="buffer">The data to write</param>
    /// <param name="offset">The offset to write at</param>
    /// <returns>The amount of successfully written data in bytes</returns>
    Task<long> WriteAsync(int id, byte[] buffer, int offset);

    /// <summary>
    /// Truncates the file to the given size
    /// </summary>
    /// <param name="id">The file handle</param>
    /// <param name="c">The new filesize</param>
    Task TruncateAsync(int id, long c);
    
    /// <summary>
    /// Flushes the file buffer
    /// </summary>
    /// <param name="id">The file handle</param>
    Task FlushAsync(int id);
    
    /// <summary>
    /// Gets the size of the file
    /// </summary>
    /// <param name="id">The file handle</param>
    /// <returns>The file size</returns>
    Task<long> GetSizeAsync(int id);
    
    /// <summary>
    /// Closes the file, freeing the handle.
    /// </summary>
    /// <param name="id">The file handle</param>
    Task CloseAsync(int id);
}