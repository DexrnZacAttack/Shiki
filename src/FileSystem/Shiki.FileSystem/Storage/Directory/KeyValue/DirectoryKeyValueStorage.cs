using Shiki.FileSystem.Abstractions;
using Shiki.FileSystem.Storage.Batch;
using Shiki.FileSystem.Storage.Serializer;

namespace Shiki.FileSystem.Storage.Directory.KeyValue;

public class DirectoryKeyValueStorage<TValue>(string directory, IPlatformDirectoryManager directoryManager, IPlatformFileManager fileManager, IStorageFileSerializer<TValue> serializer)
    : BaseDirectoryStorage(directory), IDirectoryKeyValueStorage<TValue>
{
    public async Task<BatchActionResult<Dictionary<string, TValue>, string>> LoadAsync(CancellationToken ct = default)
    {
        if (!await directoryManager.ExistsAsync(Directory))
        {
            try
            {
                await directoryManager.CreateDirectoryAsync(Directory);
            }
            catch (Exception e)
            {
                return new BatchActionResult<Dictionary<string, TValue>, string>([], [new ActionError<string>("*", e)]);
            }
        }

        List<ActionError<string>> errors = [];
        
        Dictionary<string, TValue> d = [];
        await foreach (string file in directoryManager.EnumerateAsync(Directory, serializer.FileExtensionGlob, IPlatformDirectoryManager.DirectoryEnumerationType.Files, ct))
        {
            try
            {
                await using Stream fs = await fileManager.GetStreamAsync(file, FileMode.Open, FileAccess.Read, FileShare.None);
                var res = await serializer.DeserializeAsync(fs, ct);
                if (!res.HasValue)
                {
                    errors.Add(new ActionError<string>(file, res.Error));
                    continue;
                }
                
                d.Add(Path.GetFileNameWithoutExtension(file), res.Value);
            }
            catch (Exception e)
            {
                errors.Add(new ActionError<string>(file, e));
            }
        }

        return new BatchActionResult<Dictionary<string, TValue>, string>(d, errors);
    }

    public async Task<List<ActionError<string>>> SaveAsync(Dictionary<string, TValue>? data, CancellationToken ct = default)
    {
        if (data is null)
            return [];

        List<ActionError<string>> errors = [];
        
        if (!await directoryManager.ExistsAsync(Directory))
        {
            await directoryManager.CreateDirectoryAsync(Directory);
        }

        foreach (var d in data)
        {
            string path = Path.Combine(Directory, $"{d.Key}.{serializer.FileExtension}");

            try
            {
                await using Stream fs = await fileManager.GetStreamAsync(path, FileMode.Create, FileAccess.Write, FileShare.None);
                var res = await serializer.SerializeAsync(fs, d.Value, ct);
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
    
    public async Task<List<ActionError<string>>> SaveAsync(string id, TValue data, CancellationToken ct = default)
    {
        if (data is null)
            return [];
        
        List<ActionError<string>> errors = [];
        
        if (!await directoryManager.ExistsAsync(Directory))
        {
            await directoryManager.CreateDirectoryAsync(Directory);
        }
        
        string path = Path.Combine(Directory, $"{id}.{serializer.FileExtension}");

        try
        {
            await using Stream fs = await fileManager.GetStreamAsync(path, FileMode.Create, FileAccess.Write, FileShare.None);
            await serializer.SerializeAsync(fs, data, ct);
        }
        catch (Exception e)
        {
            errors.Add(new ActionError<string>(path, e));
        }

        return errors;
    }

    public async Task DeleteAsync(string id, CancellationToken ct = default)
    {
        if (!await directoryManager.ExistsAsync(Directory))
            return;
        
        string path = Path.Combine(Directory, $"{id}.{serializer.FileExtension}");

        if (!await fileManager.ExistsAsync(path))
            return;

        await fileManager.DeleteAsync(path);
    }
}