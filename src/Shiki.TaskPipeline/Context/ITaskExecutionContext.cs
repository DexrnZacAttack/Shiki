using R3;

namespace Shiki.TaskPipeline.Context;

/// <summary>
/// Pipeline execution context
/// </summary>
public interface ITaskExecutionContext : IReadOnlyTaskExecutionContext
{
    /// <summary>
    /// The status message
    /// </summary>
    BindableReactiveProperty<string> Status { get; }
    /// <summary>
    /// Current progress, note that for tasks that may take a while, it is acceptable to set this to 0 and track progress yourself
    ///
    /// After the task ends, the TaskPipeline will set the progress back to what it should be.
    /// </summary>
    BindableReactiveProperty<double> Progress { get; }

    /// <summary>
    /// Shared task pipeline cancellation token
    /// </summary>
    CancellationToken CancellationToken { get; }
}