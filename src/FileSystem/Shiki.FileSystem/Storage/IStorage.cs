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
    Task IStorageWriter<T>.SaveAsync(T data) => SaveAsync(data);
    
    /// <summary>
    /// Serializes the given value into storage
    /// </summary>
    /// <param name="data">The value to serialize</param>
    /// <returns>A return value of type from typeparam provided to the interface</returns>
    new Task<TReturn> SaveAsync(T data);
}

/// <summary>
/// Interface for implementation that can save/write the given type parameter to storage
/// </summary>
/// <typeparam name="T">The type being stored</typeparam>
public interface IStorageWriter<in T>
{
    /// <summary>
    /// Serializes the given value into storage
    /// </summary>
    /// <param name="data">The value to serialize</param>
    Task SaveAsync(T data);
}

/// <summary>
/// Interface for implementation that can load/read the given type parameter from storage
/// </summary>
/// <typeparam name="T">The type being stored</typeparam>
public interface IStorageReader<T>
{
    /// <summary>
    /// Deserializes the type from storage
    /// </summary>
    /// <returns>The deserialized type</returns>
    Task<T> LoadAsync();
}

/// <summary>
/// Interface for implementation that can load/read and save/write the given type parameter to/from storage
/// </summary>
/// <typeparam name="T">The type being stored</typeparam>
public interface IStorage<T> : IStorageReader<T>, IStorageWriter<T>;
/// <summary>
/// Interface for implementation that can load/read and save/write the given type parameter to/from storage
/// </summary>
/// <typeparam name="T">The type being stored</typeparam>
/// <typeparam name="TWriterReturn">The type to return when saving is complete</typeparam>
public interface IStorage<T, TWriterReturn> : IStorage<T>, IStorageWriter<T, TWriterReturn>;