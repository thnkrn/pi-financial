using System.Text;
using AutoFixture;
using Google.Protobuf.WellKnownTypes;
using Pi.SetMarketDataWSS.Application.Services.ItchMapper;
using Pi.SetMarketDataWSS.Application.Services.Models.ItchMessageWrapper;
using Pi.SetMarketDataWSS.Application.Services.Models.ItchParser;
using Pi.SetMarketDataWSS.Application.Services.Types.ItchParser;
using Pi.SetMarketDataWSS.Domain.Entities;
using Action = Pi.SetMarketDataWSS.Application.Services.Models.ItchMessageWrapper.Action;

namespace Pi.SetMarketDataWSS.Application.Tests.ItchMapper;
public class ItchPriceInfoMapperServiceTest
{
    private readonly ItchPriceInfoMapperService priceInfoMapperService;
    public ItchPriceInfoMapperServiceTest()
    {
        priceInfoMapperService = new ItchPriceInfoMapperService();
    }

    [Theory]
    [InlineData(67500, 68000, -500)]
    [InlineData(65000, 68000, -3000)]
    [InlineData(68000, 65000, 3000)]
    [InlineData(680000, 650000, 30000)]
    [InlineData(2500,2000, 500)]
    public void Map_ReturnPriceInfoWithValidPriceChange_WhenGotTradeTickerMessage(int currentPrice, int closedPrice, int expectedPriceChange)
    {
        // Arrange
        var fixture = new Fixture();
        // cached price
        var cachedPriceInfo = fixture.Build<PriceInfo>()
            .With(x => x.PreClose, closedPrice.ToString())
            .With(x => x.Instrument, (Instrument)null)
            .Create();

        // tradeTicker
        var orderBookId = fixture.Create<OrderBookId>();
        var nanos = new Numeric32(){ Value = fixture.Create<uint>()};
        var metadata = fixture.Create<Metadata>();
        var price = new Price32(currentPrice);
        var action = fixture.Create<Action>();
        
        var quantity = new Numeric64(){ Value = fixture.Create<ulong>()};
        var tradeTicker = fixture.Build<TradeTickerMessageWrapper>()
            .With(x => x.Price, price)
            .With(x => x.Metadata, metadata)
            .With(x => x.Nanos, nanos)
            .With(x => x.OrderbookId, orderBookId)
            .With(x => x.TradeReportCode, (TradeReportCode)null)
            .With(x => x.DealId, (DealId)null)
            .With(x => x.DealSource, (DealSource)null)
            .With(x => x.Action, action)
            .With(x => x.Aggressor, (Alpha)null)
            .With(x=> x.Quantity, quantity)
            .Create();
        // Act
        var result = priceInfoMapperService.Map(tradeTicker, cachedPriceInfo, null);

        // Assert
        Assert.Equal(expectedPriceChange.ToString(), result.PriceChanged);
        
    }
    [Theory]
    [InlineData(65000, 68000, "-4.41")]
    [InlineData(68000, 65000, "4.62")]
    [InlineData(1000,800, "25.00")]
    public void Map_ReturnPriceInfoWithValidPriceChangeRate_WhenGotTradeTickerMessage(int currentPrice, int closedPrice, string expectedPriceChangeRate)
    {
        // Arrange
        var fixture = new Fixture();
        // cached price
        var cachedPriceInfo = fixture.Build<PriceInfo>()
            .With(x => x.PreClose, closedPrice.ToString())
            .With(x => x.Instrument, (Instrument)null)
            .Create();

        // tradeTicker
        var orderBookId = fixture.Create<OrderBookId>();
        var nanos = new Numeric32(){ Value = fixture.Create<uint>()};
        var metadata = fixture.Create<Metadata>();
        var price = new Price32(currentPrice);
        var action = fixture.Create<Action>();
        
        var quantity = new Numeric64(){ Value = fixture.Create<ulong>()};
        var tradeTicker = fixture.Build<TradeTickerMessageWrapper>()
            .With(x => x.Price, price)
            .With(x => x.Metadata, metadata)
            .With(x => x.Nanos, nanos)
            .With(x => x.OrderbookId, orderBookId)
            .With(x => x.TradeReportCode, (TradeReportCode)null)
            .With(x => x.DealId, (DealId)null)
            .With(x => x.DealSource, (DealSource)null)
            .With(x => x.Action, action)
            .With(x => x.Aggressor, (Alpha)null)
            .With(x=> x.Quantity, quantity)
            .Create();
        // Act
        var result = priceInfoMapperService.Map(tradeTicker, cachedPriceInfo, null);

        // Assert
        Assert.Equal(expectedPriceChangeRate, result.PriceChangedRate);
        
    }
    [Fact]
    public void Map_ReturnPriceInfoWithValidPreClosePrice_WhenGotReferencePriceMessage()
    {
        // Arrange
        var fixture = new Fixture();
        var orderBookId = fixture.Create<OrderBookId>();
        var nanos = fixture.Create<Nanos>();
        var metadata = fixture.Create<Metadata>();
        var price = new Price32(fixture.Create<int>());
        var priceType = new Numeric8();
        var msgQ = fixture.Build<ReferencePriceMessageWrapper>()
            .With(x => x.Price, price)
            .With(x => x.Metadata, metadata)
            .With(x => x.Nanos, nanos)
            .With(x => x.OrderBookId, orderBookId)
            .With(x => x.PriceType, priceType)
            .Create();
        var cachedPriceInfo = fixture.Build<PriceInfo>()
            .With(x => x.Instrument, (Instrument)null)
            .Create();
        
        // Act
        var result = priceInfoMapperService.Map(msgQ, cachedPriceInfo, null);

        // Assert
        Assert.Equal(price.ToString(), result.PreClose);

    }

