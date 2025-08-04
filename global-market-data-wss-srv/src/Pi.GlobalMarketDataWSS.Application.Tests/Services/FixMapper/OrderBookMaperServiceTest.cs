using AutoFixture;
using Pi.GlobalMarketDataWSS.Application.Constants;
using Pi.GlobalMarketDataWSS.Application.Services.FixMapper;
using Pi.GlobalMarketDataWSS.Domain.Entities;
using Pi.GlobalMarketDataWSS.Domain.Models.Fix;

namespace Pi.GlobalMarketDataWSS.Application.Tests.Services.FixMapper;
public class OrderBookMapperServiceTest
{
    public OrderBookMapperServiceTest()
    {
    }

    [Fact]
    public void Map_ReturnValidOrderBook_WhenGotBidMessage()
    {
        var fixture = new Fixture();
        var expectedBidPrice = fixture.Create<decimal>();
        var expectedBidQuantity = fixture.Create<decimal>();

        var entry = fixture.Build<Entry>()
            .With(e => e.MdEntryType, FixMessageType.Bid)
            .With(e => e.MdEntryPx, expectedBidPrice)
            .With(e => e.MdEntrySize, expectedBidQuantity)
            .Create();
            
        // Act
        var orderBook = OrderBookMapperService.Map(entry);

        // Assert
        Assert.NotNull(orderBook);
        Assert.NotEmpty(orderBook.BidPrice);
        Assert.NotEmpty(orderBook.BidQuantity);
        Assert.Equal(orderBook.BidPrice, expectedBidPrice.ToString());
        Assert.Equal(orderBook.BidQuantity, expectedBidQuantity.ToString());
        Assert.Null(orderBook.OfferPrice);
        Assert.Null(orderBook.OfferQuantity);
    }

    [Fact]
    public void Map_ReturnValidOrderBook_WhenGotOfferMessage()
    {
        var fixture = new Fixture();
        var expectedOfferPrice = fixture.Create<decimal>();
        var expectedOfferQuantity = fixture.Create<decimal>();

        var entry = fixture.Build<Entry>()
            .With(e => e.MdEntryType, FixMessageType.Offer)
            .With(e => e.MdEntryPx, expectedOfferPrice)
            .With(e => e.MdEntrySize, expectedOfferQuantity)
            .Create();
            
        // Act
        var orderBook = OrderBookMapperService.Map(entry);

        // Assert
        Assert.NotNull(orderBook);
        Assert.NotEmpty(orderBook.OfferPrice);
        Assert.NotEmpty(orderBook.OfferQuantity);
        Assert.Equal(orderBook.OfferPrice, expectedOfferPrice.ToString());
        Assert.Equal(orderBook.OfferQuantity, expectedOfferQuantity.ToString());
        Assert.Null(orderBook.BidPrice);
        Assert.Null(orderBook.BidQuantity);
    }
}