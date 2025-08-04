using Pi.GlobalMarketDataWSS.Domain.Models.Response;
namespace Pi.GlobalMarketDataWSS.Domain.Tests.Models.Response;
public class MarketInstrumentSearchRequestTest
{
    public MarketInstrumentSearchRequestTest()
    {
        
    }
    [Fact]
    public void InstrumentSearchCategoryList_Should_Initialize_With_Default_Values()
    {
        // Arrange & Act
        var categoryList = new InstrumentSearchCategoryList();

        // Assert
        Assert.Equal(0, categoryList.Order);
        Assert.Null(categoryList.InstrumentType);
        Assert.Null(categoryList.InstrumentCategory);
        Assert.Null(categoryList.InstrumentList);
    }

    [Fact]
    public void InstrumentSearchList_Should_Initialize_With_Default_Values()
    {
        // Arrange & Act
        var instrumentList = new InstrumentSearchList();

        // Assert
        Assert.Null(instrumentList.Venue);
        Assert.Null(instrumentList.Symbol);
        Assert.Null(instrumentList.FriendlyName);
        Assert.Null(instrumentList.Logo);
        Assert.Null(instrumentList.Price);
        Assert.Null(instrumentList.PriceChange);
        Assert.Null(instrumentList.PriceChangeRatio);
        Assert.False(instrumentList.IsFavorite);
        Assert.Null(instrumentList.Unit);
    }

    [Fact]
    public void InstrumentSearchResponse_Should_Initialize_With_Default_Values()
    {
        // Arrange & Act
        var response = new InstrumentSearchResponse();

        // Assert
        Assert.Null(response.InstrumentCategoryList);
    }

    [Fact]
    public void MarketInstrumentSearchResponse_Should_Initialize_With_Default_Values()
    {
        // Arrange & Act
        var response = new MarketInstrumentSearchResponse();

        // Assert
        Assert.Null(response.Code);
        Assert.Null(response.Message);
        Assert.Null(response.Response);
        Assert.Null(response.DebugStack);
    }

    [Fact]
    public void InstrumentSearchCategoryList_Should_Set_Properties_Correctly()
    {
        // Arrange
        var instrumentList = new List<InstrumentSearchList>
        {
            new InstrumentSearchList { Venue = "NASDAQ", Symbol = "AAPL", FriendlyName = "Apple Inc.", Price = "150.00" },
            new InstrumentSearchList { Venue = "NYSE", Symbol = "TSLA", FriendlyName = "Tesla Inc.", Price = "700.00" }
        };

        // Act
        var categoryList = new InstrumentSearchCategoryList
        {
            Order = 1,
            InstrumentType = "Stock",
            InstrumentCategory = "Technology",
            InstrumentList = instrumentList
        };

        // Assert
        Assert.Equal(1, categoryList.Order);
        Assert.Equal("Stock", categoryList.InstrumentType);
        Assert.Equal("Technology", categoryList.InstrumentCategory);
        Assert.Equal(instrumentList, categoryList.InstrumentList);
    }

    [Fact]
    public void InstrumentSearchList_Should_Set_Properties_Correctly()
    {
        // Arrange & Act
        var instrumentList = new InstrumentSearchList
        {
            Venue = "NASDAQ",
            Symbol = "AAPL",
            FriendlyName = "Apple Inc.",
            Logo = "apple_logo.png",
            Price = "150.00",
            PriceChange = "1.00",
            PriceChangeRatio = "0.67%",
            IsFavorite = true,
            Unit = "USD"
        };

        // Assert
        Assert.Equal("NASDAQ", instrumentList.Venue);
        Assert.Equal("AAPL", instrumentList.Symbol);
        Assert.Equal("Apple Inc.", instrumentList.FriendlyName);
        Assert.Equal("apple_logo.png", instrumentList.Logo);
        Assert.Equal("150.00", instrumentList.Price);
        Assert.Equal("1.00", instrumentList.PriceChange);
        Assert.Equal("0.67%", instrumentList.PriceChangeRatio);
        Assert.True(instrumentList.IsFavorite);
        Assert.Equal("USD", instrumentList.Unit);
    }

    [Fact]
    public void InstrumentSearchResponse_Should_Set_Properties_Correctly()
    {
        // Arrange
        var instrumentCategoryList = new List<InstrumentSearchCategoryList>
        {
            new InstrumentSearchCategoryList { Order = 1, InstrumentType = "Stock", InstrumentCategory = "Technology" }
        };

        // Act
        var response = new InstrumentSearchResponse
        {
            InstrumentCategoryList = instrumentCategoryList
        };

        // Assert
        Assert.Equal(instrumentCategoryList, response.InstrumentCategoryList);
    }

    [Fact]
    public void MarketInstrumentSearchResponse_Should_Set_Properties_Correctly()
    {
        // Arrange
        var instrumentResponse = new InstrumentSearchResponse
        {
            InstrumentCategoryList = new List<InstrumentSearchCategoryList>
            {
                new InstrumentSearchCategoryList { Order = 1, InstrumentType = "Stock", InstrumentCategory = "Technology" }
            }
        };

        // Act
        var response = new MarketInstrumentSearchResponse
        {
            Code = "200",
            Message = "Success",
            Response = instrumentResponse,
            DebugStack = "Debug info here"
        };

        // Assert
        Assert.Equal("200", response.Code);
        Assert.Equal("Success", response.Message);
        Assert.Equal(instrumentResponse, response.Response);
        Assert.Equal("Debug info here", response.DebugStack);
    }
}