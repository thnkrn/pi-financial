using Pi.GlobalMarketDataWSS.Domain.Models.Response;

namespace Pi.GlobalMarketDataWSS.Domain.Tests.Models.Response;

public class MarketStreamingResponseTest
{
    public MarketStreamingResponseTest() { }

    [Fact]
    public void StreamingBody_Should_Initialize_With_Default_Values()
    {
        // Arrange & Act
        var streamingBody = new StreamingBody();

        // Assert
        Assert.Empty(streamingBody.Symbol);
        Assert.Empty(streamingBody.Venue);
        Assert.Empty(streamingBody.Price);
        Assert.Empty(streamingBody.AuctionPrice);
        Assert.Empty(streamingBody.AuctionVolume);
        Assert.False(streamingBody.IsProjected);
        Assert.Equal(0, streamingBody.LastPriceTime);
        Assert.Empty(streamingBody.Open);
        Assert.Empty(streamingBody.High24H);
        Assert.Empty(streamingBody.Low24H);
        Assert.Empty(streamingBody.PriceChanged);
        Assert.Empty(streamingBody.PriceChangedRate);
        Assert.Empty(streamingBody.Volume);
        Assert.Empty(streamingBody.Amount);
        Assert.Empty(streamingBody.TotalAmount);
        Assert.Empty(streamingBody.TotalAmountK);
        Assert.Empty(streamingBody.TotalVolume);
        Assert.Empty(streamingBody.TotalVolumeK);
        Assert.Empty(streamingBody.Open1);
        Assert.Empty(streamingBody.Open2);
        Assert.Empty(streamingBody.Ceiling);
        Assert.Empty(streamingBody.Floor);
        Assert.Empty(streamingBody.Average);
        Assert.Empty(streamingBody.AverageBuy);
        Assert.Empty(streamingBody.AverageSell);
        Assert.Empty(streamingBody.Aggressor);
        Assert.Empty(streamingBody.PreClose);
        Assert.Empty(streamingBody.Status);
        Assert.Empty(streamingBody.Yield);
        Assert.Empty(streamingBody.PublicTrades);
        Assert.NotNull(streamingBody.OrderBook);
        Assert.Empty(streamingBody.SecurityType);
        Assert.Empty(streamingBody.InstrumentType);
        Assert.Empty(streamingBody.Market);
        Assert.Empty(streamingBody.LastTrade);
        Assert.Equal(0, streamingBody.ToLastTrade);
        Assert.Empty(streamingBody.Moneyness);
        Assert.Empty(streamingBody.MaturityDate);
        Assert.Empty(streamingBody.Multiplier);
        Assert.Empty(streamingBody.ExercisePrice);
        Assert.Empty(streamingBody.IntrinsicValue);
        Assert.Empty(streamingBody.PSettle);
        Assert.Empty(streamingBody.Poi);
        Assert.Empty(streamingBody.Underlying);
        Assert.Empty(streamingBody.Open0);
        Assert.Empty(streamingBody.Basis);
        Assert.Empty(streamingBody.Settle);
    }

    [Fact]
    public void StreamingOrderBook_Should_Initialize_With_Default_Values()
    {
        // Arrange & Act
        var orderBook = new StreamingOrderBook();

        // Assert
        Assert.Empty(orderBook.Bid);
        Assert.Empty(orderBook.Offer);
    }

    [Fact]
    public void StreamingResponse_Should_Initialize_With_Default_Values()
    {
        // Arrange & Act
        var streamingResponse = new StreamingResponse();

        // Assert
        Assert.Empty(streamingResponse.Data);
    }

    [Fact]
    public void MarketStreamingResponse_Should_Initialize_With_Default_Values()
    {
        // Arrange & Act
        var marketStreamingResponse = new MarketStreamingResponse();

        // Assert
        Assert.Empty(marketStreamingResponse.Code);
        Assert.Empty(marketStreamingResponse.Op);
        Assert.Empty(marketStreamingResponse.Message);
        Assert.NotNull(marketStreamingResponse.Response);
    }

