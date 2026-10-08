using Shiki.FileSystem.Storage.Batch;

namespace Shiki.FileSystem.Storage.Directory;

public interface IReadableDirectoryStorage<TValue> :
    IStorageReader<BatchActionResult<List<TValue>, string>>;