using Pi.SetMarketData.Application.Services.MarketData.MarketTicker;
using Pi.SetMarketData.Domain.Entities;
using Pi.SetMarketData.Domain.Models.Response;

namespace Pi.SetMarketData.Application.Tests.Services.MarketTicker
{
    public class MarketTickerServiceTest
    {
        [Fact]
        public void GetResult_Should_Return_MarketTickerResponse()
        {
            // Arrange
            var marketStreaming = new List<MarketStreamingResponse>();
            var instruments = new List<Instrument>();
            var instrumentDetails = new List<InstrumentDetail>();
            var morningStarStocks = new List<MorningStarStocks>();
            var exchangeTimezones = new List<ExchangeTimezone>();
            var high52WList = new List<double>();
            var low52WList = new List<double>();
            var logos = new List<string>();

            // Act
            var result = new MarketTickerService().GetResult(
                marketStreaming,
                new MarketTickerServiceParams
                {
                    High52WList = high52WList,
                    Low52WList = low52WList
                },
                instruments,
                instrumentDetails,
                morningStarStocks,
                exchangeTimezones,
                logos
            );

            // Assert
            Assert.NotNull(result);
            Assert.IsType<MarketTickerResponse>(result);
            Assert.Equal("0", result.Code);
            Assert.Equal("", result.Message);
        }
    }
}
