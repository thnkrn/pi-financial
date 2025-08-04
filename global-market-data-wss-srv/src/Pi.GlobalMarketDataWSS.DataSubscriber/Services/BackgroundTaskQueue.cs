using System.Threading.Channels;

namespace Pi.GlobalMarketDataWSS.DataSubscriber.Services;

public sealed class BackgroundTaskQueue : IDisposable
{
    private readonly ILogger<BackgroundTaskQueue> _logger;
    private readonly Timer _monitorTimer;
    private readonly Channel<Func<CancellationToken, Task>> _queue;
    private bool _disposed;
    private long _totalDequeued;
    private long _totalEnqueued;
    private long _totalFailed;

    public BackgroundTaskQueue(ILogger<BackgroundTaskQueue> logger, int capacity = 100000)
    {
        _logger = logger;
        Capacity = capacity;

        var options = new BoundedChannelOptions(capacity)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = false,
            SingleWriter = false
        };

        _queue = Channel.CreateBounded<Func<CancellationToken, Task>>(options);
        _monitorTimer = new Timer(MonitorQueueStatus, null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
    }

    public int Count => _queue.Reader.Count;

    public int Capacity { get; }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async ValueTask QueueTaskAsync(Func<CancellationToken, Task> workItem)
    {
        if (workItem == null) throw new ArgumentNullException(nameof(workItem));

        var count = Count;
        if (count > Capacity * 0.8)
            _logger.LogWarning("Queue is filling up: {Count}/{Capacity} ({Percent:F1}%)",
                count, Capacity, count * 100.0 / Capacity);

        await _queue.Writer.WriteAsync(workItem);
        Interlocked.Increment(ref _totalEnqueued);
    }

    public async ValueTask<Func<CancellationToken, Task>?> DequeueAsync(CancellationToken cancellationToken)
    {
        var workItem = await _queue.Reader.ReadAsync(cancellationToken);
        Interlocked.Increment(ref _totalDequeued);
        return workItem;
    }

    private void MonitorQueueStatus(object? state)
    {
        try
        {
            var count = Count;
            var enqueued = Interlocked.Read(ref _totalEnqueued);
            var dequeued = Interlocked.Read(ref _totalDequeued);
            var failed = Interlocked.Read(ref _totalFailed);

            _logger.LogInformation(
                "Queue Status: Current={Count}/{Capacity}, Total Enqueued={Enqueued}, " +
                "Total Processed={Dequeued}, Failed={Failed}, Success Rate={SuccessRate:F2}%",
                count, Capacity, enqueued, dequeued, failed,
                dequeued > 0 ? (dequeued - failed) * 100.0 / dequeued : 100);


            if (count > Capacity * 0.9)
                _logger.LogWarning("Queue is nearly full: {Count}/{Capacity}", count, Capacity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error monitoring queue status");
        }
    }

    public void IncrementFailedCount()
    {
        Interlocked.Increment(ref _totalFailed);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            _monitorTimer.Dispose();
            _queue.Writer.Complete();
            _logger.LogInformation(
                "BackgroundTaskQueue disposed. Total items: Enqueued={Enqueued}, Dequeued={Dequeued}, Failed={Failed}",
                Interlocked.Read(ref _totalEnqueued),
                Interlocked.Read(ref _totalDequeued),
                Interlocked.Read(ref _totalFailed));
        }

        _disposed = true;
    }

    // เพิ่ม destructor (หลีกเลี่ยงการลืม dispose)
    ~BackgroundTaskQueue()
    {
        Dispose(false);
    }
}