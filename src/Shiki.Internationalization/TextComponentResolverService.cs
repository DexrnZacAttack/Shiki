using R3;
using Shiki.Internationalization.Component;
using Shiki.Internationalization.Component.Resolver;

namespace Shiki.Internationalization;

public class TextComponentResolverService
{
    private readonly List<ITextComponentResolver> _resolvers;
    
    public TextComponentResolverService(IEnumerable<ITextComponentResolver> resolvers)
    {
        _resolvers = [.. resolvers.OrderByDescending(p => p.Priority)];
    }

    public IReadOnlyBindableReactiveProperty<string> Resolve(ITextComponent str) =>
        _resolvers.FirstOrDefault(r => r.CanProvide(new TextComponentResolverArgs(str, this)))?.Provide(new TextComponentResolverArgs(str, this)) ?? new BindableReactiveProperty<string>(string.Empty);

    public string ResolveValue(ITextComponent str) => Resolve(str).Value;

    public TextComponentResolverService WithResolver(ITextComponentResolver resolver)
    {
        _resolvers.Add(resolver);
        
        _resolvers.Sort((lhs, rhs) => rhs.Priority.CompareTo(lhs.Priority));
        return this;
    }
}