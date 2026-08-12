using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace Shiki.Tests.Benchmark;

public class BaseBenchmarker<T>
where T: BaseBenchmarker<T>, new()
{
    [Test]
    [Category("Performance")]
    public void Run()
    {
        Summary summary = BenchmarkRunner.Run<T>();

        Assert.That(summary.HasCriticalValidationErrors, Is.False);
    }
}