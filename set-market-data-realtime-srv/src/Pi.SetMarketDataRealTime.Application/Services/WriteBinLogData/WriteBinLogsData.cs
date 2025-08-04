using System.Collections.Concurrent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Pi.SetMarketDataRealTime.Application.Interfaces.WriteBinlogData;
using Pi.SetMarketDataRealTime.Domain.ConstantConfigurations;

namespace Pi.SetMarketDataRealTime.Application.Services.WriteBinLogData;

public sealed class WriteBinLogsData : IWriteBinLogsData
{
    private readonly IConfiguration _configuration;
    private readonly CancellationTokenSource _cts;
    private readonly byte[] _endSequence;
    private readonly ILogger<WriteBinLogsData> _logger;
    private readonly BlockingCollection<(byte[] data, string logPrefix)> _messageQueue;
    private readonly Task _processTask;
    private bool _disposed;

    public WriteBinLogsData(IConfiguration configuration, ILogger<WriteBinLogsData> logger)
    {
        const int queueCapacity = 100000;
        _configuration = configuration;
        _logger = logger;
        _messageQueue = new BlockingCollection<(byte[] data, string logPrefix)>(
            new ConcurrentQueue<(byte[] data, string logPrefix)>(), queueCapacity);
        _cts = new CancellationTokenSource();
        _processTask = Task.Run(ProcessQueueAsync);
        _endSequence = "M128487371".ToCharArray().Select(c => (byte)c).ToArray();
    }

    public Task WriteBinLogsDataAsync(byte[] bytes, string logPrefix)
    {
        if (!_messageQueue.TryAdd((bytes, logPrefix))) _logger.LogWarning("Log queue is full. Message dropped.");
        return Task.CompletedTask;
    }

    public void EnqueueLogData(byte[] bytes, string logPrefix)
    {
        WriteBinLogsDataAsync(bytes, logPrefix).Wait();
    }

    public async Task FlushAsync()
    {
        while (_messageQueue.Count > 0) await ProcessBatchAsync();
    }

    public void CombineLogFiles(string folderPath, string logPrefix, DateTime startDate, DateTime endDate,
        string outputFile)
    {
        using var outputStream = new FileStream(outputFile, FileMode.Create);
        for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
        {
            var fileName = $"{logPrefix}_logfile_{date:yyyyMMdd}.bin";
            var filePath = Path.Combine(folderPath, fileName);
            if (!File.Exists(filePath)) continue;
            using var inputStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            inputStream.CopyTo(outputStream);
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~WriteBinLogsData()
    {
        Dispose(false);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            _cts.Cancel();
            try
            {
                // Wait for the process task to complete
                _processTask.Wait();
            }
            catch (OperationCanceledException)
            {
                // This is expected
            }

            // Flush remaining messages
            FlushAsync().Wait();

            // Now it's safe to dispose of the CancellationTokenSource and BlockingCollection
            _cts.Dispose();
            _messageQueue.Dispose();
        }

        _disposed = true;
    }

    private async Task ProcessQueueAsync()
    {
        while (!_cts.IsCancellationRequested)
            try
            {
                await ProcessBatchAsync();
            }
            catch (OperationCanceledException)
            {
                // This is expected when cancellation is requested
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing log batch");
            }
    }

    private async Task ProcessBatchAsync()
    {
        const int batchSize = 100;
        const int flushIntervalMs = 1000;
        var batch = new List<(byte[] data, string logPrefix)>();
        var dequeueTimeout = TimeSpan.FromMilliseconds(flushIntervalMs);

        while (batch.Count < batchSize && !_cts.IsCancellationRequested)
            if (_messageQueue.TryTake(out var item, dequeueTimeout))
                batch.Add(item);
            else
                break;

        if (batch.Count > 0)
            await WriteLogsToFileAsync(batch);
    }

    private async Task WriteLogsToFileAsync(IReadOnlyCollection<(byte[] data, string logPrefix)> batch)
    {
        var folderPath = _configuration[ConfigurationKeys.ServerConfigStreamDataPath];

        if (string.IsNullOrEmpty(folderPath))
        {
            _logger.LogWarning("The stream data path does not exist.");
            return;
        }

        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

        foreach (var group in batch.GroupBy(x => x.logPrefix))
        {
            var logPrefix = group.Key;
            var fileName = $"{logPrefix}_logfile_{DateTime.Now:yyyyMMdd}.bin";
            var filePath = Path.Combine(folderPath, fileName);

            try
            {
                await using var fileStream = new FileStream(filePath,
                    FileMode.Append,
                    FileAccess.Write,
                    FileShare.None,
                    4096, true);
                await using var binaryWriter = new BinaryWriter(fileStream);

                foreach (var (data, _) in group)
                {
                    if (data.Length == 0)
                    {
                        _logger.LogWarning("Data is empty");
                        continue;
                    }

                    binaryWriter.Write(data);
                    binaryWriter.Write(_endSequence);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "An error occurred while writing data: {Error}", ex.Message);
            }
        }

        _logger.LogDebug("Batch of {BatchCount} binary logs written", batch.Count);
    }
}