    [Fact]
    public void StreamingBody_Should_Set_Properties_Correctly()
    {
        // Arrange & Act
        var streamingBody = new StreamingBody
        {
            Symbol = "AAPL",
            Venue = "NASDAQ",
            Price = "150.00",
            AuctionPrice = "149.00",
            AuctionVolume = "1000",
            IsProjected = true,
            LastPriceTime = 1234567890,
            Open = "149.50",
            High24H = "152.00",
            Low24H = "148.00",
            PriceChanged = "1.50",
            PriceChangedRate = "1.01",
            Volume = "5000",
            Amount = "750000",
            TotalAmount = "1000000",
            TotalAmountK = "1000",
            TotalVolume = "8000",
            TotalVolumeK = "8",
            Open1 = "150.00",
            Open2 = "149.50",
            Ceiling = "155.00",
            Floor = "145.00",
            Average = "150.25",
            AverageBuy = "150.00",
            AverageSell = "150.50",
            Aggressor = "Buy",
            PreClose = "149.00",
            Status = "Open",
            Yield = "0.5%",
            PublicTrades = new List<List<object>>
            {
                new List<object> { "Trade1" },
                new List<object> { "Trade2" }
            },
            OrderBook = new StreamingOrderBook
            {
                Bid = new List<List<string>>
                {
                    new List<string> { "100", "150.00" }
                },
                Offer = new List<List<string>>
                {
                    new List<string> { "100", "150.50" }
                }
            },
            SecurityType = "Stock",
            InstrumentType = "Equity",
            Market = "US",
            LastTrade = "150.00",
            ToLastTrade = 5,
            Moneyness = "At The Money",
            MaturityDate = "2025-12-31",
            Multiplier = "1",
            ExercisePrice = "150.00",
            IntrinsicValue = "0",
            PSettle = "149.75",
            Poi = "500",
            Underlying = "AAPL",
            Open0 = "149.00",
            Basis = "0.25",
            Settle = "149.75"
        };

        // Assert
        Assert.Equal("AAPL", streamingBody.Symbol);
        Assert.Equal("NASDAQ", streamingBody.Venue);
        Assert.Equal("150.00", streamingBody.Price);
        Assert.Equal("149.00", streamingBody.AuctionPrice);
        Assert.Equal("1000", streamingBody.AuctionVolume);
        Assert.True(streamingBody.IsProjected);
        Assert.Equal(1234567890, streamingBody.LastPriceTime);
        Assert.Equal("149.50", streamingBody.Open);
        Assert.Equal("152.00", streamingBody.High24H);
        Assert.Equal("148.00", streamingBody.Low24H);
        Assert.Equal("1.50", streamingBody.PriceChanged);
        Assert.Equal("1.01", streamingBody.PriceChangedRate);
        Assert.Equal("5000", streamingBody.Volume);
        Assert.Equal("750000", streamingBody.Amount);
        Assert.Equal("1000000", streamingBody.TotalAmount);
        Assert.Equal("1000", streamingBody.TotalAmountK);
        Assert.Equal("8000", streamingBody.TotalVolume);
        Assert.Equal("8", streamingBody.TotalVolumeK);
        Assert.Equal("150.00", streamingBody.Open1);
        Assert.Equal("149.50", streamingBody.Open2);
        Assert.Equal("155.00", streamingBody.Ceiling);
        Assert.Equal("145.00", streamingBody.Floor);
        Assert.Equal("150.25", streamingBody.Average);
        Assert.Equal("150.00", streamingBody.AverageBuy);
        Assert.Equal("150.50", streamingBody.AverageSell);
        Assert.Equal("Buy", streamingBody.Aggressor);
        Assert.Equal("149.00", streamingBody.PreClose);
        Assert.Equal("Open", streamingBody.Status);
        Assert.Equal("0.5%", streamingBody.Yield);
        Assert.Equal(
            new List<List<object>>
            {
                new List<object> { "Trade1" },
                new List<object> { "Trade2" }
            },
            streamingBody.PublicTrades
        );
        Assert.NotNull(streamingBody.OrderBook);
        Assert.Equal(
            new List<List<string>>
            {
                new List<string> { "100", "150.00" }
            },
            streamingBody.OrderBook.Bid
        );
        Assert.Equal(
            new List<List<string>>
            {
                new List<string> { "100", "150.50" }
            },
            streamingBody.OrderBook.Offer
        );
        Assert.Equal("Stock", streamingBody.SecurityType);
        Assert.Equal("Equity", streamingBody.InstrumentType);
        Assert.Equal("US", streamingBody.Market);
        Assert.Equal("150.00", streamingBody.LastTrade);
        Assert.Equal(5, streamingBody.ToLastTrade);
        Assert.Equal("At The Money", streamingBody.Moneyness);
        Assert.Equal("2025-12-31", streamingBody.MaturityDate);
        Assert.Equal("1", streamingBody.Multiplier);
        Assert.Equal("150.00", streamingBody.ExercisePrice);
        Assert.Equal("0", streamingBody.IntrinsicValue);
        Assert.Equal("149.75", streamingBody.PSettle);
        Assert.Equal("500", streamingBody.Poi);
        Assert.Equal("AAPL", streamingBody.Underlying);
        Assert.Equal("149.00", streamingBody.Open0);
        Assert.Equal("0.25", streamingBody.Basis);
        Assert.Equal("149.75", streamingBody.Settle);
    }
}
