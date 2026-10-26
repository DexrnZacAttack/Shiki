namespace Shiki.FileSystem.Storage;

/// <summary>
/// Interface for implementation that can save/write the given type parameter to storage, returning a value when complete
///
/// This is usually used to return some sort of error/success value instead of throwing an exception
/// </summary>
/// <typeparam name="T">The type being stored</typeparam>
/// <typeparam name="TReturn">The type to return when saving is complete</typeparam>
public interface IStorageWriter<in T, TReturn> : IStorageWriter<T>
{
    /// <inheritdoc />
    Task IStorageWriter<T>.SaveAsync(T data, CancellationToken ct) => SaveAsync(data, ct);

    /// <summary>
    /// Serializes the given value into storage
    /// </summary>
    /// <param name="data">The value to serialize</param>
    /// <param name="ct">A cancellation token</param>
    /// <returns>A return value of type from typeparam provided to the interface</returns>
    new Task<TReturn> SaveAsync(T data, CancellationToken ct = default);
}

/// <summary>
/// Interface for implementation that can save/write a value
/// </summary>
public interface IStorageWriter
{
    /// <summary>
    /// Serializes the given value into storage
    /// </summary>
    /// <param name="data">The value to serialize</param>
    /// <param name="ct">A cancellation token</param>
    Task SaveAsync(object? data, CancellationToken ct = default);
}

/// <summary>
/// Interface for implementation that can save/write the given type parameter to storage
/// </summary>
/// <typeparam name="T">The type being stored</typeparam>
public interface IStorageWriter<in T> : IStorageWriter
{
    /// <summary>
    /// Serializes the given value into storage
    /// </summary>
    /// <param name="data">The value to serialize</param>
    /// <param name="ct">A cancellation token</param>
    Task SaveAsync(T data, CancellationToken ct = default);

    /// <inheritdoc/>
    Task IStorageWriter.SaveAsync(object? data, CancellationToken ct) => data is T d ? SaveAsync(d, ct) : Task.FromException(new InvalidOperationException($"Given object is not of type {nameof(T)}"));
}

/// <summary>
/// Interface for implementation that can load/read a value from storage
/// </summary>
public interface IStorageReader
{
    /// <summary>
    /// Deserializes the type from storage
    /// </summary>
    /// <returns>The deserialized type</returns>
    /// <param name="ct">A cancellation token</param>
    Task<object?> LoadAsync(CancellationToken ct = default);
}


/// <summary>
/// Interface for implementation that can load/read the given type parameter from storage
/// </summary>
/// <typeparam name="T">The type being stored</typeparam>
public interface IStorageReader<T> : IStorageReader
{
    /// <summary>
    /// Deserializes the type from storage
    /// </summary>
    /// <returns>The deserialized type</returns>
    /// <param name="ct">A cancellation token</param>
    new Task<T> LoadAsync(CancellationToken ct = default);
    
    /// <inheritdoc/>
    async Task<object?> IStorageReader.LoadAsync(CancellationToken ct) => await (this as IStorageReader<T>).LoadAsync(ct);
}

/// <summary>
/// Interface for implementation that can load/read and save/write a value to/from storage
/// </summary>
public interface IStorage : IStorageReader, IStorageWriter;

/// <summary>
/// Interface for implementation that can load/read and save/write the given type parameter to/from storage
/// </summary>
/// <typeparam name="T">The type being stored</typeparam>
public interface IStorage<T> : IStorage, IStorageReader<T>, IStorageWriter<T>;
/// <summary>
/// Interface for implementation that can load/read and save/write the given type parameter to/from storage
/// </summary>
/// <typeparam name="T">The type being stored</typeparam>
/// <typeparam name="TWriterReturn">The type to return when saving is complete</typeparam>
public interface IStorage<T, TWriterReturn> : IStorage<T>, IStorageWriter<T, TWriterReturn>;