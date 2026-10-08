using Shiki.Common.Result;
using Shiki.Extensions.OneOf;
using Shiki.FileSystem.Storage;

namespace Shiki.Internationalization.Translation.LanguageSource;

public class FileLanguageSource(FileStorage<LanguageDto> storage) : ILanguageSource
{
    public async Task<ICollection<LanguageDto>> GetLanguagesAsync()
    {
        var res = await storage.LoadAsync();
        res.ThrowIfError();
        
        return [res.Value!];
    }
}