using R3;
using Shiki.Common.Extensions;
using Shiki.Common.Result;
using Shiki.FileSystem.Storage;

namespace Shiki.Settings.Util;

/// <summary>
/// Object that is automatically written on disk on modification
/// </summary>
/// <typeparam name="T">The type to store</typeparam>
public sealed class StoredObject<T> : IEquatable<StoredObject<T>>, IAsyncDisposable, IDisposable
{
    /// <summary>
    /// The property
    /// </summary>
    public IBindableReactiveProperty<T> Property { get; }

    /// <summary>
    /// The value, which acts on the property
    /// </summary>
    public T Value
    {
        get => Property.Value;
        set => Property.Value = value;
    }

    /// <summary>
    /// The reader
    /// </summary>
    private readonly IStorageReader<Result<T, Exception>> _reader;
    /// <summary>
    /// The writer
    /// </summary>
    private readonly IStorageWriter<T, BooleanResult<Exception>> _writer;

    /// <summary>
    /// Disposables
    /// </summary>
    private readonly CompositeDisposable _disposables = new();

    /// <summary>
    /// Action semaphore
    /// </summary>
    private readonly SemaphoreSlim _semaphore = new(1, 1);//slim shady semaphore
    
    /// <summary>
    /// Whether the StoredObject has been disposed
    /// </summary>
    private bool _disposed;
    /// <summary>
    /// The amount of loads currently happening
    ///
    /// Prevents saving due to mutations during loading
    /// </summary>
    private int _loadCt;

    /// <summary>
    /// Whether the StoredObject has been disposed
    /// </summary>
    private bool Disposed => Volatile.Read(ref _disposed);

    /// <summary>
    /// Creates a new StoredObject from an existing bindable property
    /// </summary>
    /// <param name="property">The property</param>
    /// <param name="observableFactories">Factories for observables that trigger a save on fire</param>
    /// <param name="reader">The reader</param>
    /// <param name="writer">The writer</param>
    /// <param name="onErrorResume">A handler for SubscribeAwait's onErrorResume</param>
    /// <param name="onCompleted">A handler for SubscribeAwait's onCompleted</param>
    public StoredObject(IBindableReactiveProperty<T> property, IEnumerable<Func<T, Observable<T>>> observableFactories, IStorageReader<Result<T, Exception>> reader,
                        IStorageWriter<T, BooleanResult<Exception>> writer, Action<Exception> onErrorResume,
                        Action<Result> onCompleted)
    {
        _reader = reader;
        _writer = writer;
        
        Property = property;

        var observables = Property.AsObservable().Select(v => observableFactories.Select(f => f(v)).Merge()).Switch();
        
        Property.AsObservable().Merge(observables).Where(_ => Volatile.Read(ref _loadCt) == 0)
                   .Debounce(TimeSpan.FromMilliseconds(500), TimeProvider.System)
                   .SubscribeAwait(async (_, ct) =>
                                   {
                                       var res = await RunSaveAsync(ct);
                                       res.ThrowIfError();
                                   },
                                   onErrorResume,
                                   onCompleted).AddTo(_disposables);
    }

    /// <summary>
    /// Creates a new StoredObject from a given value
    /// </summary>
    /// <param name="value">The value</param>
    /// <param name="observableFactories">Factories for observables that trigger a save on fire</param>
    /// <param name="reader">The reader</param>
    /// <param name="writer">The writer</param>
    /// <param name="onErrorResume">A handler for SubscribeAwait's onErrorResume</param>
    /// <param name="onCompleted">A handler for SubscribeAwait's onCompleted</param>
    public StoredObject(T value, IEnumerable<Func<T, Observable<T>>> observableFactories,
                        IStorageReader<Result<T, Exception>> reader,
                        IStorageWriter<T, BooleanResult<Exception>> writer, Action<Exception> onErrorResume,
                        Action<Result> onCompleted) : this(new BindableReactiveProperty<T>(value), observableFactories,
                                                           reader, writer, onErrorResume, onCompleted)
    {
        
    }
    
    /// <summary>
    /// Creates a new StoredObject from an existing bindable property
    /// </summary>
    /// <param name="property">The property</param>
    /// <param name="reader">The reader</param>
    /// <param name="writer">The writer</param>
    /// <param name="onErrorResume">A handler for SubscribeAwait's onErrorResume</param>
    /// <param name="onCompleted">A handler for SubscribeAwait's onCompleted</param>
    public StoredObject(IBindableReactiveProperty<T> property, IStorageReader<Result<T, Exception>> reader,
                        IStorageWriter<T, BooleanResult<Exception>> writer, Action<Exception> onErrorResume,
                        Action<Result> onCompleted) : this(property, [], reader, writer, onErrorResume, onCompleted)
    {
    }
    
