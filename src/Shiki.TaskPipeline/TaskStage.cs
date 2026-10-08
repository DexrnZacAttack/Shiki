using Shiki.Common.Extensions;
using Shiki.Common.Result;
using Shiki.TaskPipeline.Context;

namespace Shiki.TaskPipeline;

/// <summary>
/// A task stage
///
/// Holds the actual tasks, and defines what context type is supported for said tasks.
/// </summary>
/// <param name="transition">The </param>
/// <typeparam name="TOldContext"></typeparam>
/// <typeparam name="TCurrentContext"></typeparam>
public class TaskStage<TOldContext, TCurrentContext>(PipelineTransitionTaskExecutable<TOldContext, TCurrentContext> transition) : ITaskStage
where TOldContext : ITaskContext
where TCurrentContext : ITaskContext
{
    /// <summary>
    /// Transitions from an old context to the new one.
    /// </summary>
    public delegate TCurrentContext ContextTransitioner(TOldContext oldCtx);
    
    /// <summary>
    /// Tasks in the task stage
    /// </summary>
    private readonly List<PipelineTaskExecutable<TCurrentContext>> _tasks = [];

    /// <inheritdoc/>
    public int Count => _tasks.Count;

    /// <inheritdoc/>
    public IEnumerable<Func<ITaskContext, ITaskExecutionContext, ValueTask>> Tasks
    {
        get
        {
            foreach (var task in _tasks)
                yield return (ctx, ectx) => task.Executor((TCurrentContext)ctx, ectx);
        }
    }
    
    public void Add(PipelineTask<TCurrentContext> task) => _tasks.Add(task);
    public void Add(PipelineTaskExecutable<TCurrentContext> task) => _tasks.Add(task);
    
    public async Task<TCurrentContext> TransitionAsync(TOldContext oldCtx, ITaskExecutionContext execCtx) => await transition.Executor(oldCtx, execCtx);
    
    public async Task<ITaskContext> TransitionAsync(ITaskContext ctx, ITaskExecutionContext execCtx) => await transition.Executor((TOldContext)ctx, execCtx) ?? throw new InvalidOperationException();

    public BooleanResult<Exception> TryInsertBefore(Delegate targetTask, Delegate newTask, Delegate executorTask)
    {
        int idx = _tasks.FindIndex(ft => ft.Runnable.Method == targetTask.Method && ft.Runnable.Target == targetTask.Target);
        if (idx == -1)
            return new KeyNotFoundException("Could not find the given task").WithCurrentStackTrace();
        
        if (executorTask is not PipelineValueTask<TCurrentContext> et)
            return new InvalidCastException("Given executor task is not of the correct type").WithCurrentStackTrace();
        
        _tasks.Insert(idx, new PipelineTaskExecutable<TCurrentContext>(newTask, et));

        return BooleanResult<Exception>.Successful;
    }

    public BooleanResult<Exception> TryInsertAfter(Delegate targetTask,
                                                   Delegate newTask, Delegate executorTask)
    {
        int idx = _tasks.FindIndex(ft => ft.Runnable.Method == targetTask.Method && ft.Runnable.Target == targetTask.Target);
        if (idx == -1)
            return new KeyNotFoundException("Could not find the given task").WithCurrentStackTrace();
        
        if (executorTask is not PipelineValueTask<TCurrentContext> et)
            return new InvalidCastException("Given executor task is not of the correct type").WithCurrentStackTrace();
        
        _tasks.Insert(idx + 1, new PipelineTaskExecutable<TCurrentContext>(newTask, et));

        return BooleanResult<Exception>.Successful;
    }

    public BooleanResult<Exception> TryInsertAtStart(Delegate task, Delegate executorTask)
    {
        if (executorTask is not PipelineValueTask<TCurrentContext> et)
            return new InvalidCastException("Given executor task is not of the correct type").WithCurrentStackTrace();
        
        _tasks.Insert(0, new PipelineTaskExecutable<TCurrentContext>(task, et));
        return BooleanResult<Exception>.Successful;
    }

    public BooleanResult<Exception> TryInsertAtEnd(Delegate task, Delegate executorTask)
    {
        if (executorTask is not PipelineValueTask<TCurrentContext> et)
            return new InvalidCastException("Given executor task is not of the correct type").WithCurrentStackTrace();
        
        _tasks.Insert(_tasks.Count, new PipelineTaskExecutable<TCurrentContext>(task, et));
        return BooleanResult<Exception>.Successful;
    }

    public BooleanResult<Exception> TryReplace(Delegate targetTask, Delegate newTask, Delegate executorTask)
    {
        int idx = _tasks.FindIndex(ft => ft.Runnable.Method == targetTask.Method && ft.Runnable.Target == targetTask.Target);
        if (idx == -1)
            return new KeyNotFoundException("Could not find the given task").WithCurrentStackTrace();
        
        if (executorTask is not PipelineValueTask<TCurrentContext> et)
            return new InvalidCastException("Given executor task is not of the correct type").WithCurrentStackTrace();

        _tasks[idx] = new PipelineTaskExecutable<TCurrentContext>(newTask, et);

        return BooleanResult<Exception>.Successful;
    }

    public BooleanResult<Exception> TryRemove(Delegate targetTask)
    {
        int idx = _tasks.FindIndex(ft => ft.Runnable.Method == targetTask.Method && ft.Runnable.Target == targetTask.Target);
        if (idx == -1)
            return new KeyNotFoundException("Could not find the given task").WithCurrentStackTrace();
        
        _tasks.RemoveAt(idx);
        return BooleanResult<Exception>.Successful;
    }
}