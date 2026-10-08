using Shiki.FileSystem.Abstractions;
using Shiki.FileSystem.Storage.Batch;
using Shiki.FileSystem.Storage.Serializer;

namespace Shiki.FileSystem.Storage.Directory;



public class DirectoryStorage<TValue>(string directory, IPlatformDirectoryManager directoryManager, IPlatformFileManager fileManager, DirectoryStorage<TValue>.ValueToNameResolver resolver, IStorageFileSerializer<TValue> serializer)
    : BaseDirectoryStorage(directory), IDirectoryStorage<TValue>
{
    public delegate string ValueToNameResolver(TValue value);

    public async Task<BatchActionResult<List<TValue>, string>> LoadAsync()
    {
        if (!await directoryManager.ExistsAsync(Directory))
        {
            try
            {
                await directoryManager.CreateDirectoryAsync(Directory);
            }
            catch (Exception e)
            {
                return new BatchActionResult<List<TValue>, string>([], [new ActionError<string>("*", e)]);
            }
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

    public async Task<List<ActionError<string>>> SaveAsync(List<TValue>? data)
    {
        if (data is null)
            return [];
        
        if (!await directoryManager.ExistsAsync(Directory))
        {
            try
            {
                await directoryManager.CreateDirectoryAsync(Directory);
            }
            catch (Exception e)
            {
                return [new ActionError<string>("*", e)];
            }
        }

        List<ActionError<string>> errors = [];
        
        foreach (TValue d in data)
        {
            string path = Path.Combine(Directory, $"{resolver(d)}.{serializer.FileExtension}");

            try
            {
                await using Stream fs = await fileManager.GetStreamAsync(path, FileMode.Create, FileAccess.Write, FileShare.None);
                await serializer.SerializeAsync(fs, d);
            }
            catch (Exception e)
            {
                errors.Add(new ActionError<string>(path, e));
            }
        }

        return errors;
    }
}