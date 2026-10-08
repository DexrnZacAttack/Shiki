using R3;
using Shiki.Internationalization.Translation;
using SmartFormat;

namespace Shiki.Internationalization.Component.Resolver;

public class TranslatableTextComponentResolver(TranslationManager manager) : ITextComponentResolver<TranslatableTextComponent>
{
    public int Priority => 0;

    public IReadOnlyBindableReactiveProperty<string> Provide(TranslatableTextComponent component, TextComponentResolverService resolverService)
    {
        var v = manager.Provide(component);

        if (component.Arguments.Length == 0)
        {
            return v;
        }

        var a = component.Arguments.Select(s => resolverService.Resolve(s).AsObservable());

        return v.AsObservable().CombineLatest(Observable.CombineLatest(a), (i1, i2) => Smart.Format(i1, i2) ?? i1).ToReadOnlyBindableReactiveProperty();
    }
}