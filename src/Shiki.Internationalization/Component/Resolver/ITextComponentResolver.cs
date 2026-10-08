using R3;
using Shiki.Common.Util;

namespace Shiki.Internationalization.Component.Resolver;

public record TextComponentResolverArgs(ITextComponent TextComponent, TextComponentResolverService ResolverService);

public interface
    ITextComponentResolver : IProvider<IReadOnlyBindableReactiveProperty<string>, TextComponentResolverArgs>
{
    bool IProvider<IReadOnlyBindableReactiveProperty<string>, TextComponentResolverArgs>.CanProvide(TextComponentResolverArgs args) => CanProvide(args.TextComponent);
    
    bool CanProvide(ITextComponent component);

    IReadOnlyBindableReactiveProperty<string> IProvider<IReadOnlyBindableReactiveProperty<string>, TextComponentResolverArgs>.Provide(TextComponentResolverArgs args) => Provide(args.TextComponent, args.ResolverService);
    
    IReadOnlyBindableReactiveProperty<string> Provide(ITextComponent component, TextComponentResolverService resolverService);
}

public interface ITextComponentResolver<in TTextComponent> : ITextComponentResolver
    where TTextComponent : ITextComponent
{
    bool ITextComponentResolver.CanProvide(ITextComponent component) => component is TTextComponent c && CanProvide(c);
    bool CanProvide(TTextComponent component) => true;
    
    IReadOnlyBindableReactiveProperty<string> ITextComponentResolver.Provide(ITextComponent component, TextComponentResolverService resolverService) => Provide((TTextComponent)component, resolverService);
    
    IReadOnlyBindableReactiveProperty<string> Provide(TTextComponent component, TextComponentResolverService resolverService);

}