using System.Text.Json.Serialization;
using Shiki.Common.Serialization.Polymorphism;

namespace Shiki.Internationalization.Component;

[PolymorphicSerializable<ITextComponent>("absolute")]
public record AbsoluteTextComponent([property: JsonPropertyName("value")] string Value) : ITextComponent;