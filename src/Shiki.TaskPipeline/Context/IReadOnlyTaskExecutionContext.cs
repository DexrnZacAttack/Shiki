using R3;

namespace Shiki.TaskPipeline.Context;

/// <summary>
/// Read-only pipeline execution context
///
/// Usually exposed as a public property for status tracking
/// </summary>
public interface IReadOnlyTaskExecutionContext
{
    /// <summary>
    /// Current status message
    /// </summary>
    IReadOnlyBindableReactiveProperty<string> CurrentStatus { get; }
    
    /// <summary>
    /// Current progress
    /// </summary>
    IReadOnlyBindableReactiveProperty<double> CurrentProgress { get; }
}