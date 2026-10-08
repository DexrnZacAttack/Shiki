namespace Shiki.Internationalization.Translation.LanguageSource;

public interface ILanguageSource
{
    public Task<ICollection<LanguageDto>> GetLanguagesAsync();
}