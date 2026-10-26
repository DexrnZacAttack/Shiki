using System.Runtime.Serialization;
using System.Text.Json;
using Shiki.Common.Result;

namespace Shiki.FileSystem.Storage.Serializer;

/// <summary>
/// (De)Serializes values in JSON
/// </summary>
/// <param name="defaultOptions">Default JSON options</param>
public class JsonStorageSerializer(JsonSerializerOptions? defaultOptions = null) : IStorageFileSerializer
{
    /// <inheritdoc/>
    public string FileExtension => "json";

    public async Task<Result<object?, Exception>> DeserializeAsync(Type type, Stream stream, CancellationToken ct = default)
    {
        try
        {
            return await JsonSerializer.DeserializeAsync(stream, type, defaultOptions, ct);
        }
        catch (Exception e)
        {
            return e;
        }
    }

    public async Task<Result<object?, Exception>> DeserializeAsync(Type type, Stream stream, JsonSerializerOptions options, CancellationToken ct = default) {
        try
        {
            return await JsonSerializer.DeserializeAsync(stream, type, options, ct);
        }
        catch (Exception e)
        {
            return e;
        }
    }
    
    public async Task<BooleanResult<Exception>> SerializeAsync(Type type, Stream stream, object? data, CancellationToken ct = default) {
        try
        {
            await JsonSerializer.SerializeAsync(stream, data, type, defaultOptions, ct);
            return BooleanResult<Exception>.Successful;
        }
        catch (Exception e)
        {
            return e;
        }
    }

    public async Task<BooleanResult<Exception>> SerializeAsync(Type type, Stream stream, object? data,
                                                               JsonSerializerOptions options,
                                                               CancellationToken ct = default)
    {
        try
        {
            await JsonSerializer.SerializeAsync(stream, data, type, options, ct);
            return BooleanResult<Exception>.Successful;
        }
        catch (Exception e)
        {
            return e;
        }
    }
}

/// <summary>
/// (De)Serializes values in JSON
/// </summary>
/// <param name="defaultOptions">Default JSON options</param>
/// <typeparam name="T">The type being serialized</typeparam>
public class JsonStorageSerializer<T>(JsonSerializerOptions? defaultOptions = null) : JsonStorageSerializer, IStorageFileSerializer<T>
{
    /// <inheritdoc/>
    public async Task<Result<T, Exception>> DeserializeAsync(Stream stream, CancellationToken ct = default)
    {
        try
        {
           return await JsonSerializer.DeserializeAsync<T>(stream, defaultOptions, ct);
        }
        catch (Exception e)
        {
            return e;
        }
    }

    /// <summary>
    /// Deserializes a value of given type from the given stream, providing the given JsonSerializerOptions to JsonSerializer
    /// </summary>
    /// <param name="stream">The stream</param>
    /// <param name="options">The JSON options</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>The deserialized value</returns>
    public async Task<Result<T, Exception>> DeserializeAsync(Stream stream, JsonSerializerOptions options, CancellationToken ct = default) {
        try
        {
            return await JsonSerializer.DeserializeAsync<T>(stream, options, ct);
        }
        catch (Exception e)
        {
            return e;
        }
    }

    /// <inheritdoc/>
    public async Task<BooleanResult<Exception>> SerializeAsync(Stream stream, T data, CancellationToken ct = default)  {
        try
        {
            await JsonSerializer.SerializeAsync(stream, data, defaultOptions, ct);
            return BooleanResult<Exception>.Successful;
        }
        catch (Exception e)
        {
            return e;
        }
    }

    /// <summary>
    /// Serializes the given value into the given stream, providing the given JsonSerializerOptions to JsonSerializer
    /// </summary>
    /// <param name="stream">The stream</param>
    /// <param name="data">The value to serialize</param>
    /// <param name="options">The JSON options</param>
    /// <param name="ct">Cancellation token</param>
    public async Task<BooleanResult<Exception>> SerializeAsync(Stream stream, T data, JsonSerializerOptions options, CancellationToken ct = default)  {
        try
        {
            await JsonSerializer.SerializeAsync(stream, data, options, ct);
            return BooleanResult<Exception>.Successful;
        }
        catch (Exception e)
        {
            return e;
        }
    }
}