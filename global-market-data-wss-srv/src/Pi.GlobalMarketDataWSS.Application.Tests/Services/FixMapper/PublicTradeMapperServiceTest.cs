using AutoFixture;
using Pi.GlobalMarketDataWSS.Application.Constants;
using Pi.GlobalMarketDataWSS.Application.Services.FixMapper;
using Pi.GlobalMarketDataWSS.Domain.Entities;
using Pi.GlobalMarketDataWSS.Domain.Models.Fix;

namespace Pi.GlobalMarketDataWSS.Application.Tests.Services.FixMapper;
public class PublicTradeMapperServiceTest
{
    public PublicTradeMapperServiceTest()
    {
        
    }

    [Fact]
    public void PublicTradeMapperService_ReturnPublicTrade_WhenGotValidEntry()
    {
        // Arrange
        var fixture = new Fixture();
        var expectedDate = fixture.Create<DateTime>();
        var expectedPrice = fixture.Create<decimal>();
        var expectedQuantity = fixture.Create<decimal>();

        var entry = fixture.Build<Entry>()
            .With(e => e.MdEntryType, FixMessageType.Trade)
            .With(e => e.MdEntryPx, expectedPrice)
            .With(e => e.MdEntrySize, expectedQuantity)
            .With(e => e.MdEntryDate, expectedDate).Create();

        // Act
        var result = PublicTradeMapperService.Map(entry);
        
        // Assert 
        Assert.IsType(typeof(PublicTrade), result);
        Assert.Equal(expectedPrice.ToString(), result.Price);
        Assert.Equal(expectedQuantity.ToString(), result.Quantity);
    }
}