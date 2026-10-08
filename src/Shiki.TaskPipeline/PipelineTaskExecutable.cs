using Shiki.TaskPipeline.Context;

namespace Shiki.TaskPipeline;

public interface IPipelineTaskExecutable
{
    /// <summary>
    /// The task being executed
    /// </summary>
    public Delegate Runnable { get; }
    /// <summary>
    /// Calls the Task
    ///
    /// If no extra logic to call the Task is required, it is ideal to set this to the same value as the Task to avoid overhead 
    /// </summary>
    public Delegate Executor { get; }
}

/// <summary>
/// Wrapper to allow for extra logic when calling the PipelineTask
/// </summary>
/// <typeparam name="TContext">The context</typeparam>
public readonly struct PipelineTaskExecutable<TContext> : IPipelineTaskExecutable
where TContext : ITaskContext
{
    /// <inheritdoc/>
    public Delegate Runnable { get; }
    /// <summary>
    /// Calls the Task
    ///
    /// If no extra logic to call the Task is required, it is ideal to set this to the same value as the Task to avoid overhead 
    /// </summary>
    public PipelineValueTask<TContext> Executor { get; }

    /// <inheritdoc/>
    Delegate IPipelineTaskExecutable.Executor => Executor;

    public PipelineTaskExecutable(Delegate runnable, PipelineValueTask<TContext> executor)
    {
        Runnable = runnable;
        Executor = executor;
    }
    
    public PipelineTaskExecutable(PipelineValueTask<TContext> runnable)
    {
        Runnable = runnable;
        Executor = runnable;
    }

    public static implicit operator PipelineTaskExecutable<TContext>(PipelineTask<TContext> task) => new(task, (ctx, execCtx) => new ValueTask(task(ctx, execCtx)));
    public static implicit operator PipelineTaskExecutable<TContext>(HeadlessPipelineTask<TContext> task) => new(task, (ctx, _) => new ValueTask(task(ctx)));
    public static implicit operator PipelineTaskExecutable<TContext>(PipelineValueTask<TContext> task) => new(task);
    public static implicit operator PipelineTaskExecutable<TContext>(HeadlessPipelineValueTask<TContext> task) => new(task, (ctx, _) => task(ctx));
    
    public static implicit operator PipelineTaskExecutable<TContext>(SynchronousPipelineTask<TContext> task) => new(task,
        (ctx, execCtx) =>
        {
            task(ctx,execCtx);
            
            return default;
        });
    public static implicit operator PipelineTaskExecutable<TContext>(SynchronousHeadlessPipelineTask<TContext> task) => new(task,
        (ctx, _) =>
        {
            task(ctx);
            
            return default;
        });
    
    public static implicit operator PipelineTaskExecutable<TContext>(Func<Task> task) => new(task, (_, _) => new ValueTask(task()));
    public static implicit operator PipelineTaskExecutable<TContext>(Func<ValueTask> task) => new(task, (_, _) => task());
    public static implicit operator PipelineTaskExecutable<TContext>(Action task) => new(task, (_, _) =>
    {
        task();
        
        return default;
    });
}

/// <summary>
/// Wrapper to allow for extra logic when calling the PipelineTask
/// </summary>
/// <typeparam name="TCurrentContext">The context</typeparam>
/// <typeparam name="TNewContext">The context to transition to</typeparam>
public readonly struct PipelineTransitionTaskExecutable<TCurrentContext, TNewContext> : IPipelineTaskExecutable
where TCurrentContext : ITaskContext
where TNewContext : ITaskContext
{
    /// <inheritdoc/>
    public Delegate Runnable { get; }
    /// <summary>
    /// Calls the Task
    ///
    /// If no extra logic to call the Task is required, it is ideal to set this to the same value as the Task to avoid overhead 
    /// </summary>
    public TransitionValueTask<TCurrentContext, TNewContext> Executor { get; }

    /// <inheritdoc/>
    Delegate IPipelineTaskExecutable.Executor => Executor;

    public PipelineTransitionTaskExecutable(Delegate runnable, TransitionValueTask<TCurrentContext, TNewContext> executor)
    {
        Runnable = runnable;
        Executor = executor;
    }
    
    public PipelineTransitionTaskExecutable(TransitionValueTask<TCurrentContext, TNewContext> runnable)
    {
        Runnable = runnable;
        Executor = runnable;
    }

    public static implicit operator PipelineTransitionTaskExecutable<TCurrentContext, TNewContext>(TransitionTask<TCurrentContext, TNewContext> task) => new(task, (ctx, execCtx) => new ValueTask<TNewContext>(task(ctx, execCtx)));
    public static implicit operator PipelineTransitionTaskExecutable<TCurrentContext, TNewContext>(HeadlessTransitionTask<TCurrentContext, TNewContext> task) => new(task, (ctx, _) => new ValueTask<TNewContext>(task(ctx)));
    public static implicit operator PipelineTransitionTaskExecutable<TCurrentContext, TNewContext>(TransitionValueTask<TCurrentContext, TNewContext> task) => new(task);
    public static implicit operator PipelineTransitionTaskExecutable<TCurrentContext, TNewContext>(HeadlessTransitionValueTask<TCurrentContext, TNewContext> task) => new(task, (ctx, _) => task(ctx));
    public static implicit operator PipelineTransitionTaskExecutable<TCurrentContext, TNewContext>(SynchronousTransitionTask<TCurrentContext, TNewContext> task) => new(task, (ctx, execCtx) => new ValueTask<TNewContext>(task(ctx, execCtx)));
    public static implicit operator PipelineTransitionTaskExecutable<TCurrentContext, TNewContext>(SynchronousHeadlessTransitionTask<TCurrentContext, TNewContext> task) => new(task, (ctx, _) => new ValueTask<TNewContext>(task(ctx)));
    public static implicit operator PipelineTransitionTaskExecutable<TCurrentContext, TNewContext>(Func<Task<TNewContext>> task) => new(task, (_, _) => new ValueTask<TNewContext>(task()));
    public static implicit operator PipelineTransitionTaskExecutable<TCurrentContext, TNewContext>(Func<ValueTask<TNewContext>> task) => new(task, (_, _) => task());
    public static implicit operator PipelineTransitionTaskExecutable<TCurrentContext, TNewContext>(Func<TNewContext> task) => new(task, (_, _) => new ValueTask<TNewContext>(task()));
}