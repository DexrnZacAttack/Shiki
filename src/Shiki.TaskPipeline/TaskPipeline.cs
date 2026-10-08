using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Shiki.TaskPipeline.Context;

namespace Shiki.TaskPipeline;

/// <summary>
/// A class that allows you to run a set of Stages, each containing their own context and tasks
/// 
/// Additionally, active state info (i.e. current progress, status text) can be tracked (and managed by tasks) inside the TaskPipelineExecutionState
/// </summary>
public class TaskPipeline<TInitialContext, TFinalContext>
where TInitialContext : ITaskContext
where TFinalContext : ITaskContext
{
    /// <summary>
    /// Current execution state
    /// </summary>
    public IReadOnlyTaskExecutionContext ExecutionState => _executionState;
    
    /// <summary>
    /// Execution state
    /// </summary>
    private readonly TaskPipelineExecutionState _executionState;
    
    /// <summary>
    /// Stages that the TaskPipeline calls into
    /// </summary>
    private readonly List<ITaskStage> _stages;

    /// <summary>
    /// Creates a TaskPipeline with a list of stages and exec state
    /// </summary>
    /// <param name="stages">List of stages</param>
    /// <param name="executionState">Initial execution state reference</param>
    internal TaskPipeline(List<ITaskStage> stages, TaskPipelineExecutionState executionState)//only allow builder to call ctor
    {
        _stages = stages;
        _executionState = executionState;
    }
    
    /// <summary>
    /// Starts the pipeline
    /// </summary>
    /// <param name="initial">The initial context instance</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>The final TaskContext when running is finished</returns>
    public async Task<TFinalContext> RunAsync(TInitialContext initial, CancellationToken ct = default)
    {
        _executionState.CancellationToken = ct;
        
        ITaskContext currentContext = initial;
        
        int totalTasks = _stages.Sum(s => s.Count);
        int completedTasks = 0;
        
        _executionState.Progress.Value = 0;
        _executionState.Status.Value = "Starting pipeline...";
        
        try
        {
            foreach (ITaskStage stage in _stages)
            {
                currentContext = await stage.TransitionAsync(currentContext, _executionState);

                foreach (var task in stage.Tasks)
                {
                    ct.ThrowIfCancellationRequested();
                    
                    await task(currentContext, _executionState);

                    completedTasks++;
                    if (totalTasks > 0)
                    {
                        _executionState.Progress.Value = (completedTasks * 100.0) / totalTasks;
                    }
                }
            }
            
            return (TFinalContext)currentContext;
        }
        catch (Exception ex)
        {
            _executionState.Status.Value = $"Failed! Exception thrown: {ex.Message}";
            
            throw;
        }
    }
}