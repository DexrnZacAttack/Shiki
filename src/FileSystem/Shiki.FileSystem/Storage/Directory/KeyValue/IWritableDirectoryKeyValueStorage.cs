using Shiki.FileSystem.Storage.Batch;

namespace Shiki.FileSystem.Storage.Directory.KeyValue;

public interface IWritableDirectoryKeyValueStorage<TValue> : IStorageWriter<Dictionary<string, TValue>, List<ActionError<string>>>;