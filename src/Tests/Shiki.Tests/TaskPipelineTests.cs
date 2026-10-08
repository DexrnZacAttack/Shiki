using System.Numerics;
using R3;
using Serilog;
using Shiki.TaskPipeline;
using Shiki.TaskPipeline.Builder;
using Shiki.TaskPipeline.Context;

namespace Shiki.Tests;

public class TaskPipelineTests
{
    public record TestContext : ITaskContext;

    public record TestContext2 : ITaskContext;

    public record TestContext3 : ITaskContext;

    public record TestContext4 : ITaskContext;

    public record TestContext5 : ITaskContext;

    public record TestContext6 : ITaskContext;

    public record TestContext7 : ITaskContext;

    public record TestContext8 : ITaskContext;

    public record TestContext9 : ITransitionableTaskContext<TestContext9, TestContext8>
    {
        public static TestContext9 TransitionFrom(TestContext8 oldContext) => new TestContext9();
    }

    public TaskPipelineBuilder<TestContext, TestContext9> Builder = TaskPipelineBuilder<TestContext>.Create()
       .ThenRun(() => //fully headless, synch
        {
            Log.Information("hello, fully headless, synch world!");
        })
       .ThenRun((ctx) => //headless, synch
        {
            Log.Information("hello, headless, synch world!");
        })
       .ThenRun((ctx, execCtx) => //synch
        {
            execCtx.Status.Value = "Synch task";

            Log.Information("hello, synch world! My context is '{Context}' and the current status is '{Status}'", ctx,
                            execCtx.Status.Value);

            Console.WriteLine($"hello, world! {ctx} {execCtx.Status.Value}");
        })
       .ThenRunAsync(async Task () => //fully headless, async
        {
            Console.WriteLine("hello, world!");
        })
       .ThenRunAsync(async ValueTask (ctx) => //headless, async
        {
            Console.WriteLine("hello, world! " + ctx);
        })
       .ThenRunAsync(async ValueTask (ctx, execCtx) => //async
        {
            execCtx.Status.Value = "Async task";

            Console.WriteLine($"hello, world! {ctx} {execCtx.Status.Value}");
        })
       .ThenRunToTransitionTo<TestContext2>(() => //fully headless, synch
        {
            Console.WriteLine("hello, world!");

            return new TestContext2();
        })
       .ThenRunToTransitionTo<TestContext3>((ctx) => //headless, synch
        {
            Console.WriteLine("hello, world! " + ctx);

            return new TestContext3();
        })
       .ThenRunToTransitionTo<TestContext4>((ctx, execCtx) => //synch
        {
            execCtx.Status.Value = "Synch task";

            Console.WriteLine($"hello, world! {ctx} {execCtx.Status.Value}");

            return new TestContext4();
        })
       .ThenRunToTransitionToAsync<TestContext5>(async Task<TestContext5> () => //fully headless, async
        {
            Console.WriteLine("hello, world!");

            return new TestContext5();
        })
       .ThenRunToTransitionToAsync<TestContext6>(async Task<TestContext6> (ctx) => //headless, async
        {
            Console.WriteLine("hello, world! " + ctx);

            return new TestContext6();
        })
       .ThenRunToTransitionToAsync<TestContext7>(async Task<TestContext7> (ctx, execCtx) => //async
        {
            execCtx.Status.Value = "Async task";

            Console.WriteLine($"hello, world! {ctx} {execCtx.Status.Value}");

            return new TestContext7();
        })
       .ThenRunToTransitionTo<TestContext8>(_ => new TestContext8())
       .ThenTransitionTo<TestContext9>();

    [OneTimeSetUp]
    public async Task AppendLogging()
    {
        CompositeDisposable disposables = new();

        Builder.TryInsertAtStart<TestContext>((ctx, execCtx) =>
        {
            Observable.Merge(
                 execCtx.Status.Select(object (s) => s),
                 execCtx.Progress.Select(object (s) => s)
            ).Skip(1)
             .Subscribe(_ => Log.Information("{Status} ({Progress:F0}/100% complete)", execCtx.Status.Value, execCtx.Progress.Value))
             .AddTo(disposables);

            return Task.CompletedTask;
        });

        Builder.TryInsertAtEnd<TestContext9>((_, _) =>
        {
            disposables.Dispose();

            return Task.CompletedTask;
        });
    }

    [Test, Description("Run example TaskPipeline")]
    public async Task RunExample()
    {
        await Builder.Build().RunAsync(new TestContext());

        Assert.Pass();
    }
}