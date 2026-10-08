namespace Shiki.TaskPipeline.Context;

/// <summary>
/// Base task context interface
/// </summary>
public interface ITaskContext;

/// <summary>
/// Task context that supports direct conversion from an old context type instance
/// </summary>
/// <typeparam name="TSelf">Self</typeparam>
/// <typeparam name="TOldContext">The context type being transitioned from</typeparam>
public interface ITransitionableTaskContext<out TSelf, in TOldContext> : ITaskContext
    where TSelf : ITransitionableTaskContext<TSelf, TOldContext>, ITaskContext
    where TOldContext : ITaskContext
{
    /// <summary>
    /// Constructs TSelf from an old context type instance
    /// </summary>
    /// <param name="oldContext">The old context</param>
    /// <returns>The new context</returns>
    static abstract TSelf TransitionFrom(TOldContext oldContext);
}