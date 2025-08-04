using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;
using Pi.TfexService.Infrastructure.Options;
using Pi.TfexService.Infrastructure.Services;
using RedLockNet;

namespace Pi.TfexService.Infrastructure.Tests.Services;

public class DistributedLockServiceTests
{
    private readonly IDistributedCache _cache;
    private readonly Mock<IDistributedLockFactory> _lockFactory;

    private readonly DistributedLockService _service;

    public DistributedLockServiceTests()
    {
        var cacheOptions = new OptionsWrapper<MemoryDistributedCacheOptions>(new MemoryDistributedCacheOptions());
        _cache = new MemoryDistributedCache(cacheOptions);
        _lockFactory = new Mock<IDistributedLockFactory>();
        var featureOptions = new Mock<IOptionsMonitor<FeaturesOptions>>();
        var options = new FeaturesOptions()
        {
            TfexNotificationExpireTimeSecond = 2
        };

        featureOptions
            .Setup(o => o.CurrentValue)
            .Returns(options);

        _service = new DistributedLockService(_lockFactory.Object, _cache, featureOptions.Object);
    }

    [Fact]
    public async Task AddEvent_Should_Failed_When_Lock_Acquire_Failed()
    {
        // Arrange
        var eventName = "test-event";
        var key = $"lock-event:{eventName}";
        var redLock = new Mock<IRedLock>();
        redLock.Setup(r => r.IsAcquired).Returns(false);
        _lockFactory.Setup(f => f.CreateLockAsync(key, It.IsAny<TimeSpan>())).ReturnsAsync(redLock.Object);

        // Act
        var result = await _service.AddEventAsync(eventName);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task AddEvent_Should_Failed_When_Found_Duplication()
    {
        // Arrange
        var eventName = "test-event";
        var key = $"lock-event:{eventName}";
        var redLock = new Mock<IRedLock>();
        redLock.Setup(r => r.IsAcquired).Returns(true);
        _lockFactory.Setup(f => f.CreateLockAsync(key, It.IsAny<TimeSpan>())).ReturnsAsync(redLock.Object);

        await _cache.SetAsync(key, [1]);

        // Act
        var result = await _service.AddEventAsync(eventName);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task AddEvent_Should_Success()
    {
        // Arrange
        var eventName = "test-event";
        var key = $"lock-event:{eventName}";
        var redLock = new Mock<IRedLock>();
        redLock.Setup(r => r.IsAcquired).Returns(true);
        _lockFactory.Setup(f => f.CreateLockAsync(key, It.IsAny<TimeSpan>())).ReturnsAsync(redLock.Object);

        // Act
        var result = await _service.AddEventAsync(eventName);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task RemoveEvent_Should_Return_False_When_Event_Not_Found()
    {
        // Arrange
        var eventName = "test-event";

        // Act
        var result = await _service.RemoveEventAsync(eventName);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task RemoveEvent_Should_Success()
    {
        // Arrange
        var eventName = "test-event";
        var key = $"lock-event:{eventName}";

        // Add the event to the cache first
        await _cache.SetAsync(key, [1]);

        // Act
        var result = await _service.RemoveEventAsync(eventName);

        // Assert
        Assert.True(result);
    }
}