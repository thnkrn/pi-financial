using Xunit;
using Pi.SetMarketData.Domain.Entities;
using Pi.SetMarketData.Application.Services;

namespace Pi.SetMarketData.Tests.Services
{
    public class MarketGlobalEquityInstrumentInfoServiceTests
    {
        [Fact]
        public void GetResult_ReturnsExpectedResponse_WhenGotValidInstrument()
        {
            // Arrange
            var instrument = new Instrument
            {
                MinBidUnit = "0.01",
                TradingUnit = "1",
                Currency = "USD"
            };

            // Act
            var result = MarketGlobalEquityInstrumentInfoService.GetResult(instrument);

            // Assert
            Assert.Equal("0", result.Code);
            Assert.Equal(string.Empty, result.Message);
            Assert.Equal("0.01", result.Response?.MinimalPriceIncrement);
            Assert.Equal("1", result.Response?.MinimalQuantityIncrement);
            Assert.Equal("USD", result.Response?.Currency);
        }
        [Fact]
        public void GetResult_ReturnsZeroValueCasted_WhenInstrumentPropertyIsNull()
        {
            // Arrange
            var instrument = new Instrument
            {
                MinBidUnit = null,
                TradingUnit = null,
                Currency = "USD"
            };

            // Act
            var result = MarketGlobalEquityInstrumentInfoService.GetResult(instrument);

            // Assert
            Assert.Equal("0", result.Code);
            Assert.Equal(string.Empty, result.Message);
            Assert.Equal("0.00", result.Response?.MinimalPriceIncrement);
            Assert.Equal("0", result.Response?.MinimalQuantityIncrement);
        }
    }
}