using Shiki.FileSystem.Abstractions;
using Shiki.FileSystem.Storage.Batch;
using Shiki.FileSystem.Storage.Serializer;

namespace Shiki.FileSystem.Storage.Directory;



public class DirectoryStorage<TValue>(string directory, IPlatformDirectoryManager directoryManager, IPlatformFileManager fileManager, DirectoryStorage<TValue>.ValueToNameResolver resolver, IStorageFileSerializer<TValue> serializer)
    : BaseDirectoryStorage(directory), IDirectoryStorage<TValue>
{
    public delegate string ValueToNameResolver(TValue value);

    public async Task<BatchActionResult<List<TValue>, string>> LoadAsync(CancellationToken ct = default)
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
        await foreach (string file in directoryManager.EnumerateAsync(Directory, serializer.FileExtensionGlob, IPlatformDirectoryManager.DirectoryEnumerationType.Files, ct))
        {
            try
            {
                await using Stream fs = await fileManager.GetStreamAsync(file, FileMode.Open, FileAccess.Read, FileShare.Read);
                var res = await serializer.DeserializeAsync(fs, ct);

                if (!res.HasValue)
                {
                    errors.Add(new ActionError<string>(file, res.Error));
                    continue;
                }
                
                d.Add(res.Value);
            }
            catch (Exception e)
            {
                errors.Add(new ActionError<string>(file, e));
            }
        }

        return new BatchActionResult<List<TValue>, string>(d, errors);
    }

    public async Task<List<ActionError<string>>> SaveAsync(List<TValue>? data, CancellationToken ct = default)
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
                var res = await serializer.SerializeAsync(fs, d, ct);
                if (!res.IsSuccessful)
                {
                    errors.Add(new ActionError<string>(path, res.Error));
                }
            }
            catch (Exception e)
            {
                errors.Add(new ActionError<string>(path, e));
            }
        }

        return errors;
    }
}