    [Theory]
    [InlineData(null, 0, "0")]
    [InlineData("500", 500, "500")]
    [InlineData("000", 500, "250")]
    public void Map_ReturnPriceInfoWithValidAverageBuy_When_Got_TradeTickerMessage(string cachedPrice, int tickerPrice, string expectedAverageBuy)
    {
        // Arrange 
        var fixture = new Fixture();

        // cached price
        var cachedPriceInfo = fixture.Build<PriceInfo>()
            .With(x => x.AverageBuy, cachedPrice)
            .With(x => x.Instrument, (Instrument)null)
            .Create();
        
        // tradeTicker
        var orderBookId = fixture.Create<OrderBookId>();
        var nanos = new Numeric32(){ Value = fixture.Create<uint>()};
        var metadata = fixture.Create<Metadata>();
        var price = new Price32(tickerPrice);
        var action = fixture.Create<Action>();

        var quantity = new Numeric64(){ Value = fixture.Create<ulong>()};
        var tradeTicker = fixture.Build<TradeTickerMessageWrapper>()
            .With(x => x.Price, price)
            .With(x => x.Metadata, metadata)
            .With(x => x.Nanos, nanos)
            .With(x => x.OrderbookId, orderBookId)
            .With(x => x.TradeReportCode, (TradeReportCode)null)
            .With(x => x.DealId, (DealId)null)
            .With(x => x.DealSource, (DealSource)null)
            .With(x => x.Action, action)
            .With(x => x.Aggressor, new Alpha(Encoding.ASCII.GetBytes("A"),1))
            .With(x=> x.Quantity, quantity)
            .Create();
        
        // Act
        var result = priceInfoMapperService.Map(tradeTicker, cachedPriceInfo, null);

        // Assert
        Assert.Equal(expectedAverageBuy.ToString(), result.AverageBuy);
    }

    [Theory]
    [InlineData(null, 0, "0")]
    [InlineData("500", 500, "500")]
    [InlineData("000", 500, "250")]
    public void Map_ReturnPriceInfoWithValidAverageSell_When_Got_TradeTickerMessage(string cachedPrice, int tickerPrice, string expectedAverageSell)
    {
        // Arrange 
        var fixture = new Fixture();

        // cached price
        var cachedPriceInfo = fixture.Build<PriceInfo>()
            .With(x => x.AverageSell, cachedPrice)
            .With(x => x.Instrument, (Instrument)null)
            .Create();
        
        // tradeTicker
        var orderBookId = fixture.Create<OrderBookId>();
        var nanos = new Numeric32(){ Value = fixture.Create<uint>()};
        var metadata = fixture.Create<Metadata>();
        var price = new Price32(tickerPrice);
        var action = fixture.Create<Action>();

        var quantity = new Numeric64(){ Value = fixture.Create<ulong>()};
        var tradeTicker = fixture.Build<TradeTickerMessageWrapper>()
            .With(x => x.Price, price)
            .With(x => x.Metadata, metadata)
            .With(x => x.Nanos, nanos)
            .With(x => x.OrderbookId, orderBookId)
            .With(x => x.TradeReportCode, (TradeReportCode)null)
            .With(x => x.DealId, (DealId)null)
            .With(x => x.DealSource, (DealSource)null)
            .With(x => x.Action, action)
            .With(x => x.Aggressor, new Alpha(Encoding.ASCII.GetBytes("B"),1))
            .With(x=> x.Quantity, quantity)
            .Create();
        
        // Act
        var result = priceInfoMapperService.Map(tradeTicker, cachedPriceInfo, null);

        // Assert
        Assert.Equal(expectedAverageSell.ToString(), result.AverageSell);
    }
}
