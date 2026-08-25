namespace Shiki.TaskPipeline;

public class TaskStage<TOldContext, TContext>(Func<TOldContext, TContext> transition) : ITaskStage
    where TOldContext : class, ITaskContext
    where TContext : class, ITaskContext
{
    private readonly List<Func<TContext, CancellationToken, Task>> _tasks = [];

    public int Count => _tasks.Count;

    public IEnumerable<Func<ITaskContext, CancellationToken, Task>> Tasks
    {
        get
        {
            foreach (var task in _tasks)
                yield return (ctx, ct) => task((TContext)ctx, ct);
        }
    }

    public void Add(Func<TContext, CancellationToken, Task> task) => _tasks.Add(task);
    public ITaskContext Transition(ITaskContext old) => old is not TOldContext ctx
                                                                    ? throw new InvalidCastException()
                                                                    : transition(ctx);
}