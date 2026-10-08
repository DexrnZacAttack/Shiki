using R3;

namespace Shiki.Internationalization.Component.Resolver;

public class AbsoluteTextComponentResolver : ITextComponentResolver<AbsoluteTextComponent>
{
    public int Priority => 0;

    public IReadOnlyBindableReactiveProperty<string> Provide(AbsoluteTextComponent component, TextComponentResolverService resolverService)
    {
        return new BindableReactiveProperty<string>(component.Value);
    }
}