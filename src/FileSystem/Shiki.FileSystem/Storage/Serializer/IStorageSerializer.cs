namespace Shiki.FileSystem.Storage.Serializer;

/// <summary>
/// Serializer for files, enforcing a file extension
///
/// This allows multiple serialization methods to coexist and use their own file extension when saving
/// </summary>
/// <typeparam name="T">The type being serialized</typeparam>
public interface IStorageFileSerializer<T> : IStorageSerializer<T>
{
    /// <summary>
    /// The file extension for this serializer
    /// </summary>
    string FileExtension { get; }
    /// <summary>
    /// The file extension search glob for this serializer
    /// </summary>
    string FileExtensionGlob =>  $"*.{FileExtension}";
}

/// <summary>
/// Storage type serializer
/// </summary>
/// <typeparam name="T">The type being serialized</typeparam>
public interface IStorageSerializer<T>
{
    /// <summary>
    /// Deserializes a value of given type from the given stream
    /// </summary>
    /// <param name="stream">The stream</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>The deserialized value</returns>
    Task<T?> DeserializeAsync(Stream stream, CancellationToken ct = default);
    
    /// <summary>
    /// Serializes the given value into the given stream
    /// </summary>
    /// <param name="stream">The stream</param>
    /// <param name="data">The value to serialize</param>
    /// <param name="ct">Cancellation token</param>
    Task SerializeAsync(Stream stream, T data, CancellationToken ct = default);
}