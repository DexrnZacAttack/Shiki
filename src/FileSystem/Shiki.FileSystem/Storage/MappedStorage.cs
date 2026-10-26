using Shiki.Common.Result;
using Shiki.FileSystem.Storage.Batch;

namespace Shiki.FileSystem.Storage;

public class MappedStorage<T, TStored>(
    IStorageReader<Result<TStored, Exception>> reader,
    IStorageWriter<TStored, BooleanResult<Exception>> writer,
    Func<TStored, T> toObject,
    Func<T, TStored> toStored) : IStorageReader<Result<T, Exception>>, IStorageWriter<T, BooleanResult<Exception>>
{
    public async Task<Result<T, Exception>> LoadAsync(CancellationToken ct = default)
    {
        var res = await reader.LoadAsync(ct);
        if (!res.HasValue) return res.Error;

        try
        {
            return toObject(res.Value);
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<BooleanResult<Exception>> SaveAsync(T data, CancellationToken ct = default)
    {
        try
        {
            var res = await writer.SaveAsync(toStored(data), ct);
            if (!res.IsSuccessful) return res.Error!;
        }
        catch (Exception ex)
        {
            return ex;
        }

        return BooleanResult<Exception>.Successful;
    }
}

public class MappedBatchStorage<T, TStored, TError>(
    IStorageReader<BatchActionResult<TStored, TError>> reader,
    IStorageWriter<TStored, List<ActionError<TError>>> writer,
    Func<TStored, T> toObject,
    Func<T, TStored> toStored) : IStorageReader<Result<T, Exception>>, IStorageWriter<T, BooleanResult<Exception>>
{
    public async Task<Result<T, Exception>> LoadAsync(CancellationToken ct = default)
    {
        var res = await reader.LoadAsync(ct);
        if (res.Errors.Count > 0)
            return new AggregateException(res.Errors.Select(e => e.Exception));

        try
        {
            return toObject(res.Items);
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<BooleanResult<Exception>> SaveAsync(T data, CancellationToken ct = default)
    {
        try
        {
            var res = await writer.SaveAsync(toStored(data), ct);
            if (res.Count > 0)
                return new AggregateException(res.Select(e => e.Exception));
        }
        catch (Exception ex)
        {
            return ex;
        }

        return BooleanResult<Exception>.Successful;
    }
}
