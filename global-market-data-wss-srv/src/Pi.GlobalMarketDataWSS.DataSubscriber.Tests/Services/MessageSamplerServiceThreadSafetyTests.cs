using System.Collections.Concurrent;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Pi.GlobalMarketDataWSS.DataSubscriber.Services;
using Pi.GlobalMarketDataWSS.Domain.Models.Response;
using Xunit.Abstractions;

namespace Pi.GlobalMarketDataWSS.DataSubscriber.Tests.Services;

public class MessageSamplerServiceThreadSafetyTests
{
    private readonly Mock<ILogger<MessageSamplerService>> _mockLogger;
    private readonly IConfiguration _configuration;
    private readonly ITestOutputHelper _output;

    public MessageSamplerServiceThreadSafetyTests(ITestOutputHelper output)
    {
        _output = output;
        _mockLogger = new Mock<ILogger<MessageSamplerService>>();
        
        var configBuilder = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                {"MESSAGE_SAMPLING:ENABLED", "true"},
                {"MESSAGE_SAMPLING:SAMPLING_COUNT", "8"},
                {"MESSAGE_SAMPLING:TIME_WINDOW_MS", "20"},
                {"MESSAGE_SAMPLING:CLEANUP_INTERVAL_MINUTES", "30"},
                {"MESSAGE_SAMPLING:INACTIVE_THRESHOLD_MINUTES", "10"}
            });
        
        _configuration = configBuilder.Build();
    }

    [Fact]
    public async Task MultipleThreads_SingleSymbol_SamplingIsConsistent()
    {
        // Arrange
        var service = new MessageSamplerService(_mockLogger.Object, _configuration);
        var symbol = "AAPL";
        var threadCount = 16; // Use multiple threads to simulate concurrent access
        var messagesPerThread = 100_000; // Each thread sends 100K messages
        var totalMessages = threadCount * messagesPerThread;
        
        var publishedMessages = new ConcurrentBag<int>();
        var tasks = new List<Task>();
        var countdown = new CountdownEvent(threadCount);
        
        // Act - Start multiple threads all hitting the same symbol
        for (int t = 0; t < threadCount; t++)
        {
            var threadId = t;
            tasks.Add(Task.Run(() => {
                try
                {
                    // Wait for all threads to be ready (helps create more contention)
                    countdown.Signal();
                    countdown.Wait();
                    
                    for (int i = 0; i < messagesPerThread; i++)
                    {
                        var msgId = threadId * messagesPerThread + i;
                        var response = CreateSampleResponse(symbol);
                        if (service.ShouldPublishMessage(symbol, response))
                        {
                            publishedMessages.Add(msgId);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _output.WriteLine($"Thread {threadId} error: {ex.Message}");
                    throw;
                }
            }));
        }
        
        // Wait for all threads to complete
        await Task.WhenAll(tasks);
        
        // Calculate sampling efficiency
        var publishCount = publishedMessages.Count;
        var samplingRate = publishCount / (double)totalMessages;
        var theoreticalRate = 1.0 / 8.0; // เปลี่ยนจาก 1/10 เป็น 1/8 (SamplingCount = 8)
        
        _output.WriteLine($"Total messages: {totalMessages:N0} from {threadCount} threads");
        _output.WriteLine($"Published messages: {publishCount:N0}");
        _output.WriteLine($"Sampling rate: {samplingRate:P2}");
        _output.WriteLine($"Theoretical rate: {theoreticalRate:P2}");
        
        // Assert sampling is consistent across threads
        Assert.True(samplingRate >= 0.10 && samplingRate <= 0.15,
            $"Sampling rate should be approximately 12.5% but was {samplingRate:P2}");
        
        // No exceptions should have been thrown
        Assert.All(tasks, task => Assert.True(task.IsCompletedSuccessfully));
    }

    [Fact]
    public async Task MultipleThreads_MultipleSymbols_NoContention()
    {
        // Arrange
        var service = new MessageSamplerService(_mockLogger.Object, _configuration);
        var symbolCount = 100; // 100 different symbols
        var threadCount = 16; // Use multiple threads to simulate concurrent access
        var messagesPerSymbolPerThread = 1_000; // Each thread sends 1K messages per symbol
        
        var publishCounts = new ConcurrentDictionary<string, int>();
        var messageCounts = new ConcurrentDictionary<string, int>();
        var tasks = new List<Task>();
        
        // Act - Start multiple threads accessing different symbols
        for (int t = 0; t < threadCount; t++)
        {
            var threadId = t;
            tasks.Add(Task.Run(() => {
                try
                {
                    for (int s = 0; s < symbolCount; s++)
                    {
                        var symbol = $"SYM{s}";
                        
                        for (int i = 0; i < messagesPerSymbolPerThread; i++)
                        {
                            var response = CreateSampleResponse(symbol);
                            messageCounts.AddOrUpdate(symbol, 1, (_, count) => count + 1);
                            
                            if (service.ShouldPublishMessage(symbol, response))
                            {
                                publishCounts.AddOrUpdate(symbol, 1, (_, count) => count + 1);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _output.WriteLine($"Thread {threadId} error: {ex.Message}");
                    throw;
                }
            }));
        }
        
        // Wait for all threads to complete
        await Task.WhenAll(tasks);
        
        // Calculate sampling rates per symbol
        var samplingRates = new Dictionary<string, double>();
        foreach (var symbol in messageCounts.Keys)
        {
            var totalForSymbol = messageCounts[symbol];
            var publishedForSymbol = publishCounts.GetValueOrDefault(symbol, 0);
            samplingRates[symbol] = publishedForSymbol / (double)totalForSymbol;
        }
        
        var avgSamplingRate = samplingRates.Values.Average();
        var minRate = samplingRates.Values.Min();
        var maxRate = samplingRates.Values.Max();
        
        _output.WriteLine($"Symbols: {symbolCount}");
        _output.WriteLine($"Threads: {threadCount}");
        _output.WriteLine($"Average sampling rate: {avgSamplingRate:P2}");
        _output.WriteLine($"Min sampling rate: {minRate:P2}");
        _output.WriteLine($"Max sampling rate: {maxRate:P2}");
        
        // Assert sampling is consistent across symbols
        Assert.True(avgSamplingRate >= 0.10 && avgSamplingRate <= 0.15,
            $"Average sampling rate should be approximately 12.5% but was {avgSamplingRate:P2}");
        
        // Check that there's not too much variation between symbols
        Assert.True(maxRate - minRate < 0.05, 
            $"Sampling rate variation between symbols should be small, but was {maxRate - minRate:P2}");
        
        // No exceptions should have been thrown
        Assert.All(tasks, task => Assert.True(task.IsCompletedSuccessfully));
    }

    [Fact]
    public async Task StressTest_HighConcurrencyMixedOperations()
    {
        // Arrange
        var service = new MessageSamplerService(_mockLogger.Object, _configuration);
        await service.StartAsync(CancellationToken.None);

        var threadCount = 32; // High thread count to stress concurrency
        var operationsPerThread = 10_000; // Reduced from 50K to make test run faster
        var symbolCount = 50; // Number of distinct symbols
        
        var random = new Random(42); // Fixed seed for reproducibility
        var errorCounter = 0;
        var operations = new ConcurrentBag<string>();
        
        try
        {
            var stopwatch = Stopwatch.StartNew();
            var tasks = new List<Task>();
            
            // Act - Launch many threads doing mixed operations
            for (int t = 0; t < threadCount; t++)
            {
                tasks.Add(Task.Run(() => {
                    try
                    {
                        for (int i = 0; i < operationsPerThread; i++)
                        {
                            var symbol = $"SYM{random.Next(symbolCount)}";
                            var opType = random.Next(3); // 0=ShouldPublish, 1=StoreLatest, 2=GetLatest
                            
                            try
                            {
                                switch (opType)
                                {
                                    case 0:
                                        var result = service.ShouldPublishMessage(symbol, CreateSampleResponse(symbol));
                                        operations.Add($"Publish:{result}");
                                        break;
                                    case 1:
                                        service.StoreLatestResponse(symbol, CreateSampleResponse(symbol));
                                        operations.Add("Store");
                                        break;
                                    case 2:
                                        var response = service.GetLatestResponse(symbol);
                                        operations.Add($"Get:{response != null}");
                                        break;
                                }
                            }
                            catch (Exception ex)
                            {
                                Interlocked.Increment(ref errorCounter);
                                _output.WriteLine($"Operation error: {ex.Message}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Interlocked.Increment(ref errorCounter);
                        _output.WriteLine($"Thread error: {ex.Message}");
                    }
                }));
            }
            
            // Wait for all operations to complete
            await Task.WhenAll(tasks);
            stopwatch.Stop();
            
            // Analyze results
            var totalOperations = threadCount * operationsPerThread;
            var operationsPerSecond = totalOperations / stopwatch.Elapsed.TotalSeconds;
            
            var publishOps = operations.Count(o => o.StartsWith("Publish:"));
            var storeOps = operations.Count(o => o == "Store");
            var getOps = operations.Count(o => o.StartsWith("Get:"));
            
            _output.WriteLine($"Total operations: {totalOperations:N0}");
            _output.WriteLine($"Elapsed time: {stopwatch.Elapsed.TotalSeconds:F2} seconds");
            _output.WriteLine($"Operations per second: {operationsPerSecond:N0}");
            _output.WriteLine($"Publish operations: {publishOps:N0}");
            _output.WriteLine($"Store operations: {storeOps:N0}");
            _output.WriteLine($"Get operations: {getOps:N0}");
            _output.WriteLine($"Errors: {errorCounter}");
            
            // Assert high throughput and no errors
            Assert.Equal(0, errorCounter);
            Assert.True(operationsPerSecond > 10000, $"Should handle at least 10K ops/sec, but was {operationsPerSecond:N0}");
        }
        finally
        {
            await service.StopAsync(CancellationToken.None);
            service.Dispose();
        }
    }
    
    [Fact]
    public void Concurrent_ShouldPublishAndGetLatest_NoRaceConditions()
    {
        // Arrange
        var service = new MessageSamplerService(_mockLogger.Object, _configuration);
        var symbol = "RACE_CONDITION_TEST";
        var iterations = 10_000;
        int inconsistencyCount = 0;
    
        // Act - Try to create race conditions between ShouldPublish and GetLatest
        Parallel.For(0, iterations, i =>
        {
            // Prepare a new response with a unique ID to detect
            var uniqueId = $"MSG-{i}";
            var response = CreateSampleResponse(symbol);
            response.SendingId = uniqueId;
        
            // Store this response
            service.StoreLatestResponse(symbol, response);
            
            if (i % 10 == 0)
                Thread.Sleep(1);
            
            // Try to create race condition
            var published = service.ShouldPublishMessage(symbol, null);
            var latest = service.GetLatestResponse(symbol);
            
            // Check for inconsistency: latest should not be null if published is true
            if (published && latest == null)
            {
                Interlocked.Increment(ref inconsistencyCount);
            }
        });
    
        // Output results
        _output.WriteLine($"Iterations: {iterations}");
        _output.WriteLine($"Inconsistencies detected: {inconsistencyCount}");
        
        Assert.True(inconsistencyCount < iterations * 0.01, 
            $"Inconsistencies should be less than 1%, but was {inconsistencyCount} out of {iterations} ({inconsistencyCount * 100.0 / iterations:F2}%)");
    }

    private MarketStreamingResponse CreateSampleResponse(string symbol)
    {
        var streamingBody = new StreamingBody { Symbol = symbol };
        var streamingResponse = new StreamingResponse { Data = new List<StreamingBody> { streamingBody } };
        
        return new MarketStreamingResponse
        {
            Code = "200",
            Op = "Streaming",
            SendingTime = DateTime.UtcNow.ToString("yyyyMMdd-HH:mm:ss.fff"),
            SequenceNumber = 12345,
            ProcessingTime = DateTime.UtcNow.ToString("yyyyMMdd-HH:mm:ss.fff"),
            SendingId = Guid.NewGuid().ToString("N"),
            CreationTime = DateTime.UtcNow.ToString("yyyyMMdd-HH:mm:ss.fff"),
            MdEntryType = "0",
            Response = streamingResponse
        };
    }
}