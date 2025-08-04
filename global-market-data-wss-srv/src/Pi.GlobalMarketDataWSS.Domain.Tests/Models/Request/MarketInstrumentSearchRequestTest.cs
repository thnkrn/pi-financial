using Pi.GlobalMarketDataWSS.Domain.Models.Request;
namespace Pi.GlobalMarketDataWSS.Domain.Tests.Models.Request;
public class MarketInstrumentSearchRequestTest
{
    public MarketInstrumentSearchRequestTest()
    {
        
    }
    
    [Fact]
    public void MarketInstrumentSearchRequest_Should_Initialize_With_Default_Values()
    {
        // Arrange & Act
        var request = new MarketInstrumentSearchRequest();

        // Assert
        Assert.Null(request.InstrumentType);
        Assert.Null(request.Keyword);
    }

    [Fact]
    public void MarketInstrumentSearchRequest_Should_Set_Properties_Correctly()
    {
        // Arrange & Act
        var request = new MarketInstrumentSearchRequest
        {
            InstrumentType = "Stock",
            Keyword = "AAPL"
        };

        // Assert
        Assert.Equal("Stock", request.InstrumentType);
        Assert.Equal("AAPL", request.Keyword);
    }
}