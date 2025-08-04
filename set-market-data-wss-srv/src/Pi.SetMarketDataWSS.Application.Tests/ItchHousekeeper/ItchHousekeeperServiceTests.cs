using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Pi.SetMarketDataWSS.Application.Services.Constants;
using Pi.SetMarketDataWSS.Application.Services.ItchHousekeeper;
using Pi.SetMarketDataWSS.Application.Services.Models.ItchMessageWrapper;
using Pi.SetMarketDataWSS.Domain.Entities;
using Pi.SetMarketDataWSS.Domain.Models.Redis;
using Pi.SetMarketDataWSS.Domain.Models.Response;

namespace Pi.SetMarketDataWSS.Application.Tests.ItchHousekeeper;

public class ItchHousekeeperServiceTests
{
    private readonly Mock<ILogger<ItchHousekeeperService>> _mockLogger;
    private readonly ItchHousekeeperService _service;

    public ItchHousekeeperServiceTests()
    {
        _mockLogger = new Mock<ILogger<ItchHousekeeperService>>();
        _service = new ItchHousekeeperService(_mockLogger.Object);
    }

    [Fact]
    public void ResetStat_WithValidMessage_ReturnsExpectedResult()
    {
        // Arrange
        var message = new OrderBookStateMessageWrapper
        {
            StateName = new StateName
            {
                Value = "PRE-OPEN_E"
            }
        };


        var currentCacheValue = new Dictionary<string, string>
        {
            { CacheKey.PriceInfo, JsonConvert.SerializeObject(new PriceInfo { Open = "100", High24H = "200" }) },
            {
                CacheKey.StreamingBody, JsonConvert.SerializeObject(new MarketStreamingResponse
                {
                    Response = new StreamingResponse
                    {
                        Data = new List<StreamingBody>
                        {
                            new() { Open = "100", High24H = "200" }
                        }
                    }
                })
            }
        };

        // Act
        var result = _service.ResetStat(message, currentCacheValue);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(RedisChannel.PubSubCache, result.RedisChannel);
        Assert.Equal(3, result.RedisValue.Length);

        var priceInfo =
            JsonConvert.DeserializeObject<PriceInfo>(JsonConvert.SerializeObject(result.RedisValue[0].Value));
        Assert.Equal("0.00", priceInfo.Open);
        Assert.Equal("0.00", priceInfo.High24H);

        var streamingResponse =
            JsonConvert.DeserializeObject<MarketStreamingResponse>(
                JsonConvert.SerializeObject(result.RedisValue[1].Value));
        Assert.Equal("0.00", streamingResponse.Response.Data[0].Open);
        Assert.Equal("0.00", streamingResponse.Response.Data[0].High24H);

        var publicTrades =
            JsonConvert.DeserializeObject<PublicTrade[]>(JsonConvert.SerializeObject(result.RedisValue[2].Value));
        Assert.Empty(publicTrades);
    }


    [Fact]
    public void ResetStat_WithInvalidStateName_ReturnsNull()
    {
        // Arrange
        var message = new OrderBookStateMessageWrapper
        {
            StateName = new StateName { Value = "INVALID_STATE" }
        };

        var currentCacheValue = new Dictionary<string, string>();

        // Act
        var result = _service.ResetStat(message, currentCacheValue);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void ResetStat_WithEmptyCache_ReturnsExpectedResult()
    {
        // Arrange
        var message = new OrderBookStateMessageWrapper
        {
            StateName = new StateName { Value = "PRE-OPEN_E" }
        };

        var currentCacheValue = new Dictionary<string, string>();

        // Act
        var result = _service.ResetStat(message, currentCacheValue);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(RedisChannel.PubSubCache, result.RedisChannel);
        Assert.Equal(3, result.RedisValue.Length);

        var priceInfo =
            JsonConvert.DeserializeObject<PriceInfo>(JsonConvert.SerializeObject(result.RedisValue[0].Value));
        Assert.Null(priceInfo);

        var streamingResponse =
            JsonConvert.DeserializeObject<MarketStreamingResponse>(
                JsonConvert.SerializeObject(result.RedisValue[1].Value));
        Assert.Null(streamingResponse);

        var publicTrades =
            JsonConvert.DeserializeObject<PublicTrade[]>(JsonConvert.SerializeObject(result.RedisValue[2].Value));
        Assert.Empty(publicTrades);
    }

    [Fact]
    public void ResetStat_WithJsonException_ReturnsNull()
    {
        // Arrange
        var message = new OrderBookStateMessageWrapper
        {
            StateName = new StateName { Value = "PRE-OPEN_E" }
        };

        var currentCacheValue = new Dictionary<string, string>
        {
            { CacheKey.PriceInfo, "Invalid JSON" }
        };

        // Act
        var result = _service.ResetStat(message, currentCacheValue);

        // Assert
        Assert.Null(result);
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString().Contains("JsonException")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }
}