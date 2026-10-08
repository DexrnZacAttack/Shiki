using Shiki.Common.Extensions;
using Shiki.Common.Result;
using Shiki.FileSystem.Abstractions;
using Shiki.FileSystem.Storage.Serializer;

namespace Shiki.FileSystem.Storage;

/// <summary>
/// Storage for a given file
/// </summary>
/// <param name="directory">The directory containing the file (or null for current directory)</param>
/// <param name="filename">The name of the file to store in</param>
/// <param name="directoryManager">Directory manager</param>
/// <param name="fileManager">File manager</param>
/// <param name="serializer">Data serializer</param>
/// <typeparam name="T">The type being stored</typeparam>
public class FileStorage<T>(string? directory, string filename, IPlatformDirectoryManager directoryManager, IPlatformFileManager fileManager, IStorageSerializer<T> serializer) : IStorageWriter<T?, BooleanResult<Exception>>, IStorageReader<Result<T, Exception>>
where T : class
{
    /// <summary>
    /// The full filename including serializer dictated extension
    /// </summary>
    private string FullFilename => serializer is IStorageFileSerializer<T> s
                                       ? $"{System.IO.Path.GetFileNameWithoutExtension(filename)}.{s.FileExtension}"
                                       : filename;
    
    /// <summary>
    /// The full path of the file
    /// </summary>
    private string Path => System.IO.Path.Combine(directory ?? ".", FullFilename);

    /// <summary>
    /// Creates a FileStorage using an absolute path
    /// </summary>
    /// <param name="path">The absolute path of the file used for storage</param>
    /// <param name="directoryManager">Directory manager</param>
    /// <param name="fileManager">File manager</param>
    /// <param name="serializer">Data serializer</param>
    public FileStorage(string path, IPlatformDirectoryManager directoryManager, IPlatformFileManager fileManager, IStorageSerializer<T> serializer) :  this(System.IO.Path.GetDirectoryName(path), System.IO.Path.GetFileName(path), directoryManager, fileManager, serializer)
    {
    }

    /// <inheritdoc/>
    public async Task<Result<T, Exception>> LoadAsync()
    {
        if (!await fileManager.ExistsAsync(Path))
        {
            return new FileNotFoundException(Path);
        }

        await using Stream fs = await fileManager.GetStreamAsync(Path, FileMode.Open, FileAccess.Read, FileShare.Read);
        T? deserialized = await serializer.DeserializeAsync(fs);
        
        if (deserialized is null)
            return new NullReferenceException("Failed to deserialize");

        return deserialized;
    }

    /// <inheritdoc/>
    public async Task<BooleanResult<Exception>> SaveAsync(T? data)
    {
        if (data is null)
        {
            return new ArgumentNullException(nameof(data)).WithCurrentStackTrace();
        }
        
        if (directory != null)
        {
            try
            {
                if (!await directoryManager.ExistsAsync(directory))
                {
                    await directoryManager.CreateDirectoryAsync(directory);
                }
            }
            catch (Exception ex)
            {
                return ex;
            }
        }
        
        await using Stream fs = await fileManager.GetStreamAsync(Path, FileMode.Create, FileAccess.Write, FileShare.None);
        await serializer.SerializeAsync(fs, data);

        return BooleanResult<Exception>.Successful;
    }
}