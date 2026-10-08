using Shiki.FileSystem.Storage;
using Shiki.FileSystem.Storage.Directory;

namespace Shiki.Internationalization.Translation.LanguageSource;

public class DirectoryLanguageSource(DirectoryStorage<LanguageDto> storage) : ILanguageSource
{
    public async Task<ICollection<LanguageDto>> GetLanguagesAsync()
    {
        return (await storage.LoadAsync()).Items;
    }
}