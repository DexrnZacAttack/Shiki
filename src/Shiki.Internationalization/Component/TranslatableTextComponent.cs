using Shiki.Common.Identity;
using Shiki.Common.Serialization.Polymorphism;

namespace Shiki.Internationalization.Component;

[PolymorphicSerializable<TranslatableTextComponent, ITextComponent>]
public record TranslatableTextComponent(Identifier TranslationId, params ITextComponent[] Arguments) : ITextComponent
{
    public TranslatableTextComponent(Identifier translationId, params string[] format) : this(translationId,
        [.. format.Select(f => new AbsoluteTextComponent(f))]) {}
    
    public TranslatableTextComponent(Identifier translationId) : this(translationId, Array.Empty<ITextComponent>()) {}
    
    public TranslatableTextComponent(string translationId, params ITextComponent[] arguments) : this(Identifier.CreateInstance(translationId), arguments) {}
    public TranslatableTextComponent(string translationId, params string[] format) : this(Identifier.CreateInstance(translationId), format) {}

    public TranslatableTextComponent(string translationId) : this(Identifier.CreateInstance(translationId)) {}
    
    public TranslatableTextComponent(Identifier translationId, params object[] format) : this(translationId,
        [.. format.Select(f => new AbsoluteTextComponent(f.ToString() ?? "null"))]) {}
    
    public TranslatableTextComponent(string translationId, params object[] format) : this(Identifier.CreateInstance(translationId), format) {}
    
    
    public override string ToString() => $"{TranslationId}, Params: {string.Join( ", ", Arguments)}";
}