    /// <summary>
    /// Creates a new StoredObject from a given value
    /// </summary>
    /// <param name="value">The value</param>
    /// <param name="reader">The reader</param>
    /// <param name="writer">The writer</param>
    /// <param name="onErrorResume">A handler for SubscribeAwait's onErrorResume</param>
    /// <param name="onCompleted">A handler for SubscribeAwait's onCompleted</param>
    public StoredObject(T value, IStorageReader<Result<T, Exception>> reader,
                        IStorageWriter<T, BooleanResult<Exception>> writer, Action<Exception> onErrorResume,
                        Action<Result> onCompleted) : this(new BindableReactiveProperty<T>(value), reader, writer, onErrorResume, onCompleted)
    {
    }
    
    /// <summary>
    /// Creates a new StoredObject from an existing bindable property
    /// </summary>
    /// <param name="property">The property</param>
    /// <param name="observableFactories">Factories for observables that trigger a save on fire</param>
    /// <param name="reader">The reader</param>
    /// <param name="writer">The writer</param>
    public StoredObject(IBindableReactiveProperty<T> property, IEnumerable<Func<T, Observable<T>>> observableFactories, IStorageReader<Result<T, Exception>> reader, IStorageWriter<T, BooleanResult<Exception>> writer) : this(property, observableFactories, reader, writer, _ => {}, _ => {})
    {
       
    }
    
    /// <summary>
    /// Creates a new StoredObject from a given value
    /// </summary>
    /// <param name="value">The value</param>
    /// <param name="observableFactories">Factories for observables that trigger a save on fire</param>
    /// <param name="reader">The reader</param>
    /// <param name="writer">The writer</param>
    public StoredObject(T value, IEnumerable<Func<T, Observable<T>>> observableFactories, IStorageReader<Result<T, Exception>> reader, IStorageWriter<T, BooleanResult<Exception>> writer) : this(new BindableReactiveProperty<T>(value), observableFactories, reader, writer)
    {
       
    }
    
    /// <summary>
    /// Creates a new StoredObject from an existing bindable property
    /// </summary>
    /// <param name="property">The property</param>
    /// <param name="reader">The reader</param>
    /// <param name="writer">The writer</param>
    public StoredObject(IBindableReactiveProperty<T> property, IStorageReader<Result<T, Exception>> reader, IStorageWriter<T, BooleanResult<Exception>> writer) : this(property, [], reader, writer, _ => {}, _ => {})
    {
       
    }
        
    /// <summary>
    /// Creates a new StoredObject from a given value
    /// </summary>
    /// <param name="value">The value</param>
    /// <param name="reader">The reader</param>
    /// <param name="writer">The writer</param>
    public StoredObject(T value, IStorageReader<Result<T, Exception>> reader, IStorageWriter<T, BooleanResult<Exception>> writer) : this(new BindableReactiveProperty<T>(value),  reader, writer)
    {
       
    }

    /// <summary>
    /// Creates and loads a new StoredObject with the given value
    /// </summary>
    /// <param name="value">The value</param>
    /// <param name="storage">The storage</param>
    /// <typeparam name="TStorage">Storage type that matches both the correct reader,writer</typeparam>
    /// <returns>The new StoredObject</returns>
    public static async Task<StoredObject<T>> CreateInstanceAsync<TStorage>(T value, TStorage storage)
    where TStorage : IStorageReader<Result<T, Exception>>, IStorageWriter<T, BooleanResult<Exception>>
    {
        StoredObject<T> obj = new StoredObject<T>(value, storage, storage);

        try
        {
            var res = await obj.LoadAsync();
            if (!res.IsSuccessful)
            {
                await obj.DisposeAsync();
                throw res.Error!;
            }

            return obj;
        }
        catch
        {
            await obj.DisposeAsync();
            throw;
        }
    }
    
    /// <summary>
    /// Tries to create and load a new StoredObject with the given value
    /// </summary>
    /// <param name="value">The value</param>
    /// <param name="storage">The storage</param>
    /// <typeparam name="TStorage">Storage type that matches both the correct reader,writer</typeparam>
    /// <returns>The new StoredObject, or an exception on failure</returns>
    public static async Task<Result<StoredObject<T>, Exception>> TryCreateInstanceAsync<TStorage>(T value, TStorage storage)
    where TStorage : IStorageReader<Result<T, Exception>>, IStorageWriter<T, BooleanResult<Exception>>
    {
        StoredObject<T> obj = new StoredObject<T>(value, storage, storage);

        try
        {
            var res = await obj.LoadAsync();
            if (!res.IsSuccessful)
            {
                await obj.DisposeAsync();
                return res.Error!;
            }

            return obj;
        }
        catch (Exception ex)
        {
            await obj.DisposeAsync();
            return ex;
        }
    }

