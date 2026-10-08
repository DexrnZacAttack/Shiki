using Shiki.Common.Result;
using Shiki.TaskPipeline.Context;

namespace Shiki.TaskPipeline.Builder;

/// <summary>
/// Interface for TaskPipelineBuilder
/// </summary>
/// <typeparam name="TSelf">Self</typeparam>
/// <typeparam name="TInitialContext">The initial context</typeparam>
/// <typeparam name="TCurrentContext">The current context</typeparam>
public interface ITaskPipelineBuilder<out TSelf, TInitialContext, out TCurrentContext>
    : ITaskPipelineBuilderAcceptedAsyncTaskVariant<TSelf, PipelineTask<TCurrentContext>>,
      ITaskPipelineBuilderAcceptedAsyncTaskVariant<TSelf, HeadlessPipelineTask<TCurrentContext>>,
      ITaskPipelineBuilderAcceptedTaskVariant<TSelf, SynchronousPipelineTask<TCurrentContext>>,
      ITaskPipelineBuilderAcceptedTaskVariant<TSelf, SynchronousHeadlessPipelineTask<TCurrentContext>>,
      ITaskPipelineBuilderAcceptedAsyncTaskVariant<TSelf, PipelineValueTask<TCurrentContext>>,
      ITaskPipelineBuilderAcceptedAsyncTaskVariant<TSelf, HeadlessPipelineValueTask<TCurrentContext>>,
      ITaskPipelineBuilderAcceptedAsyncTaskVariant<TSelf, Func<Task>>,
      ITaskPipelineBuilderAcceptedAsyncTaskVariant<TSelf, Func<ValueTask>>,
      ITaskPipelineBuilderAcceptedTaskVariant<TSelf, Action>

    where TSelf : ITaskPipelineBuilder<TSelf, TInitialContext, TCurrentContext>
    where TInitialContext : ITaskContext
    where TCurrentContext : ITaskContext
{
    /// <summary>
    /// Transitions to another context via direct conversion of the current context
    /// </summary>
    /// <typeparam name="TNewContext">The new context type</typeparam>
    /// <returns>Self</returns>
    TaskPipelineBuilder<TInitialContext, TNewContext> ThenTransitionTo<TNewContext>()
        where TNewContext : ITransitionableTaskContext<TNewContext, TCurrentContext>;
    
    /// <summary>
    /// Runs a task that is expected to return the new context
    /// </summary>
    /// <param name="transition">The task</param>
    /// <typeparam name="TNewContext">The new context type</typeparam>
    /// <returns>Self</returns>
    TaskPipelineBuilder<TInitialContext, TNewContext> ThenRunToTransitionToAsync<TNewContext>(TransitionTask<TCurrentContext, TNewContext> transition) where TNewContext : ITaskContext;
    /// <summary>
    /// Runs a task that is expected to return the new context
    /// </summary>
    /// <param name="transition">The task</param>
    /// <typeparam name="TNewContext">The new context type</typeparam>
    /// <returns>Self</returns>
    TaskPipelineBuilder<TInitialContext, TNewContext> ThenRunToTransitionToAsync<TNewContext>(HeadlessTransitionTask<TCurrentContext, TNewContext> transition) where TNewContext : ITaskContext;
    /// <summary>
    /// Runs a task that is expected to return the new context
    /// </summary>
    /// <param name="transition">The task</param>
    /// <typeparam name="TNewContext">The new context type</typeparam>
    /// <returns>Self</returns>
    TaskPipelineBuilder<TInitialContext, TNewContext> ThenRunToTransitionToAsync<TNewContext>(TransitionValueTask<TCurrentContext, TNewContext> transition) where TNewContext : ITaskContext;
    /// <summary>
    /// Runs a task that is expected to return the new context
    /// </summary>
    /// <param name="transition">The task</param>
    /// <typeparam name="TNewContext">The new context type</typeparam>
    /// <returns>Self</returns>
    TaskPipelineBuilder<TInitialContext, TNewContext> ThenRunToTransitionToAsync<TNewContext>(HeadlessTransitionValueTask<TCurrentContext, TNewContext> transition) where TNewContext : ITaskContext;
    /// <summary>
    /// Runs a task that is expected to return the new context
    /// </summary>
    /// <param name="transition">The task</param>
    /// <typeparam name="TNewContext">The new context type</typeparam>
    /// <returns>Self</returns>
    TaskPipelineBuilder<TInitialContext, TNewContext> ThenRunToTransitionTo<TNewContext>(SynchronousTransitionTask<TCurrentContext, TNewContext> transition) where TNewContext : ITaskContext;
    /// <summary>
    /// Runs a task that is expected to return the new context
    /// </summary>
    /// <param name="transition">The task</param>
    /// <typeparam name="TNewContext">The new context type</typeparam>
    /// <returns>Self</returns>
    TaskPipelineBuilder<TInitialContext, TNewContext> ThenRunToTransitionTo<TNewContext>(SynchronousHeadlessTransitionTask<TCurrentContext, TNewContext> transition) where TNewContext : ITaskContext;
    /// <summary>
    /// Runs a task that is expected to return the new context
    /// </summary>
    /// <param name="transition">The task</param>
    /// <typeparam name="TNewContext">The new context type</typeparam>
    /// <returns>Self</returns>
    TaskPipelineBuilder<TInitialContext, TNewContext> ThenRunToTransitionTo<TNewContext>(Func<TNewContext> transition) where TNewContext : ITaskContext;
    /// <summary>
    /// Runs a task that is expected to return the new context
    /// </summary>
    /// <param name="transition">The task</param>
    /// <typeparam name="TNewContext">The new context type</typeparam>
    /// <returns>Self</returns>
    TaskPipelineBuilder<TInitialContext, TNewContext> ThenRunToTransitionToAsync<TNewContext>(Func<Task<TNewContext>> transition) where TNewContext : ITaskContext;
    /// <summary>
    /// Runs a task that is expected to return the new context
    /// </summary>
    /// <param name="transition">The task</param>
    /// <typeparam name="TNewContext">The new context type</typeparam>
    /// <returns>Self</returns>
    TaskPipelineBuilder<TInitialContext, TNewContext> ThenRunToTransitionToAsync<TNewContext>(Func<ValueTask<TNewContext>> transition) where TNewContext : ITaskContext;
    
    #region Insertion,Removal,Replacement
    #region Insert After

    /// <summary>
    /// Inserts a new task AFTER the target task
    /// </summary>
    /// <param name="targetTask">The task to find and insert after</param>
    /// <param name="newTask">The new task to insert</param>
    /// <typeparam name="TTargetContext">The context type where both the target task and the new task will be present</typeparam>
    /// <returns>A result of whether the operation succeeded</returns>
    BooleanResult<Exception> TryInsertAfter<TTargetContext>(Delegate targetTask, PipelineTask<TTargetContext> newTask)
        where TTargetContext : ITaskContext;

    /// <summary>
    /// Inserts a new task AFTER the target task
    /// </summary>
    /// <param name="targetTask">The task to find and insert after</param>
    /// <param name="newTask">The new task to insert</param>
    /// <typeparam name="TTargetContext">The context type where both the target task and the new task will be present</typeparam>
    /// <returns>A result of whether the operation succeeded</returns>
    BooleanResult<Exception> TryInsertAfter<TTargetContext>(Delegate targetTask, HeadlessPipelineTask<TTargetContext> newTask)
        where TTargetContext : ITaskContext;
    
    /// <summary>
    /// Inserts a new task AFTER the target task
    /// </summary>
    /// <param name="targetTask">The task to find and insert after</param>
    /// <param name="newTask">The new task to insert</param>
    /// <typeparam name="TTargetContext">The context type where both the target task and the new task will be present</typeparam>
    /// <returns>A result of whether the operation succeeded</returns>
    BooleanResult<Exception> TryInsertAfter<TTargetContext>(Delegate targetTask, PipelineValueTask<TTargetContext> newTask)
        where TTargetContext : ITaskContext;

    /// <summary>
    /// Inserts a new task AFTER the target task
    /// </summary>
    /// <param name="targetTask">The task to find and insert after</param>
    /// <param name="newTask">The new task to insert</param>
    /// <typeparam name="TTargetContext">The context type where both the target task and the new task will be present</typeparam>
    /// <returns>A result of whether the operation succeeded</returns>
    BooleanResult<Exception> TryInsertAfter<TTargetContext>(Delegate targetTask, HeadlessPipelineValueTask<TTargetContext> newTask)
        where TTargetContext : ITaskContext;

    /// <summary>
    /// Inserts a new task AFTER the target task
    /// </summary>
    /// <param name="targetTask">The task to find and insert after</param>
    /// <param name="newTask">The new task to insert</param>
    /// <typeparam name="TTargetContext">The context type where both the target task and the new task will be present</typeparam>
    /// <returns>A result of whether the operation succeeded</returns>
    BooleanResult<Exception> TryInsertAfter<TTargetContext>(Delegate targetTask, SynchronousPipelineTask<TTargetContext> newTask)
        where TTargetContext : ITaskContext;

    /// <summary>
    /// Inserts a new task AFTER the target task
    /// </summary>
    /// <param name="targetTask">The task to find and insert after</param>
    /// <param name="newTask">The new task to insert</param>
    /// <typeparam name="TTargetContext">The context type where both the target task and the new task will be present</typeparam>
    /// <returns>A result of whether the operation succeeded</returns>
    BooleanResult<Exception> TryInsertAfter<TTargetContext>(Delegate targetTask, SynchronousHeadlessPipelineTask<TTargetContext> newTask)
        where TTargetContext : ITaskContext;

    /// <summary>
    /// Inserts a new task AFTER the target task
    /// </summary>
    /// <param name="targetTask">The task to find and insert after</param>
    /// <param name="newTask">The new task to insert</param>
    /// <typeparam name="TTargetContext">The context type where both the target task and the new task will be present</typeparam>
    /// <returns>A result of whether the operation succeeded</returns>
    BooleanResult<Exception> TryInsertAfter<TTargetContext>(Delegate targetTask, Func<Task> newTask)
        where TTargetContext : ITaskContext;
    
    /// <summary>
    /// Inserts a new task AFTER the target task
    /// </summary>
    /// <param name="targetTask">The task to find and insert after</param>
    /// <param name="newTask">The new task to insert</param>
    /// <typeparam name="TTargetContext">The context type where both the target task and the new task will be present</typeparam>
    /// <returns>A result of whether the operation succeeded</returns>
    BooleanResult<Exception> TryInsertAfter<TTargetContext>(Delegate targetTask, Func<ValueTask> newTask)
        where TTargetContext : ITaskContext;

    /// <summary>
    /// Inserts a new task AFTER the target task
    /// </summary>
    /// <param name="targetTask">The task to find and insert after</param>
    /// <param name="newTask">The new task to insert</param>
    /// <typeparam name="TTargetContext">The context type where both the target task and the new task will be present</typeparam>
    /// <returns>A result of whether the operation succeeded</returns>
    BooleanResult<Exception> TryInsertAfter<TTargetContext>(Delegate targetTask, Action newTask)
        where TTargetContext : ITaskContext;
    #endregion

    #region Insert Before

    /// <summary>
    /// Inserts a new task BEFORE the target task
    /// </summary>
    /// <param name="targetTask">The task to find and insert before</param>
    /// <param name="newTask">The new task to insert</param>
    /// <typeparam name="TTargetContext">The context type where both the target task and the new task will be present</typeparam>
    /// <returns>A result of whether the operation succeeded</returns>
    BooleanResult<Exception> TryInsertBefore<TTargetContext>(Delegate targetTask, PipelineTask<TTargetContext> newTask)
        where TTargetContext : ITaskContext;

    /// <summary>
    /// Inserts a new task BEFORE the target task
    /// </summary>
    /// <param name="targetTask">The task to find and insert before</param>
    /// <param name="newTask">The new task to insert</param>
    /// <typeparam name="TTargetContext">The context type where both the target task and the new task will be present</typeparam>
    /// <returns>A result of whether the operation succeeded</returns>
    BooleanResult<Exception> TryInsertBefore<TTargetContext>(Delegate targetTask, HeadlessPipelineTask<TTargetContext> newTask)
        where TTargetContext : ITaskContext;
    
    /// <summary>
    /// Inserts a new task BEFORE the target task
    /// </summary>
    /// <param name="targetTask">The task to find and insert before</param>
    /// <param name="newTask">The new task to insert</param>
    /// <typeparam name="TTargetContext">The context type where both the target task and the new task will be present</typeparam>
    /// <returns>A result of whether the operation succeeded</returns>
    BooleanResult<Exception> TryInsertBefore<TTargetContext>(Delegate targetTask, PipelineValueTask<TTargetContext> newTask)
        where TTargetContext : ITaskContext;

    /// <summary>
    /// Inserts a new task BEFORE the target task
    /// </summary>
    /// <param name="targetTask">The task to find and insert before</param>
    /// <param name="newTask">The new task to insert</param>
    /// <typeparam name="TTargetContext">The context type where both the target task and the new task will be present</typeparam>
    /// <returns>A result of whether the operation succeeded</returns>
    BooleanResult<Exception> TryInsertBefore<TTargetContext>(Delegate targetTask, HeadlessPipelineValueTask<TTargetContext> newTask)
        where TTargetContext : ITaskContext;


    /// <summary>
    /// Inserts a new task BEFORE the target task
    /// </summary>
    /// <param name="targetTask">The task to find and insert before</param>
    /// <param name="newTask">The new task to insert</param>
    /// <typeparam name="TTargetContext">The context type where both the target task and the new task will be present</typeparam>
    /// <returns>A result of whether the operation succeeded</returns>
    BooleanResult<Exception> TryInsertBefore<TTargetContext>(Delegate targetTask, SynchronousPipelineTask<TTargetContext> newTask)
        where TTargetContext : ITaskContext;

    /// <summary>
    /// Inserts a new task BEFORE the target task
    /// </summary>
    /// <param name="targetTask">The task to find and insert before</param>
    /// <param name="newTask">The new task to insert</param>
    /// <typeparam name="TTargetContext">The context type where both the target task and the new task will be present</typeparam>
    /// <returns>A result of whether the operation succeeded</returns>
    BooleanResult<Exception> TryInsertBefore<TTargetContext>(Delegate targetTask, SynchronousHeadlessPipelineTask<TTargetContext> newTask)
        where TTargetContext : ITaskContext;

    /// <summary>
    /// Inserts a new task BEFORE the target task
    /// </summary>
    /// <param name="targetTask">The task to find and insert before</param>
    /// <param name="newTask">The new task to insert</param>
    /// <typeparam name="TTargetContext">The context type where both the target task and the new task will be present</typeparam>
    /// <returns>A result of whether the operation succeeded</returns>
    BooleanResult<Exception> TryInsertBefore<TTargetContext>(Delegate targetTask, Func<Task> newTask)
        where TTargetContext : ITaskContext;
    
    /// <summary>
    /// Inserts a new task BEFORE the target task
    /// </summary>
    /// <param name="targetTask">The task to find and insert before</param>
    /// <param name="newTask">The new task to insert</param>
    /// <typeparam name="TTargetContext">The context type where both the target task and the new task will be present</typeparam>
    /// <returns>A result of whether the operation succeeded</returns>
    BooleanResult<Exception> TryInsertBefore<TTargetContext>(Delegate targetTask, Func<ValueTask> newTask)
        where TTargetContext : ITaskContext;

    /// <summary>
    /// Inserts a new task BEFORE the target task
    /// </summary>
    /// <param name="targetTask">The task to find and insert before</param>
    /// <param name="newTask">The new task to insert</param>
    /// <typeparam name="TTargetContext">The context type where both the target task and the new task will be present</typeparam>
    /// <returns>A result of whether the operation succeeded</returns>
    BooleanResult<Exception> TryInsertBefore<TTargetContext>(Delegate targetTask, Action newTask)
        where TTargetContext : ITaskContext;
    #endregion    
    
    #region Insert at start

    /// <summary>
    /// Inserts a new task at the start of the context stage
    /// </summary>
    /// <param name="task">The new task to insert</param>
    /// <typeparam name="TTargetContext">The context type where the new task will be inserted</typeparam>
    /// <returns>A result of whether the operation succeeded</returns>
    BooleanResult<Exception> TryInsertAtStart<TTargetContext>(PipelineTask<TTargetContext> task)
        where TTargetContext : ITaskContext;

    /// <summary>
    /// Inserts a new task at the start of the context stage
    /// </summary>
    /// <param name="task">The new task to insert</param>
    /// <typeparam name="TTargetContext">The context type where the new task will be inserted</typeparam>
    /// <returns>A result of whether the operation succeeded</returns>
    BooleanResult<Exception> TryInsertAtStart<TTargetContext>(HeadlessPipelineTask<TTargetContext> task)
        where TTargetContext : ITaskContext;
    
    /// <summary>
    /// Inserts a new task at the start of the context stage
    /// </summary>
    /// <param name="task">The new task to insert</param>
    /// <typeparam name="TTargetContext">The context type where the new task will be inserted</typeparam>
    /// <returns>A result of whether the operation succeeded</returns>
    BooleanResult<Exception> TryInsertAtStart<TTargetContext>(PipelineValueTask<TTargetContext> task)
        where TTargetContext : ITaskContext;

    /// <summary>
    /// Inserts a new task at the start of the context stage
    /// </summary>
    /// <param name="task">The new task to insert</param>
    /// <typeparam name="TTargetContext">The context type where the new task will be inserted</typeparam>
    /// <returns>A result of whether the operation succeeded</returns>
    BooleanResult<Exception> TryInsertAtStart<TTargetContext>(HeadlessPipelineValueTask<TTargetContext> task)
        where TTargetContext : ITaskContext;

    /// <summary>
    /// Inserts a new task at the start of the context stage
    /// </summary>
    /// <param name="task">The new task to insert</param>
    /// <typeparam name="TTargetContext">The context type where the new task will be inserted</typeparam>
    /// <returns>A result of whether the operation succeeded</returns>
    BooleanResult<Exception> TryInsertAtStart<TTargetContext>(SynchronousPipelineTask<TTargetContext> task)
        where TTargetContext : ITaskContext;

    /// <summary>
    /// Inserts a new task at the start of the context stage
    /// </summary>
    /// <param name="task">The new task to insert</param>
    /// <typeparam name="TTargetContext">The context type where the new task will be inserted</typeparam>
    /// <returns>A result of whether the operation succeeded</returns>
    BooleanResult<Exception> TryInsertAtStart<TTargetContext>(SynchronousHeadlessPipelineTask<TTargetContext> task)
        where TTargetContext : ITaskContext;

    /// <summary>
    /// Inserts a new task at the start of the context stage
    /// </summary>
    /// <param name="task">The new task to insert</param>
    /// <typeparam name="TTargetContext">The context type where the new task will be inserted</typeparam>
    /// <returns>A result of whether the operation succeeded</returns>
    BooleanResult<Exception> TryInsertAtStart<TTargetContext>(Func<Task> task)
        where TTargetContext : ITaskContext;
    
    /// <summary>
    /// Inserts a new task at the start of the context stage
    /// </summary>
    /// <param name="task">The new task to insert</param>
    /// <typeparam name="TTargetContext">The context type where the new task will be inserted</typeparam>
    /// <returns>A result of whether the operation succeeded</returns>
    BooleanResult<Exception> TryInsertAtStart<TTargetContext>(Func<ValueTask> task)
        where TTargetContext : ITaskContext;

    /// <summary>
    /// Inserts a new task at the start of the context stage
    /// </summary>
    /// <param name="task">The new task to insert</param>
    /// <typeparam name="TTargetContext">The context type where the new task will be inserted</typeparam>
    /// <returns>A result of whether the operation succeeded</returns>
    BooleanResult<Exception> TryInsertAtStart<TTargetContext>(Action task)
        where TTargetContext : ITaskContext;
    #endregion    
    
    #region Insert at end

    /// <summary>
    /// Inserts a new task at the end of the context stage
    /// </summary>
    /// <param name="task">The new task to insert</param>
    /// <typeparam name="TTargetContext">The context type where the new task will be inserted</typeparam>
    /// <returns>A result of whether the operation succeeded</returns>
    BooleanResult<Exception> TryInsertAtEnd<TTargetContext>(PipelineTask<TTargetContext> task)
        where TTargetContext : ITaskContext;
    
    /// <summary>
    /// Inserts a new task at the end of the context stage
    /// </summary>
    /// <param name="task">The new task to insert</param>
    /// <typeparam name="TTargetContext">The context type where the new task will be inserted</typeparam>
    /// <returns>A result of whether the operation succeeded</returns>
    BooleanResult<Exception> TryInsertAtEnd<TTargetContext>(HeadlessPipelineTask<TTargetContext> task)
        where TTargetContext : ITaskContext;
    
    /// <summary>
    /// Inserts a new task at the end of the context stage
    /// </summary>
    /// <param name="task">The new task to insert</param>
    /// <typeparam name="TTargetContext">The context type where the new task will be inserted</typeparam>
    /// <returns>A result of whether the operation succeeded</returns>
    BooleanResult<Exception> TryInsertAtEnd<TTargetContext>(PipelineValueTask<TTargetContext> task)
        where TTargetContext : ITaskContext;
    
    /// <summary>
    /// Inserts a new task at the end of the context stage
    /// </summary>
    /// <param name="task">The new task to insert</param>
    /// <typeparam name="TTargetContext">The context type where the new task will be inserted</typeparam>
    /// <returns>A result of whether the operation succeeded</returns>
    BooleanResult<Exception> TryInsertAtEnd<TTargetContext>(HeadlessPipelineValueTask<TTargetContext> task)
        where TTargetContext : ITaskContext;

    /// <summary>
    /// Inserts a new task at the end of the context stage
    /// </summary>
    /// <param name="task">The new task to insert</param>
    /// <typeparam name="TTargetContext">The context type where the new task will be inserted</typeparam>
    /// <returns>A result of whether the operation succeeded</returns>
    BooleanResult<Exception> TryInsertAtEnd<TTargetContext>(SynchronousPipelineTask<TTargetContext> task)
        where TTargetContext : ITaskContext;

    /// <summary>
    /// Inserts a new task at the end of the context stage
    /// </summary>
    /// <param name="task">The new task to insert</param>
    /// <typeparam name="TTargetContext">The context type where the new task will be inserted</typeparam>
    /// <returns>A result of whether the operation succeeded</returns>
    BooleanResult<Exception> TryInsertAtEnd<TTargetContext>(SynchronousHeadlessPipelineTask<TTargetContext> task)
        where TTargetContext : ITaskContext;

    /// <summary>
    /// Inserts a new task at the end of the context stage
    /// </summary>
    /// <param name="task">The new task to insert</param>
    /// <typeparam name="TTargetContext">The context type where the new task will be inserted</typeparam>
    /// <returns>A result of whether the operation succeeded</returns>
    BooleanResult<Exception> TryInsertAtEnd<TTargetContext>(Func<Task> task)
        where TTargetContext : ITaskContext;
    
    /// <summary>
    /// Inserts a new task at the end of the context stage
    /// </summary>
    /// <param name="task">The new task to insert</param>
    /// <typeparam name="TTargetContext">The context type where the new task will be inserted</typeparam>
    /// <returns>A result of whether the operation succeeded</returns>
    BooleanResult<Exception> TryInsertAtEnd<TTargetContext>(Func<ValueTask> task)
        where TTargetContext : ITaskContext;

    /// <summary>
    /// Inserts a new task at the end of the context stage
    /// </summary>
    /// <param name="task">The new task to insert</param>
    /// <typeparam name="TTargetContext">The context type where the new task will be inserted</typeparam>
    /// <returns>A result of whether the operation succeeded</returns>
    BooleanResult<Exception> TryInsertAtEnd<TTargetContext>(Action task)
        where TTargetContext : ITaskContext;
    #endregion    
    
    #region Replace

    /// <summary>
    /// Replaces the target task with a new task
    /// </summary>
    /// <param name="targetTask">The target task to replace</param>
    /// <param name="newTask">The new task that is replacing the target</param>
    /// <typeparam name="TTargetContext">The context type where the target task is present</typeparam>
    /// <returns>A result of whether the operation succeeded</returns>
    BooleanResult<Exception> TryReplace<TTargetContext>(Delegate targetTask, PipelineTask<TTargetContext> newTask)
        where TTargetContext : ITaskContext;

    /// <summary>
    /// Replaces the target task with a new task
    /// </summary>
    /// <param name="targetTask">The target task to replace</param>
    /// <param name="newTask">The new task that is replacing the target</param>
    /// <typeparam name="TTargetContext">The context type where the target task is present</typeparam>
    /// <returns>A result of whether the operation succeeded</returns>
    BooleanResult<Exception> TryReplace<TTargetContext>(Delegate targetTask, HeadlessPipelineTask<TTargetContext> newTask)
        where TTargetContext : ITaskContext;
    
    /// <summary>
    /// Replaces the target task with a new task
    /// </summary>
    /// <param name="targetTask">The target task to replace</param>
    /// <param name="newTask">The new task that is replacing the target</param>
    /// <typeparam name="TTargetContext">The context type where the target task is present</typeparam>
    /// <returns>A result of whether the operation succeeded</returns>
    BooleanResult<Exception> TryReplace<TTargetContext>(Delegate targetTask, PipelineValueTask<TTargetContext> newTask)
        where TTargetContext : ITaskContext;

    /// <summary>
    /// Replaces the target task with a new task
    /// </summary>
    /// <param name="targetTask">The target task to replace</param>
    /// <param name="newTask">The new task that is replacing the target</param>
    /// <typeparam name="TTargetContext">The context type where the target task is present</typeparam>
    /// <returns>A result of whether the operation succeeded</returns>
    BooleanResult<Exception> TryReplace<TTargetContext>(Delegate targetTask, HeadlessPipelineValueTask<TTargetContext> newTask)
        where TTargetContext : ITaskContext;

    /// <summary>
    /// Replaces the target task with a new task
    /// </summary>
    /// <param name="targetTask">The target task to replace</param>
    /// <param name="newTask">The new task that is replacing the target</param>
    /// <typeparam name="TTargetContext">The context type where the target task is present</typeparam>
    /// <returns>A result of whether the operation succeeded</returns>
    BooleanResult<Exception> TryReplace<TTargetContext>(Delegate targetTask, SynchronousPipelineTask<TTargetContext> newTask)
        where TTargetContext : ITaskContext;

    /// <summary>
    /// Replaces the target task with a new task
    /// </summary>
    /// <param name="targetTask">The target task to replace</param>
    /// <param name="newTask">The new task that is replacing the target</param>
    /// <typeparam name="TTargetContext">The context type where the target task is present</typeparam>
    /// <returns>A result of whether the operation succeeded</returns>
    BooleanResult<Exception> TryReplace<TTargetContext>(Delegate targetTask, SynchronousHeadlessPipelineTask<TTargetContext> newTask)
        where TTargetContext : ITaskContext;

    /// <summary>
    /// Replaces the target task with a new task
    /// </summary>
    /// <param name="targetTask">The target task to replace</param>
    /// <param name="newTask">The new task that is replacing the target</param>
    /// <typeparam name="TTargetContext">The context type where the target task is present</typeparam>
    /// <returns>A result of whether the operation succeeded</returns>
    BooleanResult<Exception> TryReplace<TTargetContext>(Delegate targetTask, Func<Task> newTask)
        where TTargetContext : ITaskContext;
    
    /// <summary>
    /// Replaces the target task with a new task
    /// </summary>
    /// <param name="targetTask">The target task to replace</param>
    /// <param name="newTask">The new task that is replacing the target</param>
    /// <typeparam name="TTargetContext">The context type where the target task is present</typeparam>
    /// <returns>A result of whether the operation succeeded</returns>
    BooleanResult<Exception> TryReplace<TTargetContext>(Delegate targetTask, Func<ValueTask> newTask)
        where TTargetContext : ITaskContext;

    /// <summary>
    /// Replaces the target task with a new task
    /// </summary>
    /// <param name="targetTask">The target task to replace</param>
    /// <param name="newTask">The new task that is replacing the target</param>
    /// <typeparam name="TTargetContext">The context type where the target task is present</typeparam>
    /// <returns>A result of whether the operation succeeded</returns>
    BooleanResult<Exception> TryReplace<TTargetContext>(Delegate targetTask, Action newTask) where TTargetContext : ITaskContext;
    #endregion

    /// <summary>
    /// Removes a task from all stages, if found.
    /// </summary>
    /// <param name="targetTask">The task to remove</param>
    /// <returns>A result of whether the operation succeeded</returns>
    BooleanResult<Exception> TryRemove(Delegate targetTask);
    #endregion
}