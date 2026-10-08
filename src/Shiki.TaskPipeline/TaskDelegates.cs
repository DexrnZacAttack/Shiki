using Shiki.TaskPipeline.Context;

namespace Shiki.TaskPipeline;

/// <summary>
/// A normal pipeline task
/// </summary>
public delegate Task PipelineTask<in TContext>(TContext ctx, ITaskExecutionContext execCtx)
    where TContext : ITaskContext;

/// <summary>
/// A pipeline task that does not require the current execution context
/// </summary>
public delegate Task HeadlessPipelineTask<in TContext>(TContext ctx)
    where TContext : ITaskContext;

/// <summary>
/// A synchronous pipeline task
/// </summary>
public delegate void SynchronousPipelineTask<in TContext>(TContext ctx, ITaskExecutionContext execCtx)
    where TContext : ITaskContext;

/// <summary>
/// A synchronous pipeline task that does not require the current execution context
/// </summary>
public delegate void SynchronousHeadlessPipelineTask<in TContext>(TContext ctx)
    where TContext : ITaskContext;

/// <summary>
/// Task that also returns a new context (i.e. transitioning from the old ctx to the new one)
/// </summary>
public delegate Task<TNewContext> TransitionTask<in TCurrentContext, TNewContext>(TCurrentContext oldCtx, ITaskExecutionContext execCtx)
    where TCurrentContext : ITaskContext
    where TNewContext : ITaskContext;
    
/// <summary>
/// Task that also returns a new context (i.e. transitioning from the old ctx to the new one)
/// </summary>
public delegate Task<TNewContext> HeadlessTransitionTask<in TCurrentContext, TNewContext>(TCurrentContext oldCtx)
    where TCurrentContext : ITaskContext
    where TNewContext : ITaskContext;

/// <summary>
/// Task that also returns a new context (i.e. transitioning from the old ctx to the new one)
/// </summary>
public delegate TNewContext SynchronousTransitionTask<in TCurrentContext, out TNewContext>(TCurrentContext oldCtx, ITaskExecutionContext execCtx)
    where TCurrentContext : ITaskContext
    where TNewContext : ITaskContext;

/// <summary>
/// Task that also returns a new context (i.e. transitioning from the old ctx to the new one)
/// </summary>
public delegate TNewContext SynchronousHeadlessTransitionTask<in TCurrentContext, out TNewContext>(TCurrentContext oldCtx)
    where TCurrentContext : ITaskContext
    where TNewContext : ITaskContext;

/// <summary>
/// Normal pipeline valuetask
/// </summary>
public delegate ValueTask PipelineValueTask<in TContext>(TContext ctx, ITaskExecutionContext execCtx)
    where TContext : ITaskContext;

/// <summary>
/// A pipeline valuetask that does not require the current execution context
/// </summary>
public delegate ValueTask HeadlessPipelineValueTask<in TContext>(TContext ctx)
    where TContext : ITaskContext;

/// <summary>
/// Task that also returns a new context (i.e. transitioning from the old ctx to the new one)
/// </summary>
public delegate ValueTask<TNewContext> TransitionValueTask<in TCurrentContext, TNewContext>(TCurrentContext oldCtx, ITaskExecutionContext execCtx)
    where TCurrentContext : ITaskContext
    where TNewContext : ITaskContext;
    
/// <summary>
/// Task that also returns a new context (i.e. transitioning from the old ctx to the new one)
/// </summary>
public delegate ValueTask<TNewContext> HeadlessTransitionValueTask<in TCurrentContext, TNewContext>(TCurrentContext oldCtx)
    where TCurrentContext : ITaskContext
    where TNewContext : ITaskContext;
