using R3;
using Shiki.TaskPipeline.Context;

namespace Shiki.TaskPipeline;

/// <summary>
/// Execution state of the pipeline
///
/// Each task in a TaskPipeline is given an instance of this class to allow for status modifications.
/// </summary>
public class TaskPipelineExecutionState : ITaskExecutionContext
{
    /// <inheritdoc/>
    public BindableReactiveProperty<string> Status { get; } = new(string.Empty);
    /// <inheritdoc/>
    public BindableReactiveProperty<double> Progress { get; } = new(0);
    /// <inheritdoc/>
    public CancellationToken CancellationToken { get; set; }
    
    /// <inheritdoc/>
    public IReadOnlyBindableReactiveProperty<string> CurrentStatus => Status;
    /// <inheritdoc/>
    public IReadOnlyBindableReactiveProperty<double> CurrentProgress => Progress;
}