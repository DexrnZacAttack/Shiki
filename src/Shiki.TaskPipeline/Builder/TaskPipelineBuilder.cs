using Shiki.Common.Result;
using Shiki.TaskPipeline.Context;

namespace Shiki.TaskPipeline.Builder;

/// <summary>
/// Builder for a TaskPipeline
/// </summary>
/// <typeparam name="TInitialContext">The initial context to start out with</typeparam>
/// <typeparam name="TCurrentContext">The current context of the TaskPipelineBuilder. For users, this should just be the same as TInitialContext when constructing.</typeparam>
public sealed class TaskPipelineBuilder<TInitialContext, TCurrentContext> : ITaskPipelineBuilder<TaskPipelineBuilder<TInitialContext, TCurrentContext>, TInitialContext, TCurrentContext>
    where TInitialContext : ITaskContext
    where TCurrentContext : ITaskContext
{
    /// <summary>
    /// All registered stages
    /// </summary>
    private readonly List<ITaskStage> _stages;
    /// <summary>
    /// Current stage that this builder is iterating upon
    /// </summary>
    private readonly TaskStage<ITaskContext, TCurrentContext> _currentStage;
    
    /// <summary>
    /// Current execution state, shared with the built TaskPipeline
    /// </summary>
    public IReadOnlyTaskExecutionContext ExecutionState => _executionState;
    
    /// <summary>
    /// The execution state, shared with the built TaskPipeline
    /// </summary>
    private readonly TaskPipelineExecutionState _executionState = new();
    
    /// <summary>
    /// Creates a new TaskPipelineBuilder
    /// </summary>
    public TaskPipelineBuilder()
    {
        _currentStage = new TaskStage<ITaskContext, TCurrentContext>((SynchronousTransitionTask<ITaskContext, TCurrentContext>)((ctx, _) => (TCurrentContext)ctx));
        _stages = [_currentStage];
    }
    
    /// <summary>
    /// Creates a new TaskPipelineBuilder with preset stages
    ///
    /// Used for deriving from a previous TaskPipelineBuilder in the chain
    /// </summary>
    /// <param name="stages">Task stages</param>
    /// <param name="current">Current task stage</param>
    private TaskPipelineBuilder(List<ITaskStage> stages, TaskStage<ITaskContext, TCurrentContext> current)
    {
        _stages = stages;
        _currentStage = current;
    }

    /// <summary>
    /// Builds the TaskPipeline
    /// </summary>
    /// <returns>The built TaskPipeline</returns>
    public TaskPipeline<TInitialContext, TCurrentContext> Build() 
    {
        return new TaskPipeline<TInitialContext, TCurrentContext>([.. _stages], _executionState);
    }
    
    #region Run
    /// <summary>
    /// Queues a task to run in the current stage
    /// </summary>
    /// <param name="task">The task to run</param>
    /// <returns>Self</returns>
    private TaskPipelineBuilder<TInitialContext, TCurrentContext> ThenRunExecutable(PipelineTaskExecutable<TCurrentContext> task)
    {
        _currentStage.Add(task);
        return this;
    }
    
    /// <inheritdoc />
    public TaskPipelineBuilder<TInitialContext, TCurrentContext> ThenRunAsync(PipelineTask<TCurrentContext> task) => ThenRunExecutable(task);
    /// <inheritdoc />
    public TaskPipelineBuilder<TInitialContext, TCurrentContext> ThenRunAsync(HeadlessPipelineTask<TCurrentContext> task) => ThenRunExecutable(task);
    /// <inheritdoc />
    public TaskPipelineBuilder<TInitialContext, TCurrentContext> ThenRunAsync(PipelineValueTask<TCurrentContext> task) => ThenRunExecutable(task);
    /// <inheritdoc />
    public TaskPipelineBuilder<TInitialContext, TCurrentContext> ThenRunAsync(HeadlessPipelineValueTask<TCurrentContext> task) => ThenRunExecutable(task);
    /// <inheritdoc />
    public TaskPipelineBuilder<TInitialContext, TCurrentContext> ThenRun(SynchronousPipelineTask<TCurrentContext> task) => ThenRunExecutable(task);
    /// <inheritdoc />
    public TaskPipelineBuilder<TInitialContext, TCurrentContext> ThenRun(SynchronousHeadlessPipelineTask<TCurrentContext> task) => ThenRunExecutable(task);
    /// <inheritdoc />
    public TaskPipelineBuilder<TInitialContext, TCurrentContext> ThenRunAsync(Func<Task> task) => ThenRunExecutable(task);
    /// <inheritdoc />
    public TaskPipelineBuilder<TInitialContext, TCurrentContext> ThenRunAsync(Func<ValueTask> task) => ThenRunExecutable(task);
    /// <inheritdoc />
    public TaskPipelineBuilder<TInitialContext, TCurrentContext> ThenRun(Action task) => ThenRunExecutable(task);
    #endregion

    #region Transition
    //idk better word than transition im too sleepy
    /// <inheritdoc />
    public TaskPipelineBuilder<TInitialContext, TNewContext> ThenTransitionTo<TNewContext>()
        where TNewContext : ITransitionableTaskContext<TNewContext, TCurrentContext>
    {
        var s = new TaskStage<ITaskContext, TNewContext>((PipelineTransitionTaskExecutable<ITaskContext, TNewContext>)((ctx, _) => TNewContext.TransitionFrom((TCurrentContext)ctx)));
        _stages.Add(s);

        return new TaskPipelineBuilder<TInitialContext, TNewContext>(_stages, s);
    }
    
    /// <summary>
    /// Runs a task that is expected to return the new context
    /// </summary>
    /// <param name="task">The task</param>
    /// <typeparam name="TNewContext">The new context type</typeparam>
    /// <returns>Self</returns>
    private TaskPipelineBuilder<TInitialContext, TNewContext> ThenRunExecutableToTransitionTo<TNewContext>(PipelineTransitionTaskExecutable<TCurrentContext, TNewContext> task)
        where TNewContext : ITaskContext
    {
        var s = new TaskStage<ITaskContext, TNewContext>((TransitionValueTask<ITaskContext, TNewContext>)((ctx, execCtx) => task.Executor((TCurrentContext)ctx, execCtx)));
        _stages.Add(s);

        return new TaskPipelineBuilder<TInitialContext, TNewContext>(_stages, s);
    }

    /// <inheritdoc />
    public TaskPipelineBuilder<TInitialContext, TNewContext> ThenRunToTransitionToAsync<TNewContext>(
        TransitionTask<TCurrentContext, TNewContext> transition)
        where TNewContext : ITaskContext => ThenRunExecutableToTransitionTo<TNewContext>(transition);
    
    /// <inheritdoc />
    public TaskPipelineBuilder<TInitialContext, TNewContext> ThenRunToTransitionToAsync<TNewContext>(
        HeadlessTransitionTask<TCurrentContext, TNewContext> transition)
        where TNewContext : ITaskContext => ThenRunExecutableToTransitionTo<TNewContext>(transition);
    
    /// <inheritdoc />
    public TaskPipelineBuilder<TInitialContext, TNewContext> ThenRunToTransitionToAsync<TNewContext>(
        TransitionValueTask<TCurrentContext, TNewContext> transition)
        where TNewContext : ITaskContext => ThenRunExecutableToTransitionTo<TNewContext>(transition);
    
    /// <inheritdoc />
    public TaskPipelineBuilder<TInitialContext, TNewContext> ThenRunToTransitionToAsync<TNewContext>(
        HeadlessTransitionValueTask<TCurrentContext, TNewContext> transition)
        where TNewContext : ITaskContext => ThenRunExecutableToTransitionTo<TNewContext>(transition);
    
    /// <inheritdoc />
    public TaskPipelineBuilder<TInitialContext, TNewContext> ThenRunToTransitionTo<TNewContext>(
        SynchronousTransitionTask<TCurrentContext, TNewContext> transition)
        where TNewContext : ITaskContext => ThenRunExecutableToTransitionTo<TNewContext>(transition);
    
    /// <inheritdoc />
    public TaskPipelineBuilder<TInitialContext, TNewContext> ThenRunToTransitionTo<TNewContext>(
        SynchronousHeadlessTransitionTask<TCurrentContext, TNewContext> transition)
        where TNewContext : ITaskContext => ThenRunExecutableToTransitionTo<TNewContext>(transition);    
    
    /// <inheritdoc />
    public TaskPipelineBuilder<TInitialContext, TNewContext> ThenRunToTransitionToAsync<TNewContext>(
        Func<Task<TNewContext>> transition)
        where TNewContext : ITaskContext => ThenRunExecutableToTransitionTo<TNewContext>(transition);
    
    /// <inheritdoc />
    public TaskPipelineBuilder<TInitialContext, TNewContext> ThenRunToTransitionToAsync<TNewContext>(
        Func<ValueTask<TNewContext>> transition)
        where TNewContext : ITaskContext => ThenRunExecutableToTransitionTo<TNewContext>(transition);
    
    /// <inheritdoc />
    public TaskPipelineBuilder<TInitialContext, TNewContext> ThenRunToTransitionTo<TNewContext>(
        Func<TNewContext> transition)
        where TNewContext : ITaskContext => ThenRunExecutableToTransitionTo<TNewContext>(transition);
    #endregion
    
    #region Insertion,Removal,Replacement

    /// <summary>
    /// Just performs the insert/whatever action and handles errors
    /// </summary>
    /// <param name="action">The mutation action</param>
    /// <returns>Whether the action was successful</returns>
    private BooleanResult<Exception> TryPerformInsert(Func<ITaskStage, BooleanResult<Exception>> action)
    {
        foreach (ITaskStage taskStage in _stages)
        {
            var res = action(taskStage);
            switch (res.Error)
            {
                case KeyNotFoundException:
                    continue;
                case null:
                    return BooleanResult<Exception>.Successful;
                default:
                    return res;
            }
        }
    
        return new KeyNotFoundException();
    }
    
    #region Insert After
    /// <summary>
    /// Inserts a new task AFTER the target task
    /// </summary>
    /// <param name="targetTask">The task to find and insert after</param>
    /// <param name="newTask">The new task to insert</param>
    /// <typeparam name="TTargetContext">The context type where both the target task and the new task will be present</typeparam>
    /// <returns>A result of whether the operation succeeded</returns>
    private BooleanResult<Exception> TryInsertExecutableAfter<TTargetContext>(Delegate targetTask, PipelineTaskExecutable<TTargetContext> newTask)
    where TTargetContext : ITaskContext => TryPerformInsert(stage => stage.TryInsertAfter(targetTask, newTask));
        
    /// <inheritdoc />
    public BooleanResult<Exception> TryInsertAfter<TTargetContext>(Delegate targetTask, PipelineTask<TTargetContext> newTask)
        where TTargetContext : ITaskContext => TryInsertExecutableAfter<TTargetContext>(targetTask, newTask);
                    
    /// <inheritdoc />
    public BooleanResult<Exception> TryInsertAfter<TTargetContext>(Delegate targetTask, HeadlessPipelineTask<TTargetContext> newTask)
        where TTargetContext : ITaskContext => TryInsertExecutableAfter<TTargetContext>(targetTask, newTask);
    
    /// <inheritdoc />
    public BooleanResult<Exception> TryInsertAfter<TTargetContext>(Delegate targetTask, PipelineValueTask<TTargetContext> newTask)
        where TTargetContext : ITaskContext => TryInsertExecutableAfter<TTargetContext>(targetTask, newTask);
                    
    /// <inheritdoc />
    public BooleanResult<Exception> TryInsertAfter<TTargetContext>(Delegate targetTask, HeadlessPipelineValueTask<TTargetContext> newTask)
        where TTargetContext : ITaskContext => TryInsertExecutableAfter<TTargetContext>(targetTask, newTask);
                  
    /// <inheritdoc />
    public BooleanResult<Exception> TryInsertAfter<TTargetContext>(Delegate targetTask, SynchronousPipelineTask<TTargetContext> newTask)
        where TTargetContext : ITaskContext => TryInsertExecutableAfter<TTargetContext>(targetTask, newTask);
             
    /// <inheritdoc />
    public BooleanResult<Exception> TryInsertAfter<TTargetContext>(Delegate targetTask, SynchronousHeadlessPipelineTask<TTargetContext> newTask)
        where TTargetContext : ITaskContext => TryInsertExecutableAfter<TTargetContext>(targetTask, newTask);
                   
    /// <inheritdoc />
    public BooleanResult<Exception> TryInsertAfter<TTargetContext>(Delegate targetTask, Func<Task> newTask)
        where TTargetContext : ITaskContext => TryInsertExecutableAfter<TTargetContext>(targetTask, newTask);
                                      
    /// <inheritdoc />
    public BooleanResult<Exception> TryInsertAfter<TTargetContext>(Delegate targetTask, Func<ValueTask> newTask)
        where TTargetContext : ITaskContext => TryInsertExecutableAfter<TTargetContext>(targetTask, newTask);
                   
    /// <inheritdoc />
    public BooleanResult<Exception> TryInsertAfter<TTargetContext>(Delegate targetTask, Action newTask)
        where TTargetContext : ITaskContext => TryInsertExecutableAfter<TTargetContext>(targetTask, newTask);
    #endregion

    #region Insert Before
    /// <summary>
    /// Inserts a new task BEFORE the target task
    /// </summary>
    /// <param name="targetTask">The task to find and insert after</param>
    /// <param name="newTask">The new task to insert</param>
    /// <typeparam name="TTargetContext">The context type where both the target task and the new task will be present</typeparam>
    /// <returns>A result of whether the operation succeeded</returns>
    private BooleanResult<Exception> TryInsertExecutableBefore<TTargetContext>(Delegate targetTask, PipelineTaskExecutable<TTargetContext> newTask)
    where TTargetContext : ITaskContext => TryPerformInsert(stage => stage.TryInsertBefore(targetTask, newTask));
              
    /// <inheritdoc />
    public BooleanResult<Exception> TryInsertBefore<TTargetContext>(Delegate targetTask, PipelineTask<TTargetContext> newTask)
    where TTargetContext : ITaskContext => TryInsertExecutableBefore<TTargetContext>(targetTask, newTask);
                 
    /// <inheritdoc />
    public BooleanResult<Exception> TryInsertBefore<TTargetContext>(Delegate targetTask, HeadlessPipelineTask<TTargetContext> newTask)
    where TTargetContext : ITaskContext => TryInsertExecutableBefore<TTargetContext>(targetTask, newTask);
    
    /// <inheritdoc />
    public BooleanResult<Exception> TryInsertBefore<TTargetContext>(Delegate targetTask, PipelineValueTask<TTargetContext> newTask)
        where TTargetContext : ITaskContext => TryInsertExecutableBefore<TTargetContext>(targetTask, newTask);
                 
    /// <inheritdoc />
    public BooleanResult<Exception> TryInsertBefore<TTargetContext>(Delegate targetTask, HeadlessPipelineValueTask<TTargetContext> newTask)
        where TTargetContext : ITaskContext => TryInsertExecutableBefore<TTargetContext>(targetTask, newTask);
                      
    /// <inheritdoc />
    public BooleanResult<Exception> TryInsertBefore<TTargetContext>(Delegate targetTask, SynchronousPipelineTask<TTargetContext> newTask)
    where TTargetContext : ITaskContext => TryInsertExecutableBefore<TTargetContext>(targetTask, newTask);
                 
    /// <inheritdoc />
    public BooleanResult<Exception> TryInsertBefore<TTargetContext>(Delegate targetTask, SynchronousHeadlessPipelineTask<TTargetContext> newTask)
    where TTargetContext : ITaskContext => TryInsertExecutableBefore<TTargetContext>(targetTask, newTask);
             
    /// <inheritdoc />
    public BooleanResult<Exception> TryInsertBefore<TTargetContext>(Delegate targetTask, Func<Task> newTask)
    where TTargetContext : ITaskContext => TryInsertExecutableBefore<TTargetContext>(targetTask, newTask);
                              
    /// <inheritdoc />
    public BooleanResult<Exception> TryInsertBefore<TTargetContext>(Delegate targetTask, Func<ValueTask> newTask)
    where TTargetContext : ITaskContext => TryInsertExecutableBefore<TTargetContext>(targetTask, newTask);
                 
    /// <inheritdoc />
    public BooleanResult<Exception> TryInsertBefore<TTargetContext>(Delegate targetTask, Action newTask)
    where TTargetContext : ITaskContext => TryInsertExecutableBefore<TTargetContext>(targetTask, newTask);
    #endregion    
    
    #region Insert at start
    /// <summary>
    /// Inserts a new task at the start of the context stage
    /// </summary>
    /// <param name="task">The new task to insert</param>
    /// <typeparam name="TTargetContext">The context type where the new task will be inserted</typeparam>
    /// <returns>A result of whether the operation succeeded</returns>
    private BooleanResult<Exception> TryInsertExecutableAtStart<TTargetContext>(PipelineTaskExecutable<TTargetContext> task)
    where TTargetContext : ITaskContext => TryPerformInsert(stage => stage.TryInsertAtStart(task));

    /// <inheritdoc />
    public BooleanResult<Exception> TryInsertAtStart<TTargetContext>(PipelineTask<TTargetContext> task)
        where TTargetContext : ITaskContext => TryInsertExecutableAtStart<TTargetContext>(task);
        
    /// <inheritdoc />
    public BooleanResult<Exception> TryInsertAtStart<TTargetContext>(HeadlessPipelineTask<TTargetContext> task)
        where TTargetContext : ITaskContext => TryInsertExecutableAtStart<TTargetContext>(task);
    
    /// <inheritdoc />
    public BooleanResult<Exception> TryInsertAtStart<TTargetContext>(PipelineValueTask<TTargetContext> task)
        where TTargetContext : ITaskContext => TryInsertExecutableAtStart<TTargetContext>(task);
        
    /// <inheritdoc />
    public BooleanResult<Exception> TryInsertAtStart<TTargetContext>(HeadlessPipelineValueTask<TTargetContext> task)
        where TTargetContext : ITaskContext => TryInsertExecutableAtStart<TTargetContext>(task);
        
    /// <inheritdoc />
    public BooleanResult<Exception> TryInsertAtStart<TTargetContext>(SynchronousPipelineTask<TTargetContext> task)
        where TTargetContext : ITaskContext => TryInsertExecutableAtStart<TTargetContext>(task);
        
    /// <inheritdoc />
    public BooleanResult<Exception> TryInsertAtStart<TTargetContext>(SynchronousHeadlessPipelineTask<TTargetContext> task)
        where TTargetContext : ITaskContext => TryInsertExecutableAtStart<TTargetContext>(task);
        
    /// <inheritdoc />
    public BooleanResult<Exception> TryInsertAtStart<TTargetContext>(Func<Task> task)
        where TTargetContext : ITaskContext => TryInsertExecutableAtStart<TTargetContext>(task);
                
    /// <inheritdoc />
    public BooleanResult<Exception> TryInsertAtStart<TTargetContext>(Func<ValueTask> task)
        where TTargetContext : ITaskContext => TryInsertExecutableAtStart<TTargetContext>(task);
        
    /// <inheritdoc />
    public BooleanResult<Exception> TryInsertAtStart<TTargetContext>(Action task)
        where TTargetContext : ITaskContext => TryInsertExecutableAtStart<TTargetContext>(task);
    #endregion    
    
    #region Insert at end
    /// <summary>
    /// Inserts a new task at the end of the context stage
    /// </summary>
    /// <param name="task">The new task to insert</param>
    /// <typeparam name="TTargetContext">The context type where the new task will be inserted</typeparam>
    /// <returns>A result of whether the operation succeeded</returns>
    private BooleanResult<Exception> TryInsertExecutableAtEnd<TTargetContext>(PipelineTaskExecutable<TTargetContext> task)
    where TTargetContext : ITaskContext => TryPerformInsert(stage => stage.TryInsertAtEnd(task));
                
    /// <inheritdoc />
    public BooleanResult<Exception> TryInsertAtEnd<TTargetContext>(PipelineTask<TTargetContext> task)
    where TTargetContext : ITaskContext => TryInsertExecutableAtEnd<TTargetContext>(task);
                      
    /// <inheritdoc />
    public BooleanResult<Exception> TryInsertAtEnd<TTargetContext>(HeadlessPipelineTask<TTargetContext> task)
    where TTargetContext : ITaskContext => TryInsertExecutableAtEnd<TTargetContext>(task);
    
    /// <inheritdoc />
    public BooleanResult<Exception> TryInsertAtEnd<TTargetContext>(PipelineValueTask<TTargetContext> task)
        where TTargetContext : ITaskContext => TryInsertExecutableAtEnd<TTargetContext>(task);
                      
    /// <inheritdoc />
    public BooleanResult<Exception> TryInsertAtEnd<TTargetContext>(HeadlessPipelineValueTask<TTargetContext> task)
        where TTargetContext : ITaskContext => TryInsertExecutableAtEnd<TTargetContext>(task);
                
    /// <inheritdoc />
    public BooleanResult<Exception> TryInsertAtEnd<TTargetContext>(SynchronousPipelineTask<TTargetContext> task)
    where TTargetContext : ITaskContext => TryInsertExecutableAtEnd<TTargetContext>(task);
                 
    /// <inheritdoc />
    public BooleanResult<Exception> TryInsertAtEnd<TTargetContext>(SynchronousHeadlessPipelineTask<TTargetContext> task)
    where TTargetContext : ITaskContext => TryInsertExecutableAtEnd<TTargetContext>(task);
              
    /// <inheritdoc />
    public BooleanResult<Exception> TryInsertAtEnd<TTargetContext>(Func<Task> task)
    where TTargetContext : ITaskContext => TryInsertExecutableAtEnd<TTargetContext>(task);
                            
    /// <inheritdoc />
    public BooleanResult<Exception> TryInsertAtEnd<TTargetContext>(Func<ValueTask> task)
    where TTargetContext : ITaskContext => TryInsertExecutableAtEnd<TTargetContext>(task);
              
    /// <inheritdoc />
    public BooleanResult<Exception> TryInsertAtEnd<TTargetContext>(Action task)
    where TTargetContext : ITaskContext => TryInsertExecutableAtEnd<TTargetContext>(task);
    #endregion    
    
    #region Replace
    /// <summary>
    /// Replaces the target task with a new task
    /// </summary>
    /// <param name="targetTask">The target task to replace</param>
    /// <param name="newTask">The new task that is replacing the target</param>
    /// <typeparam name="TTargetContext">The context type where the target task is present</typeparam>
    /// <returns>A result of whether the operation succeeded</returns>
    private BooleanResult<Exception> TryReplaceWithExecutable<TTargetContext>(Delegate targetTask, PipelineTaskExecutable<TTargetContext> newTask)
    where TTargetContext : ITaskContext => TryPerformInsert(stage => stage.TryReplace(targetTask, newTask));
                
    /// <inheritdoc />
    public BooleanResult<Exception> TryReplace<TTargetContext>(Delegate targetTask, PipelineTask<TTargetContext> newTask)
    where TTargetContext : ITaskContext => TryReplaceWithExecutable<TTargetContext>(targetTask, newTask);
               
    /// <inheritdoc />
    public BooleanResult<Exception> TryReplace<TTargetContext>(Delegate targetTask, HeadlessPipelineTask<TTargetContext> newTask)
    where TTargetContext : ITaskContext => TryReplaceWithExecutable<TTargetContext>(targetTask, newTask);
    
    /// <inheritdoc />
    public BooleanResult<Exception> TryReplace<TTargetContext>(Delegate targetTask, PipelineValueTask<TTargetContext> newTask)
        where TTargetContext : ITaskContext => TryReplaceWithExecutable<TTargetContext>(targetTask, newTask);
               
    /// <inheritdoc />
    public BooleanResult<Exception> TryReplace<TTargetContext>(Delegate targetTask, HeadlessPipelineValueTask<TTargetContext> newTask)
        where TTargetContext : ITaskContext => TryReplaceWithExecutable<TTargetContext>(targetTask, newTask);
                
    /// <inheritdoc />
    public BooleanResult<Exception> TryReplace<TTargetContext>(Delegate targetTask, SynchronousPipelineTask<TTargetContext> newTask)
    where TTargetContext : ITaskContext => TryReplaceWithExecutable<TTargetContext>(targetTask, newTask);
                
    /// <inheritdoc />
    public BooleanResult<Exception> TryReplace<TTargetContext>(Delegate targetTask, SynchronousHeadlessPipelineTask<TTargetContext> newTask)
    where TTargetContext : ITaskContext => TryReplaceWithExecutable<TTargetContext>(targetTask, newTask);
                    
    /// <inheritdoc />
    public BooleanResult<Exception> TryReplace<TTargetContext>(Delegate targetTask, Func<Task> newTask)
    where TTargetContext : ITaskContext => TryReplaceWithExecutable<TTargetContext>(targetTask, newTask);
                        
    /// <inheritdoc />
    public BooleanResult<Exception> TryReplace<TTargetContext>(Delegate targetTask, Func<ValueTask> newTask)
    where TTargetContext : ITaskContext => TryReplaceWithExecutable<TTargetContext>(targetTask, newTask);
    
    /// <inheritdoc />
    public BooleanResult<Exception> TryReplace<TTargetContext>(Delegate targetTask, Action newTask) where TTargetContext : ITaskContext => TryReplaceWithExecutable<TTargetContext>(targetTask, newTask);
    #endregion    
    
    /// <inheritdoc />
    public BooleanResult<Exception> TryRemove(Delegate targetTask) => TryPerformInsert(stage => stage.TryRemove(targetTask));
    #endregion
}

/// <summary>
/// Helper class for creating a normal TaskPipelineBuilder
/// </summary>
/// <typeparam name="TInitialContext">The initial context</typeparam>
public static class TaskPipelineBuilder<TInitialContext>
    where TInitialContext : ITaskContext
{
    /// <summary>
    /// Creates a new TaskPipelineBuilder
    /// </summary>
    /// <returns>The new TaskPipelineBuilder</returns>
    public static TaskPipelineBuilder<TInitialContext, TInitialContext> Create() => new();
}