using Shiki.FileSystem.Storage.Batch;

namespace Shiki.FileSystem.Storage.Directory;

public interface IWritableDirectoryStorage<TValue> : IStorageWriter<List<TValue>, List<ActionError<string>>>;