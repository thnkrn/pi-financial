using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Pi.GlobalMarketDataWSS.DataSubscriber.Services;
using Pi.GlobalMarketDataWSS.Domain.Models.Response;
using Xunit.Abstractions;

namespace Pi.GlobalMarketDataWSS.DataSubscriber.Tests.Services;

public class MessageSamplerServicePerformanceTests
{
    private readonly Mock<ILogger<MessageSamplerService>> _mockLogger;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly ITestOutputHelper _output;

    public MessageSamplerServicePerformanceTests(ITestOutputHelper output)
    {
        _output = output;
        _mockLogger = new Mock<ILogger<MessageSamplerService>>();
        _mockConfiguration = new Mock<IConfiguration>();
        
        var enabledSection = new Mock<IConfigurationSection>();
        enabledSection.Setup(s => s.Value).Returns("true");
        _mockConfiguration.Setup(c => c.GetSection("MESSAGE_SAMPLING:ENABLED")).Returns(enabledSection.Object);
        
        var countSection = new Mock<IConfigurationSection>();
        countSection.Setup(s => s.Value).Returns("8");
        _mockConfiguration.Setup(c => c.GetSection("MESSAGE_SAMPLING:SAMPLING_COUNT")).Returns(countSection.Object);
        
        var timeWindowSection = new Mock<IConfigurationSection>();
        timeWindowSection.Setup(s => s.Value).Returns("20");
        _mockConfiguration.Setup(c => c.GetSection("MESSAGE_SAMPLING:TIME_WINDOW_MS")).Returns(timeWindowSection.Object);
        
        var cleanupSection = new Mock<IConfigurationSection>();
        cleanupSection.Setup(s => s.Value).Returns("30");
        _mockConfiguration.Setup(c => c.GetSection("MESSAGE_SAMPLING:CLEANUP_INTERVAL_MINUTES"))
            .Returns(cleanupSection.Object);
        
        var inactiveSection = new Mock<IConfigurationSection>();
        inactiveSection.Setup(s => s.Value).Returns("10");
        _mockConfiguration.Setup(c => c.GetSection("MESSAGE_SAMPLING:INACTIVE_THRESHOLD_MINUTES"))
            .Returns(inactiveSection.Object);
    }

    [Fact]
    public void ProcessMillionMessages_SingleSymbol_MeasurePerformance()
    {
        // Arrange
        var service = new MessageSamplerService(_mockLogger.Object, _mockConfiguration.Object);
        var symbol = "AAPL";
        var messageCount = 1_000_000; // 1 million messages
        
        // Act
        var stopwatch = Stopwatch.StartNew();
        
        for (int i = 0; i < messageCount; i++)
        {
            service.ShouldPublishMessage(symbol, CreateSampleResponse(symbol));
        }
        
        stopwatch.Stop();
        
        // Calculate metrics
        var totalTime = stopwatch.ElapsedMilliseconds;
        var messagesPerSecond = messageCount * 1000 / (double)totalTime;
        var timePerMessage = totalTime / (double)messageCount;
        
        _output.WriteLine($"Processed {messageCount:N0} messages in {totalTime:N0}ms");
        _output.WriteLine($"Messages per second: {messagesPerSecond:N0}");
        _output.WriteLine($"Time per message: {timePerMessage:F6}ms");
        
        // Rough estimate - A high-performance implementation should process millions of messages per second on modern hardware
        // Assert that processing time is less than 0.001ms per message (1 microsecond)
        Assert.True(timePerMessage < 0.001, $"Time per message should be less than 0.001ms but was {timePerMessage:F6}ms");
        
        // Assert we can easily handle the required load (8M/hour = ~2,222 messages per second)
        Assert.True(messagesPerSecond > 5000, $"Should handle at least 5000 messages per second, but got {messagesPerSecond:N0}");
    }

