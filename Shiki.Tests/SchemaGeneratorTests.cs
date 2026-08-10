using System.Text.Json;
using Shiki.Common.Identity;
using Shiki.Common.Result;
using Shiki.Tests.Util;

namespace Shiki.Tests;

public class SchemaGeneratorTests
{
    private SchemaGenerator _schemaGenerator;
    
    [SetUp]
    public void Setup()
    {
        this._schemaGenerator = new SchemaGenerator(JsonSerializerOptions.Default);
    }
    
    [Test, Explicit($"View the output schema for {nameof(Identifier)}")]
    public async Task GenerateIdentifierSchema() => Assert.Pass(await _schemaGenerator.GenerateSchema<Identifier>());
    
    [Test, Explicit($"View the output schema for {nameof(Result<string, FileNotFoundException>)}")]
    public async Task GenerateResultSchema() => Assert.Pass(await _schemaGenerator.GenerateSchema<Result<string, FileNotFoundException>>());
    
    [Test, Explicit($"View the output schema for {nameof(BooleanResult<>)}")]
    public async Task GenerateBooleanResultSchema() => Assert.Pass(await _schemaGenerator.GenerateSchema<BooleanResult<FileNotFoundException>>());
    
    [Test, Explicit($"View the output schema for {nameof(TransportableResult<>)}")]
    public async Task GenerateTransportableResultSchema() => Assert.Pass(await _schemaGenerator.GenerateSchema<TransportableResult<string>>());
    
    [Test, Explicit($"View the output schema for {nameof(TransportableBooleanResult)}")]
    public async Task GenerateTransportableBooleanResultSchema() => Assert.Pass(await _schemaGenerator.GenerateSchema<TransportableBooleanResult>());
}