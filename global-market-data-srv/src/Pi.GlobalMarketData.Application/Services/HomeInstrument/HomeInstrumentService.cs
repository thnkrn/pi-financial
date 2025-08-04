using System.Linq.Expressions;
using Pi.GlobalMarketData.Domain.ConstantConfigurations;
using Pi.GlobalMarketData.Domain.Constants;
using Pi.GlobalMarketData.Domain.Entities;
using Pi.GlobalMarketData.Domain.Models.Response;

namespace Pi.GlobalMarketData.Application.Services.HomeInstrument
{
    public static class HomeInstrumentService
    {
        public static HomeInstrumentsResponse GetResult(
            List<GeInstrument> instruments,
            IDictionary<string, PriceResponse?> priceResponseDict,
            List<string> logos
        )
        {
            List<InstrumentList> _instrumentLists = [];

            for (int i = 0; i < instruments.Count; i++)
            {
                var instrument = instruments[i];
                var _instrumentList = new InstrumentList();
                if (priceResponseDict.TryGetValue($"{CacheKey.GeStreamingBody}{instrument.Symbol}", out var marketStreaming))
                {
                    _instrumentList = new InstrumentList
                    {
                        Order = i + 1,
                        InstrumentType = instrument.InstrumentType ?? "",
                        InstrumentCategory = instrument.InstrumentCategory?.Replace(" ", "") ?? "",
                        Venue = instrument.Venue ?? "",
                        Symbol = instrument.Symbol ?? "",
                        FriendlyName = instrument.Name,
                        Logo = i < logos.Count ? logos[i] : "",
                        Unit = instrument.Currency ?? "",
                        Price = marketStreaming?.Price ?? "0.00",
                        PriceChange = marketStreaming?.PriceChanged ?? "0.00",
                        PriceChangeRatio = marketStreaming?.PriceChangedRate ?? "0.00",
                        TotalValue = marketStreaming?.TotalAmount ?? "0.00",
                        TotalVolume = marketStreaming?.TotalVolume ?? "0.00",
                    };
                }
                else
                {
                    _instrumentList = new InstrumentList
                    {
                        Order = i + 1,
                        InstrumentType = instrument.InstrumentType ?? "",
                        InstrumentCategory = instrument.InstrumentCategory?.Replace(" ", "") ?? "",
                        Venue = instrument.Venue ?? "",
                        Symbol = instrument.Symbol ?? "",
                        FriendlyName = instrument.Name,
                        Logo = i < logos.Count ? logos[i] : "",
                        Unit = "",
                        Price = "0.00",
                        PriceChange = "0.00",
                        PriceChangeRatio = "0.00",
                        TotalValue = "0.00",
                        TotalVolume = "0.00",
                    };
                }

                _instrumentLists.Add(_instrumentList);
            }

            return new HomeInstrumentsResponse
            {
                Code = "0",
                Message = "",
                Response = new InstrumentsResponse { InstrumentList = _instrumentLists },
            };
        }

        public static Expression<Func<GeInstrument, bool>> GetGeInstrumentFilter(CuratedList curatedList)
        {
            return (curatedList.Name, curatedList.RelevantTo) switch
            {
                ("Home NASDAQ", "Homepage") => target =>
                    target.InstrumentType == InstrumentConstants.GlobalEquity &&
                    target.InstrumentCategory == InstrumentConstants.GlobalStocks &&
                    target.Venue == InstrumentConstants.NASDAQ,
                ("Home HK", "Homepage") => target =>
                    target.InstrumentType == InstrumentConstants.GlobalEquity &&
                    target.InstrumentCategory == InstrumentConstants.GlobalStocks &&
                    target.Venue == InstrumentConstants.HKEX,
                _ => target => false
            };
        }
    }
}
