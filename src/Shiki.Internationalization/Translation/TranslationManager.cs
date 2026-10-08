using R3;
using Shiki.Common.Identity.Slug;
using Shiki.Common.Identity.Slug.Formatting.Formatters;
using Shiki.Internationalization.Component;
using Shiki.Internationalization.Translation.LanguageSource;
using SmartFormat;

namespace Shiki.Internationalization.Translation;

public class TranslationManager
{
    public Dictionary<string, Language> Languages { get; } = [];
    public IReadOnlyBindableReactiveProperty<Language?> CurrentLanguage => _currentLanguage;

    private readonly BindableReactiveProperty<Language?> _currentLanguage = new();
    
    public TranslationManager(IEnumerable<LanguageDto> languages)
    {
        Languages = languages.ToDictionary(l => l.Id, l => new Language(l.Strings));
    }

    public TranslationManager()
    {
    }

    public TranslationManager UseLanguage(string id)
    {
        _currentLanguage.Value = Languages[id];
        return this;
    }


    public async Task<TranslationManager> ApplyLanguagesFromSourceAsync(ILanguageSource source, Slug<SnakeSlugFormatter> overlayIdentifier, int priority)
    {
        foreach (LanguageDto languageDto in await source.GetLanguagesAsync())
        {
            if (!Languages.TryGetValue(languageDto.Id, out Language? language))
            {
                language = new Language();
                Languages.Add(languageDto.Id, language);
            }
            
            language.SetOverlay(overlayIdentifier, priority, languageDto.Strings.ToDictionary(k => k.Key, string? (v) => v.Value));
        }

        return this;
    }

    public IReadOnlyBindableReactiveProperty<string> Provide(TranslatableTextComponent str) =>
        Provide(str, _currentLanguage);
    
    public IReadOnlyBindableReactiveProperty<string> Provide(TranslatableTextComponent str, BindableReactiveProperty<Language?> language)
    {
        return language.Select(l => l is not null ? l.GetString(str.TranslationId).AsObservable() : Observable.Return(str.TranslationId.ToString()))
                       .Switch()
                       .ToReadOnlyBindableReactiveProperty<string>();
    }
}