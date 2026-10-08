using Shiki.Common.Result;
using Shiki.TaskPipeline.Context;

namespace Shiki.TaskPipeline;

public interface ITaskStage
{
    /// <summary>
    /// Total amount of tasks stored in the task stage
    /// </summary>
    int Count { get; }
    
    /// <summary>
    /// All tasks in the task stage
    /// </summary>
    IEnumerable<Func<ITaskContext, ITaskExecutionContext, ValueTask>> Tasks { get; }

    /// <summary>
    /// Transitions from one context to another
    /// </summary>
    /// <param name="ctx">The context to transition from</param>
    /// <param name="execCtx">The execution context</param>
    /// <returns>The new context</returns>
    Task<ITaskContext> TransitionAsync(ITaskContext ctx, ITaskExecutionContext execCtx);

    /// <summary>
    /// Tries to insert the given task at a position that is before the given target task
    /// </summary>
    /// <param name="targetTask">The task to find</param>
    /// <param name="newTask">The new task to insert</param>
    /// <param name="executorTask">Middleman that calls the task</param>
    /// <returns>Whether the insertion was successful</returns>
    BooleanResult<Exception> TryInsertBefore(Delegate targetTask, Delegate newTask, Delegate executorTask);
    
    /// <summary>
    /// Tries to insert the given task at a position that is before the given target task
    /// </summary>
    /// <param name="targetTask">The task to find</param>
    /// <param name="newTask">The new task to insert</param>
    /// <returns>Whether the insertion was successful</returns>
    BooleanResult<Exception> TryInsertBefore(Delegate targetTask, IPipelineTaskExecutable newTask) => TryInsertBefore(targetTask, newTask.Runnable, newTask.Executor);
    
    /// <summary>
    /// Tries to insert the given task at a position that is after the given target task
    /// </summary>
    /// <param name="targetTask">The task to find</param>
    /// <param name="newTask">The new task to insert</param>
    /// <param name="executorTask">Middleman that calls the task</param>
    /// <returns>Whether the insertion was successful</returns>
    BooleanResult<Exception> TryInsertAfter(Delegate targetTask, Delegate newTask, Delegate executorTask);
    
    /// <summary>
    /// Tries to insert the given task at a position that is after the given target task
    /// </summary>
    /// <param name="targetTask">The task to find</param>
    /// <param name="newTask">The new task to insert</param>
    /// <returns>Whether the insertion was successful</returns>
    BooleanResult<Exception> TryInsertAfter(Delegate targetTask, IPipelineTaskExecutable newTask) => TryInsertAfter(targetTask, newTask.Runnable, newTask.Executor);
    
    /// <summary>
    /// Tries to insert the given task at the start of the stage
    /// </summary>
    /// <param name="task">The task to insert</param>
    /// <param name="executorTask">Middleman that calls the task</param>
    /// <returns>Whether the insertion was successful</returns>
    BooleanResult<Exception> TryInsertAtStart(Delegate task, Delegate executorTask);
    
    /// <summary>
    /// Tries to insert the given task at the start of the stage
    /// </summary>
    /// <param name="task">The task to insert</param>
    /// <returns>Whether the insertion was successful</returns>
    BooleanResult<Exception> TryInsertAtStart(IPipelineTaskExecutable task) => TryInsertAtStart(task.Runnable, task.Executor);
    
    /// <summary>
    /// Tries to insert the given task at the end of the stage
    /// </summary>
    /// <param name="task">The task to insert</param>
    /// <param name="executorTask">Middleman that calls the task</param>
    /// <returns>Whether the insertion was successful</returns>
    BooleanResult<Exception> TryInsertAtEnd(Delegate task, Delegate executorTask);
    
    /// <summary>
    /// Tries to insert the given task at the end of the stage
    /// </summary>
    /// <param name="task">The task to insert</param>
    /// <returns>Whether the insertion was successful</returns>
    BooleanResult<Exception> TryInsertAtEnd(IPipelineTaskExecutable task) => TryInsertAtEnd(task.Runnable, task.Executor);
    
    /// <summary>
    /// Tries to replace the target task with the given task
    /// </summary>
    /// <param name="targetTask">The target to replace</param>
    /// <param name="newTask">The task to replace it with</param>
    /// <param name="executorTask">Middleman that calls the task</param>
    /// <returns>Whether the replacement was successful</returns>
    BooleanResult<Exception> TryReplace(Delegate targetTask, Delegate newTask, Delegate executorTask);
    
    /// <summary>
    /// Tries to replace the target task with the given task
    /// </summary>
    /// <param name="targetTask">The target to replace</param>
    /// <param name="newTask">The task to replace it with</param>
    /// <returns>Whether the replacement was successful</returns>
    BooleanResult<Exception> TryReplace(Delegate targetTask, IPipelineTaskExecutable newTask) => TryReplace(targetTask, newTask.Runnable, newTask.Executor);
    
    /// <summary>
    /// Tries to remove the target task
    /// </summary>
    /// <param name="targetTask">The task to remove</param>
    /// <returns>Whether the removal was successful</returns>
    BooleanResult<Exception> TryRemove(Delegate targetTask);
    
    /// <summary>
    /// Tries to remove the target task
    /// </summary>
    /// <param name="targetTask">The task to remove</param>
    /// <returns>Whether the removal was successful</returns>
    BooleanResult<Exception> TryRemove(IPipelineTaskExecutable targetTask) => TryRemove(targetTask.Runnable);
}