using R3;

namespace Shiki.TaskPipeline;

public class TaskPipeline
{
    //TODO allow for custom return type on RunAsync for the last context to be an IValuedTaskContext
    //TODO allow combining task status, maybe move into own class and pass it around?
    public BindableReactiveProperty<double> Progress { get; } = new(0.0);
    public BindableReactiveProperty<string> Status { get; } = new("Starting");

    private readonly List<ITaskStage> _stages = [];
    
    internal TaskPipeline() {}//only allow builder to call ctor

    internal void Add(ITaskStage stage) => _stages.Add(stage);

    public async Task RunAsync(ITaskContext initial, CancellationToken ct = default)
    {
        ITaskContext currentContext = initial;
        IDisposable? subscription = null;
        
        int tasks = _stages.Sum(s => s.Count);
        int completed = 0;
        try
        {
            foreach (ITaskStage stage in _stages)
            {
                currentContext = stage.Transition(currentContext);

                subscription?.Dispose(); //make sure old is disposed otherwise we will leak subscriptions
                //todo see comment in itaskcontext
                subscription = currentContext.Status.Subscribe(s => Status.Value = s);

                foreach (var task in stage.Tasks)
                {
                    ct.ThrowIfCancellationRequested();
                    
                    await task(currentContext, ct);

                    completed++;
                    Progress.Value = (completed * 100) / tasks;
                }
            }
        }
        catch (Exception ex)
        {
            Status.Value = "Failed! Check the logs for more info.";
            
            throw;
        }
        finally
        {
            subscription?.Dispose();
        }
    }
}