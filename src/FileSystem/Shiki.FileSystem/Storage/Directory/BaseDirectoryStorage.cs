namespace Shiki.FileSystem.Storage.Directory;

public class BaseDirectoryStorage(string directory)
{
    protected readonly string Directory = directory;
}