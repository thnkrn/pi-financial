using Pi.GlobalMarketData.Domain.Entities;
using Pi.GlobalMarketData.Domain.Models.Response;

namespace Pi.GlobalMarketData.Application.Services.MarketData.MarketDerivativeInformation
{
    public static class MarketDerivativeInformationService
    {
        public static MarketDerivativeInformationResponse GetResult(Instrument instruments)
        {
            var _data = new DerivativeInformationResponse
            {
                SecurityType = instruments.SecurityType ?? "",
                TradingUnit = instruments.TradingUnit ?? "0.00",
                MinBidUnit = instruments.MinBidUnit ?? "0",
                Multiplier = instruments.Multiplier ?? "0",
                InitialMargin = instruments.InitialMargin ?? "0",
            };

            return new MarketDerivativeInformationResponse
            {
                Code = "0",
                Message = "",
                Response = _data,
            };
        }
    }
}
