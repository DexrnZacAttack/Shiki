using System.Text.Json.Serialization;
using ObservableCollections;
using R3;
using Shiki.Common.Extensions;
using Shiki.Common.Identity;
using Shiki.Common.Identity.Slug;
using Shiki.Common.Identity.Slug.Formatting.Formatters;
using Shiki.Internationalization.Component;

namespace Shiki.Internationalization.Translation;

public class Language : IDisposable
{
    private static readonly Slug<SnakeSlugFormatter> BaseId = "base".ToSlug<SnakeSlugFormatter>();

    public interface IOverlay
    {
        int Priority { get; }
        IReadOnlyObservableDictionary<Identifier, string?> Strings { get; }
    }
    
    public class MutableOverlay(int priority, ObservableDictionary<Identifier, string?> strings) : IOverlay
    {
        public int Priority { get; set; } = priority;
        public IReadOnlyObservableDictionary<Identifier, string?> Strings => MutableStrings;
        public ObservableDictionary<Identifier, string?> MutableStrings { get; } = strings;

        public MutableOverlay(int priority, IReadOnlyDictionary<Identifier, string?> strings) : this(priority, new ObservableDictionary<Identifier, string?>(strings))
        {
        }
    }
    
    private readonly Dictionary<Slug<SnakeSlugFormatter>, MutableOverlay> _overlays = [];
    private readonly Dictionary<Slug<SnakeSlugFormatter>, IDisposable> _subscriptions = [];

    private readonly Dictionary<Identifier, BindableReactiveProperty<string>> _observing = [];
    
    public IReadOnlyDictionary<Identifier, string> Strings => _strings;
    private readonly Dictionary<Identifier, string> _strings = [];

    [JsonIgnore]
    [JsonPropertyName("strings")]
    public ObservableDictionary<Identifier, string> BaseStrings
    {
        get => (GetOverlay(BaseId)! as MutableOverlay)!.MutableStrings!;
        set => SetOverlay(BaseId, 0, value.ToDictionary(o => o.Key, string? (v) => v.Value));
    }
    
    [JsonConstructor]
    public Language(IReadOnlyDictionary<Identifier, string> strings)
    {
        BaseStrings = new ObservableDictionary<Identifier, string>(strings);
    }
    
    public Language() {}

    public IReadOnlyBindableReactiveProperty<string> GetString(Identifier id)
    {
        if (!_observing.TryGetValue(id, out var prop))
        {
            string v = _strings.TryGetValue(id, out string? value) ? value : id.ToString();
            
            prop = new BindableReactiveProperty<string>(v);
            _observing.Add(id, prop);
        }

        return prop;
    }
    
    public void SetOverlay(Slug<SnakeSlugFormatter> id, int priority, IReadOnlyDictionary<Identifier, string?> strings)
    {
        if (_overlays.TryGetValue(id, out MutableOverlay? overlay))
        {
            overlay.MutableStrings.Clear();
            
            foreach (var keyValuePair in strings)
            {
                overlay.MutableStrings.Add(keyValuePair);
            }
            
            overlay.Priority = priority;
        }
        else
        {
            overlay = new MutableOverlay(priority, strings);
            _overlays[id] = overlay;
        }

        CompositeDisposable disposables = new();
        overlay.Strings.ObserveChanged().Subscribe(_ => Rebuild()).AddTo(disposables);

        if (_subscriptions.Remove(id, out IDisposable? disposable))
        {
            disposable.Dispose();
        }
        
        _subscriptions[id] = disposables;

        Rebuild();
    }

    public IOverlay? GetOverlay(Slug<SnakeSlugFormatter> id) => _overlays.TryGetValue(id, out MutableOverlay? overlay) ? overlay : null;
    public MutableOverlay? GetMutableOverlay(Slug<SnakeSlugFormatter> id) => _overlays.TryGetValue(id, out MutableOverlay? overlay) ? overlay : null;


    public void RemoveOverlay(Slug<SnakeSlugFormatter> id)
    {
        _overlays.Remove(id);

        if (_subscriptions.Remove(id, out IDisposable? disposable))
        {
            disposable.Dispose();
        }
        
        Rebuild();
    }

    private void Rebuild()
    {
        _strings.Clear();

        var sorted = _overlays.OrderBy(o => o.Value.Priority).ThenBy(kvp => kvp.Key);
        
        foreach (var keyValuePair in sorted)
        {
            foreach (var valueString in keyValuePair.Value.Strings)
            {
                if (valueString.Value != null)
                {
                    _strings[valueString.Key] = valueString.Value;
                }
                else
                {
                    _strings.Remove(valueString.Key);
                }
            }
        }
        
        foreach ((Identifier id, var prop) in _observing)
        {
            string str = _strings.TryGetValue(id, out string? v) ? v : id.ToString();
            
            if (prop.Value != str)
                prop.Value = str;
        }
    }

    public void Dispose()
    {
        foreach ((var key, IDisposable disposable) in _subscriptions)
        {
            disposable.Dispose();
        }
    }
}

public record LanguageDto(string Id, IReadOnlyDictionary<Identifier, string> Strings);
public record LanguageFileDto([property: JsonPropertyName("strings")] Dictionary<Identifier, string> Strings);