using Shiki.FileSystem.Storage.Batch;

namespace Shiki.FileSystem.Storage.Directory.KeyValue;

public interface IReadableDirectoryKeyValueStorage<TValue> :
    IStorageReader<BatchActionResult<Dictionary<string, TValue>, string>>;