using Pi.SetMarketData.Application.Utils;
using Pi.SetMarketData.Domain.Entities;
using Pi.SetMarketData.Domain.Models.Response;

namespace Pi.SetMarketData.Application.Services.MarketData.MarketDerivativeInformation
{
    public static class MarketDerivativeInformationService
    {
        public static MarketDerivativeInformationResponse GetResult(
            Instrument? instruments,
            InstrumentDetail instrumentDetail,
            InitialMargin initialMarginData,
            double initialMarginMultiplier
        )
        {
            var instrument = instruments ?? new Instrument();

            string initialMargin = "";

            if (
                (initialMarginData.Im != null || initialMarginData.Im != "0")
                && initialMarginMultiplier > 0
            )
            {
                double initialMarginDouble = double.TryParse(initialMarginData.Im, out var im)
                    ? im
                    : 0;

                initialMargin = (initialMarginDouble * initialMarginMultiplier).ToString();
            }
            var @decimal = instrumentDetail.DecimalsInPrice != 0 ? instrumentDetail.DecimalsInPrice : instrumentDetail.Decimals;

            var data = new DerivativeInformationResponse
            {
                SecurityType = instrument.SecurityType ?? "",
                TradingUnit = instrument.TradingUnit.FormatDecimals(@decimal),
                MinBidUnit = instrument.MinBidUnit ?? "0",
                Multiplier = instrument.Multiplier ?? "0",
                InitialMargin = initialMargin,
            };

            return new MarketDerivativeInformationResponse
            {
                Code = "0",
                Message = "",
                Response = data,
            };
        }
    }
}
