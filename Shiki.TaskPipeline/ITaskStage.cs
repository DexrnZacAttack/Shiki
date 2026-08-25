namespace Shiki.TaskPipeline;

public interface ITaskStage
{
    int Count { get; }
    IEnumerable<Func<ITaskContext, CancellationToken, Task>> Tasks { get; }

    ITaskContext Transition(ITaskContext old);
}