namespace Shiki.Common.Util;

/// <summary>
/// Calls a given task every TimeSpan tick
/// </summary>
/// <param name="timeSpan">The timespan</param>
/// <param name="task">The task</param>
public class PeriodicRunnerTask(TimeSpan timeSpan, Func<CancellationToken, Task> task) : IDisposable
{
    /// <summary>
    /// The timespan
    /// </summary>
    private readonly TimeSpan _timeSpan = timeSpan;
    
    /// <summary>
    /// The task
    /// </summary>
    private readonly Func<CancellationToken, Task> _taskFunc = task;
    
    /// <summary>
    /// Cancellation token
    /// </summary>
    private CancellationTokenSource? _cts;
    /// <summary>
    /// The runner
    /// </summary>
    private Task? _runnerTask;

    /// <summary>
    /// Starts the timer task
    /// </summary>
    /// <param name="ct">The cancellation token</param>
    /// <returns>The PeriodicRunnerTask</returns>
    /// <exception cref="InvalidOperationException">If you attempt to start the task while it's already running</exception>
    public PeriodicRunnerTask Start(CancellationToken ct = default)
    {
        if (_cts is { IsCancellationRequested: false })
        {
            throw new InvalidOperationException("Periodic runner is already running");
        }

        this._cts = new CancellationTokenSource();

        this._runnerTask = Task.Run(async () =>
        {
            // how the fuck does this shit work
            //stupdi
            using CancellationTokenSource lcts = CancellationTokenSource.CreateLinkedTokenSource(_cts.Token, ct);

            using PeriodicTimer timer = new(_timeSpan);
            
            try
            {
                await InvokeAsync(lcts.Token);
                while (await timer.WaitForNextTickAsync(lcts.Token))
                {
                    await InvokeAsync(lcts.Token);
                }
            }
            catch (OperationCanceledException)
            {
            }
        }, ct);

        return this;
    }
    
    /// <summary>
    /// Calls the Task
    /// </summary>
    /// <param name="ct">The token</param>
    private async Task InvokeAsync(CancellationToken ct = default)
    {
        await _taskFunc.Invoke(ct);
    }

    /// <summary>
    /// Stops the task
    /// </summary>
    public async Task StopAsync()
    {
        _cts?.Cancel();

        if (_runnerTask != null)
        {
            try
            {
                await _runnerTask; 
            }
            catch (OperationCanceledException)
            {
            }
        }
    }

    /// <summary>
    /// Stops the task
    /// </summary>
    public void Stop() => this._cts?.Cancel();

    /// <inheritdoc/>
    public void Dispose()
    {
        _cts?.Dispose();
    }
}