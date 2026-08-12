using BenchmarkDotNet.Attributes;
using Shiki.Common.Identity;

namespace Shiki.Tests.Benchmark;

[MemoryDiagnoser]
public class IdentifierBenchmark : BaseBenchmarker<IdentifierBenchmark>
{
    private static void CreateIdentifiers(int cnt)
    {
        for (int i = 0; i < cnt; i++)
        {
            new Identifier("Shiki.Tests", "identifier", "benchmark_1000_identifiers");
        }
    }
    
    private static void CreateStringIdentifiers(int cnt)
    {
        for (int i = 0; i < cnt; i++)
        {
            Identifier.TryParseIntoResult("Shiki.Tests:identifier/benchmark_1000_identifiers");
        }
    }
    
    [Benchmark]
    public void Create100Identifiers() => CreateIdentifiers(100);
    
    [Benchmark]
    public void Create1000Identifiers() => CreateIdentifiers(1000);
    
    [Benchmark]
    public void Create100StringIdentifiers() => CreateStringIdentifiers(100);
    
    [Benchmark]
    public void Create1000StringIdentifiers() => CreateStringIdentifiers(1000);
}