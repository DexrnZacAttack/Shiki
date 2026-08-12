using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using Shiki.Common.Identity;
using Shiki.Common.Result;
using Shiki.Tests.Benchmark;
using Shiki.Tests.Util;
using Shiki.Tests.Util.Extensions;

namespace Shiki.Tests;

public class IdentifierTests
{
    private static readonly Identifier Id = Constants.Root.Derived("identifier");
    
    [Test, Description($"New {nameof(Identifier)}")]
    public void CreateIdentifier() => Assert.Pass(new Identifier(typeof(IdentifierTests).Namespace!, "identifier", "create").ToObjectString());
    
    [Test, Description($"New {nameof(Identifier)} using namespace of {nameof(IdentifierTests)}")]
    public void CreateIdentifierWithNamespaceOfType() => Assert.Pass(Identifier.WithNamespaceOfType<IdentifierTests>("identifier", "create_with_namespace_of_type").ToObjectString());
    
    [Test, Description($"New {nameof(Identifier)} using {nameof(IdentifierTests)} as namespace")]
    public void CreateIdentifierWithTypeAsNamespace() => Assert.Pass(Identifier.WithTypeAsNamespace<IdentifierTests>("create_with_type_as_namespace").ToObjectString());
    
    [Test, Description($"New {nameof(Identifier)} with constant {nameof(IdentifierNamespace)} as namespace")]
    public void CreateIdentifierWithIdentifierNamespaceConstant() => Assert.Pass(Constants.Root.Derived("identifier", "create_with_identifier_namespace_constant").ToObjectString());
    
    [Test, Description($"New {nameof(Identifier)} derived from {nameof(Id)}")]
    public void CreateDerivedIdentifier() => Assert.Pass(Id.Derived("create_derived_identifier").ToObjectString());
    
    [Test, Description($"New parsed {nameof(Identifier)} with string")]
    public void ParseStringIntoIdentifier() => Assert.Pass(Identifier.TryParseIntoResult("Shiki.Tests:identifier/parse_string").ToObjectString());
    
    [Test, Description($"New parsed {nameof(Identifier)} with invalid string")]
    public void ParseInvalidStringIntoIdentifier() => Assert.Pass(Identifier.TryParseIntoResult("Shiki.Tests:identifier/parse_invalid_string:::").ToObjectString());
    
    [Test, Explicit($"Make sure with syntax doesn't work on {nameof(Identifier)} instance")]
    public void TestWithSyntax()
    {
        Identifier id = new("Shiki.Tests", "identifier", "test_with_syntax");
        
        Identifier i = id with
        {
            
        };

        Assert.That(i, Is.EqualTo(id), "with syntax does work");
    }
}