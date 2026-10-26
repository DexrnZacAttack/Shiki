using Shiki.Common.Result;

namespace Shiki.FileSystem.Storage.Serializer;

/// <summary>
/// Serializer for files, enforcing a file extension
///
/// This allows multiple serialization methods to coexist and use their own file extension when saving
/// </summary>
public interface IStorageFileSerializer : IStorageSerializer
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
/// Serializer for files, enforcing a file extension
///
/// This allows multiple serialization methods to coexist and use their own file extension when saving
/// </summary>
/// <typeparam name="T">The type being serialized</typeparam>
public interface IStorageFileSerializer<T> : IStorageFileSerializer, IStorageSerializer<T>;

/// <summary>
/// Storage type serializer
/// </summary>
public interface IStorageSerializer
{
    /// <summary>
    /// Deserializes a value of given type from the given stream
    /// </summary>
    /// <param name="type">The type to deserialize</param>
    /// <param name="stream">The stream</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>The deserialized value</returns>
    Task<Result<object?, Exception>> DeserializeAsync(Type type, Stream stream, CancellationToken ct = default);
    
    /// <summary>
    /// Serializes the given value into the given stream
    /// </summary>
    /// <param name="type">The type to serialize</param>
    /// <param name="stream">The stream</param>
    /// <param name="data">The value to serialize</param>
    /// <param name="ct">Cancellation token</param>
    Task<BooleanResult<Exception>> SerializeAsync(Type type, Stream stream, object? data, CancellationToken ct = default);
}

/// <summary>
/// Storage type serializer
/// </summary>
/// <typeparam name="T">The type being serialized</typeparam>
public interface IStorageSerializer<T> : IStorageSerializer
{
    /// <inheritdoc/>
    async Task<BooleanResult<Exception>> IStorageSerializer.SerializeAsync(Type type, Stream stream, object? data, CancellationToken ct) => data is T t ? await SerializeAsync(stream, t, ct) : new InvalidCastException(nameof(data));

    /// <inheritdoc/>
    async Task<Result<object?, Exception>> IStorageSerializer.DeserializeAsync(Type type, Stream stream, CancellationToken ct) => type.IsAssignableFrom(typeof(T)) ? await DeserializeAsync(type, stream, ct) : new InvalidCastException(nameof(type));

    /// <summary>
    /// Deserializes a value of given type from the given stream
    /// </summary>
    /// <param name="stream">The stream</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>The deserialized value</returns>
    Task<Result<T, Exception>> DeserializeAsync(Stream stream, CancellationToken ct = default);
    
    /// <summary>
    /// Serializes the given value into the given stream
    /// </summary>
    /// <param name="stream">The stream</param>
    /// <param name="data">The value to serialize</param>
    /// <param name="ct">Cancellation token</param>
    Task<BooleanResult<Exception>> SerializeAsync(Stream stream, T data, CancellationToken ct = default);
}