using System.Text.Json;
using Shiki.Common.Identity;
using Shiki.Common.Identity.Slug;
using Shiki.Common.Identity.Slug.Formatting.Formatters;
using Shiki.Common.Result;
using Shiki.Tests.Util.Extensions;

namespace Shiki.Tests;

public class DeserializerTests
{
    #region Identifier
    [Test, Description($"Deserialize json for {nameof(Identifier)}")]
    public void DeserializeIdentifier() =>
        Assert.Pass(JsonSerializer.Deserialize<Identifier>(""" "Shiki:tests/serializer/identifier" """)!.ToObjectString());
    
    [Test, Description($"Deserialize json for invalid {nameof(Identifier)}")]
    public void DeserializeInvalidIdentifier() =>
        Assert.Throws<FormatException>(() => JsonSerializer.Deserialize<Identifier>(""" "haha look at my invalid identifier" """)!.ToObjectString());
    #endregion
    
    #region Slug
    #region Dash
    
    [Test, Description($"Deserialize json for {nameof(Slug<DashSlugFormatter>)}")]
    public void DeserializeDashSlug() =>
        Assert.Pass(JsonSerializer.Deserialize<Slug<DashSlugFormatter>>(""" "hello-world" """)!.ToObjectString());
    
    [Test, Description($"Deserialize json for improper {nameof(Slug<DashSlugFormatter>)}")]
    public void DeserializeImproperDashSlug() =>
        Assert.Pass(JsonSerializer.Deserialize<Slug<DashSlugFormatter>>(""" "Hello, world!" """)!.ToObjectString());
    
    #endregion
    
    #region Snake
    [Test, Description($"Deserialize json for {nameof(Slug<SnakeSlugFormatter>)}")]
    public void DeserializeSnakeSlug() =>
        Assert.Pass(JsonSerializer.Deserialize<Slug<SnakeSlugFormatter>>(""" "hello_world" """)!.ToObjectString());
    
    [Test, Description($"Deserialize json for {nameof(Slug<SnakeSlugFormatter>)}")]
    public void DeserializeImproperSnakeSlug() =>
        Assert.Pass(JsonSerializer.Deserialize<Slug<SnakeSlugFormatter>>(""" "Hello, world!" """)!.ToObjectString());
    #endregion
    #endregion
    
    #region Result
    [Test, Description($"Make sure deserializer for {nameof(Result<string, NotImplementedException>)} throws")]
    public void DeserializeResultThrow() =>
        Assert.Throws<NotSupportedException>(() => JsonSerializer.Deserialize<Result<string, NotImplementedException>>(""" {"success": true, "value": "Hello, world!"} """)!.ToObjectString());
    
    #region BooleanResult
    [Test, Description($"Make sure deserializer for {nameof(BooleanResult<NotImplementedException>)} throws")]
    public void DeserializeBooleanResultThrow() =>
        Assert.Throws<NotSupportedException>(() => JsonSerializer.Deserialize<BooleanResult<NotImplementedException>>(""" {"success": true, "value": "Hello, world!"} """)!.ToObjectString());
    #endregion
    
    #region TransportableResult
    [Test, Description($"Deserialize json for {nameof(TransportableResult<string>)}")]
    public void DeserializeSuccessfulResultString() =>
        Assert.Pass(JsonSerializer.Deserialize<TransportableResult<string>>(""" {"success": true, "value": "Hello, world!"} """)!.ToObjectString());
    
    [Test, Description($"Deserialize json for {nameof(TransportableResult<string>)}")]
    public void DeserializeFailedResultString() =>
        Assert.Pass(JsonSerializer.Deserialize<TransportableResult<string>>($$""" {"success": false, "exception": {"type": "{{typeof(FileNotFoundException).FullName}}", "message": "{{nameof(TransportableResult<string>)}} deserialization test", "inner": null } } """)!.ToObjectString());

    #endregion
    
    #region TransportableBooleanResult
    [Test, Description($"Deserialize json for {nameof(TransportableBooleanResult)}")]
    public void DeserializeSuccessfulBooleanResultString() =>
        Assert.Pass(JsonSerializer.Deserialize<TransportableBooleanResult>(""" {"success": true} """)!.ToObjectString());
    
    [Test, Description($"Deserialize json for {nameof(TransportableBooleanResult)}")]
    public void DeserializeFailedBooleanResultString() =>
        Assert.Pass(JsonSerializer.Deserialize<TransportableBooleanResult>($$""" {"success": false, "exception": {"type": "{{typeof(FileNotFoundException).FullName}}", "message": "{{nameof(TransportableBooleanResult)}} deserialization test", "inner": null } } """)!.ToObjectString());
    #endregion
    
    #endregion
}