using System.Collections.Concurrent;
using System.Text;
using Newtonsoft.Json;

namespace Pi.SetMarketDataRealTime.Application.Services.WriteBinLogData;

public sealed class RealTimeStockMessageLogger : IDisposable
{
    private int _batchSize;
    private CancellationTokenSource? _cts;
    private bool _disposed;
    private int _flushIntervalMs;
    private ConcurrentDictionary<string, BlockingCollection<string>>? _messageQueues;
    private string? _messageToFilePath;
    private HashSet<string>? _messageTypes;
    private string? _orderBookId;
    private Task[]? _writerTasks;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~RealTimeStockMessageLogger()
    {
        Dispose(false);
    }

    public void BuildRealTimeStockMessageLogger(string? messageToFilePath, IEnumerable<string> messageTypes,
        string? orderBookId,
        int batchSize = 1000, int flushIntervalMs = 1000)
    {
        _messageToFilePath = messageToFilePath;
        _messageTypes = new HashSet<string>(messageTypes);
        _orderBookId = orderBookId;
        _batchSize = batchSize;
        _flushIntervalMs = flushIntervalMs;
        _messageQueues = new ConcurrentDictionary<string, BlockingCollection<string>>();
        _cts = new CancellationTokenSource();
        _writerTasks = new Task[Environment.ProcessorCount];

        for (var i = 0; i < _writerTasks.Length; i++) _writerTasks[i] = Task.Run(ProcessQueuesAsync);
    }

    public void EnqueueStockMessage<T>(T stockMessage, string message, string messageType)
    {
        if (_messageTypes != null && !_messageTypes.Contains(messageType))
            return;

        var containsOrderBookId = "\"OrderBookID\":{\"Value\":" + _orderBookId + "}";

        if (!string.IsNullOrEmpty(_orderBookId) &&
            !message.Contains(containsOrderBookId, StringComparison.OrdinalIgnoreCase))
            return;

        var fileName = GetCurrentFileName();
        var stockData = JsonConvert.SerializeObject(stockMessage);

        _messageQueues?.GetOrAdd(fileName, _ => new BlockingCollection<string>())
            .Add(stockData);
    }

    private async Task ProcessQueuesAsync()
    {
        while (_cts is { IsCancellationRequested: false })
            try
            {
                var currentFileName = GetCurrentFileName();
                if (_messageQueues != null && _messageQueues.TryGetValue(currentFileName, out var queue))
                    await ProcessQueueAsync(currentFileName, queue);

                await Task.Delay(100); // Small delay to prevent tight looping
            }
            catch (OperationCanceledException)
            {
                // This is expected when cancellation is requested
                break;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing to file: {ex.Message}");
                throw;
            }
    }

    private async Task ProcessQueueAsync(string fileName, BlockingCollection<string> queue)
    {
        var messages = new List<string>(_batchSize);
        var dequeueTimeout = TimeSpan.FromMilliseconds(_flushIntervalMs);

        while (messages.Count < _batchSize)
            if (queue.TryTake(out var message, dequeueTimeout))
                messages.Add(message);
            else break;

        if (messages.Count > 0) await WriteMessagesToFileAsync(fileName, messages);
    }

    private async Task WriteMessagesToFileAsync(string fileName, List<string> messages)
    {
        if (_messageToFilePath != null)
        {
            var filePath = Path.Combine(_messageToFilePath, fileName);
            try
            {
                await using var writer = new StreamWriter(filePath, true, Encoding.UTF8);
                foreach (var message in messages) await writer.WriteLineAsync(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing to file {fileName}: {ex.Message}");
            }
        }
    }

    private static string GetCurrentFileName()
    {
        return $"{DateTime.Now:yyyyMMdd}_stock_message_data.pi";
    }

    private void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            _cts?.Cancel();

            try
            {
                if (_writerTasks != null) Task.WaitAll(_writerTasks);
                if (_messageQueues != null)
                {
                    foreach (var kvp in _messageQueues) ProcessQueueAsync(kvp.Key, kvp.Value).Wait();
                    foreach (var queue in _messageQueues.Values) queue.Dispose();
                }
            }
            catch (OperationCanceledException)
            {
                // This is expected
            }

            _cts?.Dispose();
        }

        _disposed = true;
    }
}