    [Fact]
    public void ProcessMillionMessages_MultipleSymbols_MeasurePerformance()
    {
        // Arrange
        var service = new MessageSamplerService(_mockLogger.Object, _mockConfiguration.Object);
        var symbols = new[] { "AAPL", "MSFT", "GOOG", "AMZN", "META" };
        var messageCount = 1_000_000; // 1 million messages total across all symbols
        var messagesPerSymbol = messageCount / symbols.Length;
        
        // Act
        var stopwatch = Stopwatch.StartNew();
        
        for (int i = 0; i < messagesPerSymbol; i++)
        {
            foreach (var symbol in symbols)
            {
                service.ShouldPublishMessage(symbol, CreateSampleResponse(symbol));
            }
        }
        
        stopwatch.Stop();
        
        // Calculate metrics
        var totalTime = stopwatch.ElapsedMilliseconds;
        var messagesPerSecond = messageCount * 1000 / (double)totalTime;
        var timePerMessage = totalTime / (double)messageCount;
        
        _output.WriteLine($"Processed {messageCount:N0} messages across {symbols.Length} symbols in {totalTime:N0}ms");
        _output.WriteLine($"Messages per second: {messagesPerSecond:N0}");
        _output.WriteLine($"Time per message: {timePerMessage:F6}ms");
        
        // Assert we can easily handle the required load with multiple symbols
        Assert.True(messagesPerSecond > 2222, $"Should handle at least 2,222 messages per second, but got {messagesPerSecond:N0}");
    }

    [Fact]
    public void MemoryUsage_LargeNumberOfSymbols_RemainsBounded()
    {
        // Arrange
        var service = new MessageSamplerService(_mockLogger.Object, _mockConfiguration.Object);
        var symbolCount = 10_000; // Test with 10,000 different symbols
        var messagesPerSymbol = 10; // Each symbol gets enough messages to trigger a publish
        
        // Force GC collect to get more accurate baseline
        GC.Collect();
        GC.WaitForPendingFinalizers();
        var initialMemory = GC.GetTotalMemory(true);
        
        // Act - Process messages for many symbols
        for (int symIdx = 0; symIdx < symbolCount; symIdx++)
        {
            var symbol = $"SYM{symIdx}";
            for (int msgIdx = 0; msgIdx < messagesPerSymbol; msgIdx++)
            {
                service.ShouldPublishMessage(symbol, CreateSampleResponse(symbol));
            }
        }
        
        // Force GC again to measure actual retained memory
        GC.Collect();
        GC.WaitForPendingFinalizers();
        var finalMemory = GC.GetTotalMemory(true);
        var memoryUsed = finalMemory - initialMemory;
        var bytesPerSymbol = memoryUsed / symbolCount;
        
        _output.WriteLine($"Initial memory: {initialMemory:N0} bytes");
        _output.WriteLine($"Final memory: {finalMemory:N0} bytes");
        _output.WriteLine($"Memory used for {symbolCount:N0} symbols: {memoryUsed:N0} bytes");
        _output.WriteLine($"Average bytes per symbol: {bytesPerSymbol:N0}");
        
        // This is a rough estimate - should be small enough to handle thousands of symbols
        Assert.True(bytesPerSymbol < 3000, $"Memory usage per symbol should be less than 3000 bytes, but was {bytesPerSymbol:N0}");
    }

    [Fact]
    public void HighFrequencySymbol_SamplingIsEffective()
    {
        // Arrange
        var service = new MessageSamplerService(_mockLogger.Object, _mockConfiguration.Object);
        var symbol = "HIGH_FREQ_SYMBOL";
        var totalMessages = 100_000; // 100K messages
    
        // Act
        var publishCount = 0;
        var stopwatch = Stopwatch.StartNew();
    
        for (int i = 0; i < totalMessages; i++)
        {
            if (service.ShouldPublishMessage(symbol, CreateSampleResponse(symbol)))
            {
                publishCount++;
            }
        }
    
        stopwatch.Stop();
    
        // Calculate sampling efficiency
        var samplingRate = publishCount / (double)totalMessages;
        var theoreticalRate = 1.0 / 8.0; // เปลี่ยนจาก 1/10 เป็น 1/8 เนื่องจาก count=8
    
        _output.WriteLine($"Total messages: {totalMessages:N0}");
        _output.WriteLine($"Published messages: {publishCount:N0}");
        _output.WriteLine($"Sampling rate: {samplingRate:P2}");
        _output.WriteLine($"Theoretical rate: {theoreticalRate:P2}");
        _output.WriteLine($"Time taken: {stopwatch.ElapsedMilliseconds:N0}ms");
        
        Assert.True(samplingRate >= 0.10 && samplingRate <= 0.15,
            $"Sampling rate should be approximately 12.5% but was {samplingRate:P2}");
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