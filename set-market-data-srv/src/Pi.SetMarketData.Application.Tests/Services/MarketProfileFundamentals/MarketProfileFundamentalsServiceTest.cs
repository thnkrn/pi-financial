using Pi.SetMarketData.Application.Constants;
using Pi.SetMarketData.Application.Services.MarketData.MarketProfileFundamentals;
using Pi.SetMarketData.Domain.Entities;
using Pi.SetMarketData.Domain.Models.Response;

namespace Pi.SetMarketData.Application.Tests.Services.MarketProfileFundamentals;

public class MarketProfileFundamentalsServiceTest
{
    [Fact]
    public void GetResult_ShouldReturnValidResponse_ForThaiStockWarrantsInstrument()
    {
        // Arrange
        var instrument = new Instrument
        {
            Symbol = "NER-W2",
            InstrumentCategory = InstrumentConstants.ThaiStockWarrants,
            ExerciseDate = "15/05/2026",
            ExercisePrice = "5.500000000000"
        };
        var underlyingInstrument = new Instrument();
        var instrumentDetail = new InstrumentDetail();
        var priceInfo = new PriceInfo();
        var marketStreamingResponse = new MarketStreamingResponse();
        var morningStarStocks = new MorningStarStocks();

        // Act
        // var result = new MarketProfileFundamentalsService()
        //     //.SetIsTfex(true)
        //     .SetMorningStar(morningStarStocks)
        //     .SetInstrument(instrument, underlyingInstrument)
        //     .SetInstrumentDetail(instrumentDetail)
        //     .SetPriceInfo(priceInfo)
        //     .SetMarketStreaming(marketStreamingResponse)
        //     .GetResult();
        //
        // // Assert
        // Assert.Equal("15 May 2026", result?.Response?.ExerciseDate);
        // Assert.Equal("15 May 2026", result?.Response?.MaturityDate);
        // Assert.Equal("5.5", result?.Response?.ExercisePrice);
    }

    [Fact]
    public void GetResult_ShouldReturnValidResponse_ForThaiDerivativesWarrantsInstrument()
    {
        // Arrange
        var instrument = new Instrument
        {
            Symbol = "SET5041P2412T",
            InstrumentCategory = "Thai Derivatives Warrants",
            ExerciseDate = "03/01/2025",
            LastTradingDate = "27/12/2024",
            ExercisePrice = "700.000000000000",
            MaturityDate = "03/01/2025",
            IssuerSeries = "JPM / T"
        };
        var underlyingInstrument = new Instrument
        {
            Symbol = "SET",
            Venue = "Equity",
            InstrumentType = "Equity",
            InstrumentCategory = "SET Indices",
            FriendlyName = "THE STOCK EXCHANGE OF THAILAND",
            LongName = "SET_SET Index"
        };
        var instrumentDetail = new InstrumentDetail();
        var priceInfo = new PriceInfo();
        var marketStreamingResponse = new MarketStreamingResponse();
        var morningStarStocks = new MorningStarStocks();

        // Act
        // var result = new MarketProfileFundamentalsService()
        //     //.SetIsTfex(true)
        //     .SetMorningStar(morningStarStocks)
        //     .SetInstrument(instrument, underlyingInstrument)
        //     .SetInstrumentDetail(instrumentDetail)
        //     .SetPriceInfo(priceInfo)
        //     .SetMarketStreaming(marketStreamingResponse)
        //     .GetResult();
        //
        // // Assert
        // Assert.Equal("", result?.Response?.ExerciseDate);
        // Assert.Equal("", result?.Response?.DaysToExercise);
        // Assert.Equal("700", result?.Response?.ExercisePrice);
        // Assert.Equal("03 Jan 2025", result?.Response?.MaturityDate);
        // Assert.Equal("27 Dec 2024", result?.Response?.LastTradingDate);
        // Assert.Equal("JPM / T", result?.Response?.IssuerSeries);
        //
        // Assert.Equal("SET", result?.Response?.UnderlyingSymbol);
        // Assert.Equal("Equity", result?.Response?.UnderlyingVenue);
        // Assert.Equal("Equity", result?.Response?.UnderlyingInstrumentType);
        // Assert.Equal("SET Indices", result?.Response?.UnderlyingInstrumentCategory);
    }

