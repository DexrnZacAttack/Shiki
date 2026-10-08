namespace Shiki.FileSystem.Storage.Directory.KeyValue;

public interface IDirectoryKeyValueStorage<TValue> :
    IReadableDirectoryKeyValueStorage<TValue>, IWritableDirectoryKeyValueStorage<TValue>;