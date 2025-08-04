using Pi.SetMarketData.Application.Services.MarketDataController.MarketProfileOverviewService;
using Pi.SetMarketData.Domain.Entities;
using Pi.SetMarketData.Domain.Models.Response;

namespace Pi.SetMarketData.Application.Tests.Services.MarketProfileOverview
{
    public class MarketProfileOverviewServiceTest
    {
        [Fact]
        public void GetResult_ShouldReturnValidResponse_WhenGivenValidInputs()
        {
            // Arrange
            var instrument = new Instrument { Market = "SET", Currency = "THB", InstrumentCategory = "SET" };
            var marketProfileParams = new MarketProfileOverviewParams { High52W = 150.50, Low52W = 120.25 };
            var setVenueMapping = new SetVenueMapping { Exchange = "SET" };
            var corporateAction = new CorporateAction { Code = "XA,XB", Date = "2024-06-26" };
            var tradingSign = new TradingSign { Sign = "L,HALT_E" };
            var marketStreaming = new MarketStreamingResponse
            {
                Response = new StreamingResponse
                {
                    Data = new List<StreamingBody>
                    {
                        new StreamingBody
                        {
                            Price = "145.50",
                            PreClose = "144.00",
                            PriceChanged = "1.50",
                            PriceChangedRate = "1.04",
                            LastTrade = "27/12/2024"
                        }
                    }
                }
            };

            // Act
            var result = new MarketProfileOverviewService()
                .WithInstruments(instrument)
                .WithMarketProfileParams(marketProfileParams)
                .WithVenueMapping(setVenueMapping)
                .WithCorporateAction(corporateAction)
                .WithTradingSign(tradingSign)
                .WithMarketStreamingResponse(marketStreaming)
                .WithIsTfex(true).GetResult();

            // Assert
            Assert.NotNull(result);
            Assert.Equal("0", result.Code);
            Assert.Equal(string.Empty, result.Message);
            Assert.NotNull(result.Response);
            Assert.Equal("SET", result.Response.Market);
            Assert.Equal("SET", result.Response.Exchange);
            Assert.Equal("145.50", result.Response.LastPrice);
            Assert.Equal("144.00", result.Response.PriorClose);
            Assert.Equal("1.50", result.Response.PriceChange);
            Assert.Equal("1.04", result.Response.PriceChangePercentage);
            Assert.Equal("150.50", result.Response.High52W);
            Assert.Equal("120.25", result.Response.Low52W);
            Assert.Equal("Dec 24", result.Response.ContractMonth);
            Assert.Equal("THB", result.Response.Currency);
            Assert.NotNull(result.Response.CorporateActions);
            Assert.NotNull(result.Response.TradingSign);
            Assert.Equal(2, result.Response.TradingSign.Count);
            Assert.Equal("L", result.Response.TradingSign.FirstOrDefault());
            Assert.Equal("H", result.Response.TradingSign.LastOrDefault());
            Assert.Equal(2, result.Response.CorporateActions.Count);
            Assert.Equal("XA", result.Response.CorporateActions.FirstOrDefault()?.Type);
            Assert.Equal("2024-06-26", result.Response.CorporateActions.FirstOrDefault()?.Date);
            Assert.Equal("XB", result.Response.CorporateActions.LastOrDefault()?.Type);
            Assert.Equal("2024-06-26", result.Response.CorporateActions.LastOrDefault()?.Date);
        }

        [Fact]
        public void GetResult_ShouldHandleEmptyMarketStreaming_WhenStreamingDataIsNull()
        {
            // Arrange
            var instrument = new Instrument();
            var marketProfileParams = new MarketProfileOverviewParams();
            var setVenueMapping = new SetVenueMapping();
            var corporateAction = new CorporateAction();
            var tradingSign = new TradingSign();
            var marketStreaming = new MarketStreamingResponse();

            // Act
            var result = new MarketProfileOverviewService()
                .WithInstruments(instrument)
                .WithMarketProfileParams(marketProfileParams)
                .WithVenueMapping(setVenueMapping)
                .WithCorporateAction(corporateAction)
                .WithTradingSign(tradingSign)
                .WithMarketStreamingResponse(marketStreaming)
                .WithIsTfex(false).GetResult();

            // Assert
            Assert.NotNull(result);
            Assert.Equal("0", result.Code);
            Assert.Equal(string.Empty, result.Message);
            Assert.NotNull(result.Response);
            Assert.Equal("0", result.Response.LastPrice);
            Assert.Equal("0", result.Response.PriorClose);
            Assert.Equal("0", result.Response.PriceChange);
            Assert.Equal("0", result.Response.PriceChangePercentage);
            Assert.Equal("0.00", result.Response.High52W);
            Assert.Equal("0.00", result.Response.Low52W);
            Assert.Equal(string.Empty, result.Response.ContractMonth);
            Assert.Empty(result.Response.TradingSign);
        }
    }
}
