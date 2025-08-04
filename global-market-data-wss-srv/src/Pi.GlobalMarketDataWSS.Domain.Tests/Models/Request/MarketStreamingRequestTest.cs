using Xunit;
using Pi.GlobalMarketDataWSS.Domain.Models.Request;
namespace Pi.GlobalMarketDataWSS.Domain.Tests.Models.Request;
public class MarketStreamingRequestTest
{
    public MarketStreamingRequestTest()
    {
        
    }

    [Fact]
    public void Data_Should_Initialize_With_Default_Values()
    {
        // Arrange & Act
        var data = new Data();

        // Assert
        Assert.Null(data.Param);
        Assert.Null(data.SubscribeType);
    }

    [Fact]
    public void MarketStreamingParameter_Should_Initialize_With_Default_Values()
    {
        // Arrange & Act
        var marketStreamingParameter = new MarketStreamingParameter();

        // Assert
        Assert.Null(marketStreamingParameter.Market);
        Assert.Null(marketStreamingParameter.Symbol);
    }

    [Fact]
    public void MarketStreamingRequest_Should_Initialize_With_Default_Values()
    {
        // Arrange & Act
        var marketStreamingRequest = new MarketStreamingRequest();

        // Assert
        Assert.Null(marketStreamingRequest.Data);
        Assert.Null(marketStreamingRequest.Op);
        Assert.Null(marketStreamingRequest.SessionId);
    }
    [Fact]
    public void Data_Should_Set_Properties_Correctly()
    {
        // Arrange
        var parameters = new List<MarketStreamingParameter>
        {
            new MarketStreamingParameter { Market = "US", Symbol = "AAPL" },
            new MarketStreamingParameter { Market = "EU", Symbol = "BMW" }
        };

        // Act
        var data = new Data
        {
            Param = parameters,
            SubscribeType = "RealTime"
        };

        // Assert
        Assert.Equal(parameters, data.Param);
        Assert.Equal("RealTime", data.SubscribeType);
    }

    [Fact]
    public void MarketStreamingParameter_Should_Set_Properties_Correctly()
    {
        // Arrange & Act
        var marketStreamingParameter = new MarketStreamingParameter
        {
            Market = "US",
            Symbol = "AAPL"
        };

        // Assert
        Assert.Equal("US", marketStreamingParameter.Market);
        Assert.Equal("AAPL", marketStreamingParameter.Symbol);
    }

    [Fact]
    public void MarketStreamingRequest_Should_Set_Properties_Correctly()
    {
        // Arrange
        var data = new Data
        {
            Param = new List<MarketStreamingParameter>
            {
                new MarketStreamingParameter { Market = "US", Symbol = "AAPL" }
            },
            SubscribeType = "RealTime"
        };

        // Act
        var marketStreamingRequest = new MarketStreamingRequest
        {
            Data = data,
            Op = "subscribe",
            SessionId = "session123"
        };

        // Assert
        Assert.Equal(data, marketStreamingRequest.Data);
        Assert.Equal("subscribe", marketStreamingRequest.Op);
        Assert.Equal("session123", marketStreamingRequest.SessionId);
    }
}