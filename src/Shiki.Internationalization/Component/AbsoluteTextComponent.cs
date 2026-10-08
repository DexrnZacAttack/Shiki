using Shiki.Common.Serialization.Polymorphism;

namespace Shiki.Internationalization.Component;

[PolymorphicSerializable<AbsoluteTextComponent, ITextComponent>]
public record AbsoluteTextComponent(string Value) : ITextComponent;