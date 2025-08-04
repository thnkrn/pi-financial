using AutoFixture;
using Pi.SetMarketDataWSS.Application.Services.ItchMapper;
using Pi.SetMarketDataWSS.Application.Services.Models.ItchMessageWrapper;
using Pi.SetMarketDataWSS.Application.Services.Models.ItchOrderBookMapper;
using Pi.SetMarketDataWSS.Application.Services.Types.ItchParser;

namespace Pi.SetMarketDataWSS.Application.Tests.ItchMapper;
public class ItchPublicTradeMapperServiceTest
{
    private readonly ItchPublicTradeMapperService mapper;
    public ItchPublicTradeMapperServiceTest()
    {
        mapper = new ItchPublicTradeMapperService();
    }

    [Theory]
    [InlineData(1727774699000000000, 1727774699)]
    public void Map_ReturnPublicTradeWithValidTimestamp_WhenGotTradeTickerMessageWithNanoTimestamp(long nanosTimestamp, long secondTimestamp)
    {
        // Arrange
        var tradeTicker = new TradeTickerMessageWrapper();
        tradeTicker.DealDateTime = new Timestamp(nanosTimestamp);
            
        // Act
        var result = mapper.Map(tradeTicker, new PublicTrade[]{});

        // Assert
        Assert.Equal(secondTimestamp, result.PublicTrade[0].Nanos);
    }
}