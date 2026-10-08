using Shiki.FileSystem.Abstractions;
using Shiki.FileSystem.Storage.Batch;
using Shiki.FileSystem.Storage.Serializer;

namespace Shiki.FileSystem.Storage.Directory.KeyValue;

public class ReadOnlyDirectoryKeyValueStorage<TValue>(string directory, IPlatformDirectoryManager directoryManager, IPlatformFileManager fileManager, IStorageFileSerializer<TValue> serializer)
    : BaseDirectoryStorage(directory), IReadableDirectoryKeyValueStorage<TValue>
{
    public async Task<BatchActionResult<Dictionary<string, TValue>, string>> LoadAsync()
    {
        if (!await directoryManager.ExistsAsync(Directory))
        {
            return new BatchActionResult<Dictionary<string, TValue>, string>([], [new ActionError<string>("*", new DirectoryNotFoundException())]);
        }

        List<ActionError<string>> errors = [];
        
        Dictionary<string, TValue> d = [];
        await foreach (string file in directoryManager.EnumerateAsync(Directory, serializer.FileExtensionGlob, IPlatformDirectoryManager.DirectoryEnumerationType.Files))
        {
            try
            {
                await using Stream fs = await fileManager.GetStreamAsync(file, FileMode.Open, FileAccess.Read, FileShare.None);
                TValue? data = await serializer.DeserializeAsync(fs);

                if (data != null)
                {
                    d.Add(Path.GetFileNameWithoutExtension(file), data);
                }
            }
            catch (Exception e)
            {
                errors.Add(new ActionError<string>(file, e));
            }
        }

        return new BatchActionResult<Dictionary<string, TValue>, string>(d, errors);
    }
}