using Shiki.FileSystem.Abstractions;
using Shiki.FileSystem.Storage.Batch;
using Shiki.FileSystem.Storage.Serializer;

namespace Shiki.FileSystem.Storage.Directory;

public class ReadOnlyDirectoryStorage<TValue>(string directory, IPlatformDirectoryManager directoryManager, IPlatformFileManager fileManager, DirectoryStorage<TValue>.ValueToNameResolver resolver, IStorageFileSerializer<TValue> serializer)
    : BaseDirectoryStorage(directory), IReadableDirectoryStorage<TValue>
{
    public async Task<BatchActionResult<List<TValue>, string>> LoadAsync()
    {
        if (!await directoryManager.ExistsAsync(Directory))
        {
            return new BatchActionResult<List<TValue>, string>([], [new ActionError<string>("*", new DirectoryNotFoundException())]);
        }
        
        List<ActionError<string>> errors = [];
        
        List<TValue> d = [];
        await foreach (string file in directoryManager.EnumerateAsync(Directory, serializer.FileExtensionGlob, IPlatformDirectoryManager.DirectoryEnumerationType.Files))
        {
            try
            {
                await using Stream fs = await fileManager.GetStreamAsync(file, FileMode.Open, FileAccess.Read, FileShare.Read);
                TValue? data = await serializer.DeserializeAsync(fs);

                if (data != null)
                {
                    d.Add(data);
                }
            }
            catch (Exception e)
            {
                errors.Add(new ActionError<string>(file, e));
            }
        }

        return new BatchActionResult<List<TValue>, string>(d, errors);
    }
}