using System.Text.Json;
using Shiki.Common.Identity;
using Shiki.Common.Identity.Slug;
using Shiki.Common.Identity.Slug.Formatting.Formatters;
using Shiki.Common.Result;

namespace Shiki.Tests;

public class SerializerTests
{
    [Test, Description($"Output json for {nameof(Identifier)} instance")]
    public void SerializeIdentifier() =>
        Assert.Pass(JsonSerializer.Serialize(new Identifier("Shiki.Tests", "serializer", "identifier")));
    
    [Test, Description($"Output json for {nameof(Slug<DashSlugFormatter>)} instance")]
    public void SerializeSlug() =>
        Assert.Pass(JsonSerializer.Serialize(new Slug<DashSlugFormatter>("hello-world")));
    
    [Test, Description($"Output json for improperly created {nameof(Slug<DashSlugFormatter>)} instance")]
    public void SerializeImproperSlug() =>
        Assert.Pass(JsonSerializer.Serialize(new Slug<DashSlugFormatter>("Hello, world!")));
    
    [Test, Description($"Output json for {nameof(Slug<SnakeSlugFormatter>)} instance")]
    public void SerializeSnakeSlug() =>
        Assert.Pass(JsonSerializer.Serialize(new Slug<SnakeSlugFormatter>("hello_world")));
    
    [Test, Description($"Output json for improperly created {nameof(Slug<SnakeSlugFormatter>)} instance")]
    public void SerializeImproperSnakeSlug() =>
        Assert.Pass(JsonSerializer.Serialize(new Slug<SnakeSlugFormatter>("Hello, world!")));
    
    [Test, Description($"Output json for {nameof(Result<string, FileNotFoundException>)} instance")]
    public void SerializeSuccessfulResultString() =>
        Assert.Pass(JsonSerializer.Serialize(new Result<string, FileNotFoundException>("Hello, world!").GetTransportableResult()));
    
    [Test, Description($"Output json for {nameof(Result<string, FileNotFoundException>)} instance")]
    public void SerializeFailedResultString() =>
        Assert.Pass(JsonSerializer.Serialize(new Result<string, FileNotFoundException>(new FileNotFoundException("Failed because fuck you!!!!")).GetTransportableResult()));
    
    [Test, Description($"Output json for successful {nameof(BooleanResult<>)} instance")]
    public void SerializeSuccessfulBooleanResult() =>
        Assert.Pass(JsonSerializer.Serialize(new BooleanResult<FileNotFoundException>(null).GetTransportableResult()));
    
    [Test, Description($"Output json for unsuccessful {nameof(BooleanResult<>)} instance")]
    public void SerializeFailedBooleanResult() =>
        Assert.Pass(JsonSerializer.Serialize(new BooleanResult<FileNotFoundException>(new FileNotFoundException("garbage")).GetTransportableResult()));
}