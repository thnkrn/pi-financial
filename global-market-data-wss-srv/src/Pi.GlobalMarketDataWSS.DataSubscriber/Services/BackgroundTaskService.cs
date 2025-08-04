using System.Diagnostics;

namespace Pi.GlobalMarketDataWSS.DataSubscriber.Services;

public class BackgroundTaskService : BackgroundService
{
    private readonly ILogger<BackgroundTaskService> _logger;
    private readonly BackgroundTaskQueue _queue;
    private readonly Stopwatch _uptime = new();
    private readonly int _workerCount;
    private long _activeWorkers;
    private long _totalProcessingTimeMs;

    /// <summary>
    /// </summary>
    /// <param name="queue"></param>
    /// <param name="logger"></param>
    public BackgroundTaskService(
        BackgroundTaskQueue queue,
        ILogger<BackgroundTaskService> logger)
    {
        _queue = queue;
        _logger = logger;
        _workerCount = Math.Max(Environment.ProcessorCount, 4);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Background Task Service starting with {WorkerCount} workers", _workerCount);
        _uptime.Start();

        var workers = new Task[_workerCount];
        for (var i = 0; i < _workerCount; i++)
        {
            var workerId = i;
            workers[i] = Task.Run(async () => await ProcessTasksAsync(workerId, stoppingToken), stoppingToken);
        }

        await Task.WhenAll(workers);

        _uptime.Stop();
        _logger.LogInformation(
            "Background Task Service stopped after {UptimeHours:F1} hours of operation",
            _uptime.Elapsed.TotalHours);
    }

    private async Task ProcessTasksAsync(int workerId, CancellationToken stoppingToken)
    {
        _logger.LogDebug("Worker {WorkerId} started", workerId);

        while (!stoppingToken.IsCancellationRequested)
        {
            Func<CancellationToken, Task>? workItem = null;
            var stopwatch = new Stopwatch();

            try
            {
                // รอรับงานจากคิว
                workItem = await _queue.DequeueAsync(stoppingToken);
                Interlocked.Increment(ref _activeWorkers);

                stopwatch.Start();
                if (workItem != null)
                    await workItem(stoppingToken);
                stopwatch.Stop();

                Interlocked.Add(ref _totalProcessingTimeMs, stopwatch.ElapsedMilliseconds);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                if (workItem != null) _queue.IncrementFailedCount();

                _logger.LogError(ex, "Error occurred on worker {WorkerId}", workerId);

                try
                {
                    await Task.Delay(100, stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
            }
            finally
            {
                if (workItem != null) Interlocked.Decrement(ref _activeWorkers);
            }
        }

        _logger.LogDebug("Worker {WorkerId} stopped", workerId);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Stopping background service with {RemainingItems} items in queue and {ActiveWorkers} active workers",
            _queue.Count, Interlocked.Read(ref _activeWorkers));

        await base.StopAsync(cancellationToken);
    }
}