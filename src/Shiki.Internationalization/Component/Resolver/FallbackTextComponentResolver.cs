using R3;

namespace Shiki.Internationalization.Component.Resolver;

public class FallbackTextComponentResolver : ITextComponentResolver
{
    public int Priority => int.MinValue;

    public bool CanProvide(ITextComponent args) => true;

    public IReadOnlyBindableReactiveProperty<string> Provide(ITextComponent component, TextComponentResolverService resolverService)
    {
        return new BindableReactiveProperty<string>(component.ToString() ?? "null");
    }
}