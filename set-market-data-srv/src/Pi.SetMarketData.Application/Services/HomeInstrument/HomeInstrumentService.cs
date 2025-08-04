using Pi.SetMarketData.Domain.Entities;
using Pi.SetMarketData.Domain.Models.Response;

namespace Pi.SetMarketData.Application.Services.MarketData.HomeInstrument
{
    public static class HomeInstrumentService
    {
        public static HomeInstrumentsResponse GetResult(
            List<Instrument> instruments,
            List<PriceResponse?> priceResponses,
            List<string> logos
        )
        {
            List<InstrumentList> _instrumentLists = [];

            for (int i = 0; i < instruments.Count; i++)
            {
                var instrument = instruments[i];
                var priceResponse = priceResponses[i];
                var _instrumentList = new InstrumentList
                {
                    Order = i + 1,
                    InstrumentType = instrument.InstrumentType ?? "",
                    InstrumentCategory = instrument.InstrumentCategory ?? "",
                    Venue = instrument.Venue ?? "",
                    Symbol = instrument.Symbol ?? "",
                    FriendlyName = string.IsNullOrEmpty(instrument.FriendlyName) ?
                        instrument.LongName :
                        instrument.FriendlyName,
                    Logo = i < logos.Count ? logos[i] : "",
                    Unit = instrument.Currency ?? "",
                    Price = priceResponse?.Price?.ToString() ?? "0.00",
                    PriceChange = priceResponse?.PriceChanged?.ToString() ?? "0.00",
                    PriceChangeRatio = priceResponse?.PriceChangedRate?.ToString() ?? "0.00",
                    TotalValue = priceResponse?.TotalAmount?.ToString() ?? "0.00",
                    TotalVolume = priceResponse?.TotalVolume?.ToString() ?? "0.00",
                };

                _instrumentLists.Add(_instrumentList);
            }

            return new HomeInstrumentsResponse
            {
                Code = "0",
                Message = "",
                Response = new InstrumentsResponse
                {
                    InstrumentList = _instrumentLists
                },
            };
        }

        public static (CuratedFilter, bool) GetCuratedFilter(string? name, string? relevantTo)
        {
            return (name?.ToLower(), relevantTo?.ToLower()) switch
            {
                ("home set", "homepage") => (new CuratedFilter { FilterName = "SET50" }, true),
                ("home mai", "homepage") => (new CuratedFilter { FilterName = "MAI" }, true),
                _ => (new CuratedFilter(), false) ,
            };
        }
    }
}