    [Theory]
    [InlineData("5.0", "5.50", MoneynessConstants.OTM)]
    [InlineData("5.5", "5.50", MoneynessConstants.ATM)]
    [InlineData("6.0", "5.50", MoneynessConstants.ITM)]
    [InlineData(null, "5.50", "")]
    [InlineData("6.0", null, "")]
    public void GetResult_ShouldReturnCorrectMoneyness_ForThaiStockWarrantsInstrument(string currentPrice,
        string exercisePrice, string expectedMoneyness)
    {
        // Arrange
        var instrument = new Instrument
        {
            InstrumentCategory = InstrumentConstants.ThaiStockWarrants,
            ExercisePrice = exercisePrice
        };
        var underlyingInstrument = new Instrument();
        var instrumentDetail = new InstrumentDetail();
        var priceInfo = new PriceInfo();
        var marketStreamingResponse = new MarketStreamingResponse
        {
            Response = new StreamingResponse
            {
                Data = new List<StreamingBody>
                {
                    new()
                    {
                        Underlying = currentPrice
                    }
                }
            }
        };
        var morningStarStocks = new MorningStarStocks();

        // Act
        // var result = new MarketProfileFundamentalsService()
        //     //.SetIsTfex(true)
        //     .SetMorningStar(morningStarStocks)
        //     .SetInstrument(instrument, underlyingInstrument)
        //     .SetInstrumentDetail(instrumentDetail)
        //     .SetPriceInfo(priceInfo)
        //     .SetMarketStreaming(marketStreamingResponse)
        //     .GetResult();
        //
        // // Assert
        // Assert.Equal(expectedMoneyness, result?.Response?.Moneyness);
    }

    [Theory]
    [InlineData("5.0", "5.50", "Call", MoneynessConstants.OTM)]
    [InlineData("5.5", "5.50", "Call", MoneynessConstants.ATM)]
    [InlineData("6.0", "5.50", "Call", MoneynessConstants.ITM)]
    [InlineData("5.0", "5.50", "Put", MoneynessConstants.ITM)]
    [InlineData("5.5", "5.50", "Put", MoneynessConstants.ATM)]
    [InlineData("6.0", "5.50", "Put", MoneynessConstants.OTM)]
    [InlineData("6.0", "5.50", "", "")]
    [InlineData(null, "5.50", "", "")]
    [InlineData("6.0", null, "", "")]
    public void GetResult_ShouldReturnCorrectMoneyness_ForThaiDerivativesWarrantsInstrument
    (
        string currentPrice,
        string exercisePrice,
        string direction,
        string expectedMoneyness
    )
    {
        // Arrange
        var instrument = new Instrument
        {
            InstrumentCategory = "Thai Derivatives Warrants",
            Direction = direction,
            ExercisePrice = exercisePrice
        };
        var underlyingInstrument = new Instrument();
        var instrumentDetail = new InstrumentDetail();
        var marketStreamingResponse = new MarketStreamingResponse
        {
            Response = new StreamingResponse
            {
                Data = new List<StreamingBody>
                {
                    new()
                    {
                        Price = currentPrice
                    }
                }
            }
        };
        var morningStarStocks = new MorningStarStocks();

        // Act
        var result = new MarketProfileFundamentalsService()
            //.SetIsTfex(true)
            .SetMorningStar(morningStarStocks)
            .SetInstrument(instrument, underlyingInstrument)
            .SetInstrumentDetail(instrumentDetail)
            .SetUnderlying(marketStreamingResponse)
            .GetResult();

        // Assert
        Assert.Equal(expectedMoneyness, result?.Response?.Moneyness);
    }
}