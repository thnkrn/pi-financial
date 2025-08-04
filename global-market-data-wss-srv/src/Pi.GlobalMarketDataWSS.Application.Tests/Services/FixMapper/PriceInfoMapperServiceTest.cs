using Pi.GlobalMarketDataWSS.Application.Services.FixMapper;
using Pi.GlobalMarketDataWSS.Domain.Entities;
using AutoFixture;
using Pi.GlobalMarketDataWSS.Domain.Models.Response;
using MongoDB.Bson;
using Pi.GlobalMarketDataWSS.Domain.Models.Fix;
using Pi.GlobalMarketDataWSS.Application.Constants;
namespace Pi.GlobalMarketDataWSS.Application.Tests.Services.FixMapper;
public class PriceInfoMapperServicesTest
{
    private readonly PriceInfoMapperService priceInfoMapperService;
    public PriceInfoMapperServicesTest()
    {
        priceInfoMapperService= new PriceInfoMapperService();
    }
    [Fact]
    public void Map_ReturnStreaminBodyWithAuctionPrice_WhenMarketSessionIsPreMarket()
    {
        // Arrange
        var fixture = new Fixture();

        Entry entry = fixture.Build<Entry>()
            .With(e => e.MdEntryTime, DateTime.Now)
            .With(e => e.MdEntryType, FixMessageType.Trade).Create();
        string marketSession = MarketSession.PreMarket;
        StreamingBody streamingBody = new StreamingBody();

        // Act
        var result = priceInfoMapperService.Map(streamingBody, marketSession, entry);

        // Assert
        Assert.Equal(entry.MdEntryPx.ToString(), result.AuctionPrice);
        Assert.Equal(entry.MdEntrySize.ToString(), result.AuctionVolume);
        Assert.True(result.IsProjected);
    }
    [Fact]
    public void Map_ReturnStreaminBodyWithPriorClose_WhenMarketSessionIsPreMarket()
    {
        // Arrange
        var fixture = new Fixture();

        Entry entry = fixture.Build<Entry>()
            .With(e => e.MdEntryTime, new DateTime(2025, 2, 5, 16, 21, 02))
            .With(e => e.MdEntryType, FixMessageType.ClosingPrice).Create();
        string marketSession = MarketSession.PreMarket;
        StreamingBody streamingBody = new StreamingBody();

        // Act
        var result = priceInfoMapperService.Map(streamingBody, marketSession, entry);

        // Assert
        Assert.Equal(entry.MdEntryPx.ToString(), result.PriorClose);
        Assert.NotEqual(entry.MdEntryPx.ToString(), result.PreClose);
    }
    
}