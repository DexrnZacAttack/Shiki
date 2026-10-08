namespace Shiki.TaskPipeline.Builder;

/// <summary>
/// Simple interface to help with implementing methods with all the different task delegate types
/// </summary>
/// <typeparam name="TSelf">Self</typeparam>
/// <typeparam name="TTaskDelegate">The task delegate type</typeparam>
public interface ITaskPipelineBuilderAcceptedTaskVariant<out TSelf, in TTaskDelegate>
    where TSelf : ITaskPipelineBuilderAcceptedTaskVariant<TSelf, TTaskDelegate>
{
    /// <summary>
    /// Queues a task to run in the current stage
    /// </summary>
    /// <param name="task">The task to run</param>
    /// <returns>Self</returns>
    TSelf ThenRun(TTaskDelegate task);
}

/// <summary>
/// Simple interface to help with implementing methods with all the different task delegate types
/// </summary>
/// <typeparam name="TSelf">Self</typeparam>
/// <typeparam name="TTaskDelegate">The task delegate type</typeparam>
public interface ITaskPipelineBuilderAcceptedAsyncTaskVariant<out TSelf, in TTaskDelegate>
    where TSelf : ITaskPipelineBuilderAcceptedAsyncTaskVariant<TSelf, TTaskDelegate>
{
    /// <summary>
    /// Queues a task to run in the current stage
    /// </summary>
    /// <param name="task">The task to run</param>
    /// <returns>Self</returns>
    TSelf ThenRunAsync(TTaskDelegate task);
}