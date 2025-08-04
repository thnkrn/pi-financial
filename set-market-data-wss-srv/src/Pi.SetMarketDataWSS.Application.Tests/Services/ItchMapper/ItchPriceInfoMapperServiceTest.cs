using AutoFixture;
using Pi.SetMarketDataWSS.Application.Services.ItchMapper;
using Pi.SetMarketDataWSS.Application.Services.Models.ItchMessageWrapper;
using Pi.SetMarketDataWSS.Application.Services.Types.ItchParser;

public class ItchPriceInfoMapperServiceTest
{
    private readonly ItchPriceInfoMapperService _priceInfoMapperService;
    public ItchPriceInfoMapperServiceTest()
    {
        _priceInfoMapperService = new ItchPriceInfoMapperService();
    }

    [Fact]
    public void Map_ReturnCorrectAverageSell_When_GotAggressor_B()
    {
        // Arrange
        var fixture = new Fixture();
        
        var orderBookStateStatus = fixture.Create<string>();
        var mockedTradeTicker = new TradeTickerMessageWrapper {
            Aggressor = new Alpha(new ReadOnlySpan<byte>(66),1),
        };
        // Act
        var result = _priceInfoMapperService.Map(mockedTradeTicker, null, orderBookStateStatus);
        
        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.AverageSell);
        Assert.Equal(mockedTradeTicker.Price.Value.ToString(), result.AverageSell);
    }
    [Fact]
    public void Map_ReturnCorrectAverageBuy_When_GotAggressor_A()
    {
        // Arrange
        var fixture = new Fixture();
        
        var orderBookStateStatus = fixture.Create<string>();
        var mockedTradeTicker = new TradeTickerMessageWrapper {
            Aggressor = new Alpha(new ReadOnlySpan<byte>(65),1),
            Price = new Price32(200)
        };
        // Act
        var result = _priceInfoMapperService.Map(mockedTradeTicker, null, orderBookStateStatus);
        
        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.AverageBuy);
        Assert.Equal(mockedTradeTicker.Price.Value.ToString(), result.AverageBuy);
    }
}