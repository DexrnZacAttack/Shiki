using Shiki.FileSystem.Storage;
using Shiki.FileSystem.Storage.Directory.KeyValue;

namespace Shiki.Internationalization.Translation.LanguageSource;

public class DirectoryKeyValueLanguageSource(IReadableDirectoryKeyValueStorage<LanguageFileDto> storage) : ILanguageSource
{
    public async Task<ICollection<LanguageDto>> GetLanguagesAsync()
    {
        return
        [
            .. (await storage.LoadAsync()).Items.Select(l => new LanguageDto(l.Key,
                                                                             l.Value.Strings))
        ];
    }
}