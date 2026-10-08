using System.Text.Json;

namespace Shiki.FileSystem.Storage.Serializer;

/// <summary>
/// (De)Serializes values in JSON
/// </summary>
/// <param name="defaultOptions">Default JSON options</param>
/// <typeparam name="T">The type being serialized</typeparam>
public class JsonStorageSerializer<T>(JsonSerializerOptions? defaultOptions = null) : IStorageFileSerializer<T>
{
    /// <inheritdoc/>
    public string FileExtension => "json";
    
    /// <inheritdoc/>
    public async Task<T?> DeserializeAsync(Stream stream, CancellationToken ct = default) => await JsonSerializer.DeserializeAsync<T>(stream, defaultOptions, ct);

    /// <summary>
    /// Deserializes a value of given type from the given stream, providing the given JsonSerializerOptions to JsonSerializer
    /// </summary>
    /// <param name="stream">The stream</param>
    /// <param name="options">The JSON options</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>The deserialized value</returns>
    public async Task<T?> DeserializeAsync(Stream stream, JsonSerializerOptions options, CancellationToken ct = default) => await JsonSerializer.DeserializeAsync<T>(stream, options, ct);

    /// <inheritdoc/>
    public async Task SerializeAsync(Stream stream, T data, CancellationToken ct = default) => await JsonSerializer.SerializeAsync(stream, data, defaultOptions, ct);

    /// <summary>
    /// Serializes the given value into the given stream, providing the given JsonSerializerOptions to JsonSerializer
    /// </summary>
    /// <param name="stream">The stream</param>
    /// <param name="data">The value to serialize</param>
    /// <param name="options">The JSON options</param>
    /// <param name="ct">Cancellation token</param>
    public async Task SerializeAsync(Stream stream, T data, JsonSerializerOptions options, CancellationToken ct = default) => await JsonSerializer.SerializeAsync(stream, data, options, ct);
}