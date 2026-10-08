namespace Shiki.FileSystem.Storage.Directory;

public interface IDirectoryStorage<TValue> :
    IReadableDirectoryStorage<TValue>, IWritableDirectoryStorage<TValue>;