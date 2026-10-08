using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;
using SpawnDev.SpawnJS.JSObjects;

namespace Shiki.FileSystem.Implementations.Web.Stream;

/// <summary>
/// Worker service for opening, holding, and writing to open sync access handles
/// </summary>
/// <param name="fileManager">The file manager (for opening handles)</param>
[SupportedOSPlatform("browser")]
public class OPFSWorkerService(BrowserFileManager fileManager) : IOPFSWorkerService
{
    /// <summary>
    /// Handle storage
    /// </summary>
    private readonly OPFSHandleStorage _handles = new();
    
    /// <inheritdoc/>
    public async Task<int> OpenAsync(string path, FileMode mode)
    {
        if (mode == FileMode.CreateNew && await fileManager.ExistsAsync(path)) {
            throw new InvalidOperationException("File already exists");
        }
        
        bool c = mode is FileMode.Create or FileMode.CreateNew or FileMode.OpenOrCreate;
        FileSystemFileHandle? file = await fileManager.Get(path, c, c);

        if (file == null) throw new InvalidOperationException("Failed to open file");
        
        FileSystemSyncAccessHandle handle = await file.CreateSyncAccessHandle();

        if (mode is FileMode.Create or FileMode.Truncate)
        {
            handle.Truncate(0);
            handle.Flush();
        }
        
        return _handles.RegisterHandle(handle);
    }
    
    //I can marshal fucking tuple just fine but not custom class??????
    /// <inheritdoc/>
    public async Task<(byte[] Buffer, long Length)> ReadAsync(int id, long offset, int count)
    {
        FileSystemSyncAccessHandle handle = _handles.GetHandle(id);
        
        byte[] buffer = new byte[count];
        long read = handle.Read(buffer, new FileSystemSyncReadWriteOptions { At = offset });
        
        return (buffer, read);
    }
    
    /// <inheritdoc/>
    public Task<long> WriteAsync(int id, byte[] buffer, int offset)
    {
        FileSystemSyncAccessHandle handle = _handles.GetHandle(id);
        
        return Task.FromResult(handle.Write(buffer, new FileSystemSyncReadWriteOptions { At = offset }));
    }

    /// <inheritdoc/>
    public Task FlushAsync(int id)
    {
        FileSystemSyncAccessHandle handle = _handles.GetHandle(id);
        
        handle.Flush();

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task TruncateAsync(int id, long c)
    {
        FileSystemSyncAccessHandle handle = _handles.GetHandle(id);
        
        handle.Truncate(c);

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task CloseAsync(int id)
    {
        _handles.UnregisterHandle(id);

        return Task.CompletedTask;
    }
    
    /// <inheritdoc/>
    public Task<long> GetSizeAsync(int id)
    {
        FileSystemSyncAccessHandle handle = _handles.GetHandle(id);
        
        return Task.FromResult(handle.GetSize());
    }
}

/// <summary>
/// Stores open OPFS file handles
/// </summary>
[SupportedOSPlatform("browser")]
public class OPFSHandleStorage
{
    //I don't like this
    /// <summary>
    /// List of opened handles
    /// </summary>
    private readonly Dictionary<int, FileSystemSyncAccessHandle> _handles = [];
    
    /// <summary>
    /// Last handle id
    /// </summary>
    private int _lastHandleId = 0;

    /// <summary>
    /// Gets a handle from the opened handles
    /// </summary>
    /// <param name="id">The file handle id</param>
    /// <returns>The handle</returns>
    /// <exception cref="ObjectDisposedException">If the handle has been disposed</exception>
    public FileSystemSyncAccessHandle GetHandle(int id)
    {
        ObjectDisposedException.ThrowIf(!TryGetHandle(id, out FileSystemSyncAccessHandle? handle), id);
        
        return handle;
    }

    /// <summary>
    /// Tries to get a file handle from the opened handles
    /// </summary>
    /// <param name="id">The file handle id</param>
    /// <param name="handle">The handle</param>
    /// <returns>Whether the handle was found</returns>
    public bool TryGetHandle(int id, [NotNullWhen(true)] out FileSystemSyncAccessHandle? handle) => _handles.TryGetValue(id, out handle);

    /// <summary>
    /// Registers a new handle to the handles list
    /// </summary>
    /// <param name="handle">The handle to register</param>
    /// <returns>The id of the handle</returns>
    public int RegisterHandle(FileSystemSyncAccessHandle handle)
    {
        _handles[_lastHandleId] = handle;

        return _lastHandleId++;
    }

    /// <summary>
    /// Removes and frees the given handle from the handles list
    /// </summary>
    /// <param name="id">The file handle</param>
    /// <returns>Whether the handle was found in the opened handles list</returns>
    public bool UnregisterHandle(int id)
    {
        if (!_handles.Remove(id, out FileSystemSyncAccessHandle? handle))
            return false;
        
        handle?.Flush();
        handle?.Close();
        handle?.Dispose();

        return true;
    }
}