    /// <summary>
    /// Creates and loads a new StoredObject with the given value
    /// </summary>
    /// <param name="value">The value</param>
    /// <param name="observableFactories">Factories for observables that trigger a save on fire</param>
    /// <param name="storage">The storage</param>
    /// <typeparam name="TStorage">Storage type that matches both the correct reader,writer</typeparam>
    /// <returns>The new StoredObject</returns>
    public static async Task<StoredObject<T>> CreateInstanceAsync<TStorage>(T value, IEnumerable<Func<T, Observable<T>>> observableFactories, TStorage storage)
    where TStorage : IStorageReader<Result<T, Exception>>, IStorageWriter<T, BooleanResult<Exception>>
    {
        StoredObject<T> obj = new(value, observableFactories, storage, storage);

        try
        {
            var res = await obj.LoadAsync();
            if (!res.IsSuccessful)
            {
                await obj.DisposeAsync();
                throw res.Error!;
            }

            return obj;
        }
        catch
        {
            await obj.DisposeAsync();
            throw;
        }
    }

    /// <summary>
    /// Tries to create and load a new StoredObject with the given value
    /// </summary>
    /// <param name="value">The value</param>
    /// <param name="observableFactories">Factories for observables that trigger a save on fire</param>
    /// <param name="storage">The storage</param>
    /// <typeparam name="TStorage">Storage type that matches both the correct reader,writer</typeparam>
    /// <returns>The new StoredObject, or an exception on failure</returns>
    public static async Task<Result<StoredObject<T>, Exception>> TryCreateInstanceAsync<TStorage>(T value, IEnumerable<Func<T, Observable<T>>> observableFactories, TStorage storage)
    where TStorage : IStorageReader<Result<T, Exception>>, IStorageWriter<T, BooleanResult<Exception>>
    {
        StoredObject<T> obj = new StoredObject<T>(value, observableFactories, storage, storage);

        try
        {
            var res = await obj.LoadAsync();
            if (!res.IsSuccessful)
            {
                await obj.DisposeAsync();
                return res.Error!;
            }

            return obj;
        }
        catch (Exception ex)
        {
            await obj.DisposeAsync();
            return ex;
        }
    }
    
    /// <summary>
    /// Loads the object from storage
    /// </summary>
    /// <param name="ct">A cancellation token</param>
    /// <returns>The result</returns>
    public async ValueTask<BooleanResult<Exception>> LoadAsync(CancellationToken ct = default)
    {
        if (Disposed)
            return ObjectDisposedException.GetFor(this);
        
        Interlocked.Increment(ref _loadCt);
        try
        {
            if (Disposed)
                return ObjectDisposedException.GetFor(this);
            
            await _semaphore.WaitAsync(ct);//got that EnterCriticalSection
            try
            {
                var res = await _reader.LoadAsync(ct);
                if (!res.HasValue)
                    return res.Error;

                Property.Value = res.Value;
            }
            finally
            {
                _semaphore.Release();
            }

        }
        finally
        {
            Interlocked.Decrement(ref _loadCt);
        }

        return BooleanResult<Exception>.Successful;
    }
    
    /// <summary>
    /// Saves the object to disk
    /// </summary>
    /// <param name="ct">The cancellation token</param>
    /// <returns>The result</returns>
    private async ValueTask<BooleanResult<Exception>> RunSaveAsync(CancellationToken ct = default)
    {
        if (Disposed)
            return ObjectDisposedException.GetFor(this);
        
        await _semaphore.WaitAsync(ct);
        try
        {
            return await _writer.SaveAsync(Property.Value, ct);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <inheritdoc/>
    public bool Equals(StoredObject<T>? other) => other != null && EqualityComparer<T>.Default.Equals(Property.Value, other.Property.Value);
    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is StoredObject<T> other && Equals(other);
    /// <inheritdoc/>
    public override int GetHashCode() => Property.Value == null ? 0 : EqualityComparer<T>.Default.GetHashCode(Property.Value);
    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref _disposed, true))
            return;
        
        _disposables.Dispose();
        
        await _semaphore.WaitAsync();
        try
        {
            Property.Dispose();
        }
        finally
        {
            _semaphore.Release();
            // _semaphore.Dispose();
        }
    }
    /// <inheritdoc/>
    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, true))
            return;
        
        _disposables.Dispose();
        
        _semaphore.Wait();
        try
        {
            Property.Dispose();
        }
        finally
        {
            _semaphore.Release();
            // _semaphore.Dispose();
        }
    }
}