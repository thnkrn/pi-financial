using System.Linq.Expressions;
using Pi.GlobalMarketData.Domain.Constants;
using Pi.GlobalMarketData.Domain.Entities;
using Pi.GlobalMarketData.Domain.Models.Response;

namespace Pi.GlobalMarketData.Application.Services.MarketData.MarketFilterInstruments;

public static class MarketFilterInstrumentsService
{
    public static MarketFilterInstrumentsResponse GetResult
    (
        List<CuratedFilter> curatedFilters,
        List<GeInstrument> instruments,
        List<PriceResponse?> priceResponses,
        List<string> logos
    )
    {
        var _instrumentCategoryList = new List<InstrumentCategoryList>();
        var groupedInstruments = instruments
            .GroupBy(instrument => new { instrument.InstrumentType, instrument.InstrumentCategory, instrument.GeInstrumentId })
            .ToList();

        for (int i = 0; i < groupedInstruments.Count; i++)
        {
            var instrument = groupedInstruments[i];
            var _instrumentList = new List<FilterInstrumentList>();
            for (int j = 0; j < instruments.Count; j++)
            {
                if (instruments[j].InstrumentType == instrument.Key.InstrumentType && instruments[j].InstrumentCategory == instrument.Key.InstrumentCategory)
                {
                    var priceResponse = priceResponses[j];
                    _instrumentList.Add(new FilterInstrumentList
                    {
                        Venue = instruments[j].Venue ?? "",
                        Symbol = instruments[j].Symbol ?? "",
                        FriendlyName = instruments[j].Name,
                        Logo = j < logos.Count ? logos[j] : "",
                        Price = string.IsNullOrEmpty(priceResponse?.Price) ?
                            "0.00" :
                            priceResponse.Price,
                        PriceChange = string.IsNullOrEmpty(priceResponse?.PriceChanged) ?
                            "0.00" :
                            priceResponse.PriceChanged,
                        PriceChangeRatio = string.IsNullOrEmpty(priceResponse?.PriceChangedRate) ?
                            "0.00" :
                            priceResponse.PriceChangedRate,
                        IsFavorite = false,
                        Unit = instruments[j].Currency ?? string.Empty,
                        LatestNavTimestamp = 0, // TBD
                        IsMainSession = false, // TBD   
                        TotalValue = string.IsNullOrEmpty(priceResponse?.TotalAmount) ?
                            "0.00" :
                            priceResponse.TotalAmount,
                        TotalVolume = string.IsNullOrEmpty(priceResponse?.TotalVolume) ?
                            "0" :
                            priceResponse.TotalVolume
                    });
                }
            }

            _instrumentCategoryList.Add(new InstrumentCategoryList
            {
                Order = curatedFilters.Find(x => x.FilterId == instrument.Key.GeInstrumentId)?.Ordering ?? 0,
                InstrumentType = instrument.Key.InstrumentType,
                InstrumentCategory = instrument.Key.InstrumentCategory?.Replace(" ", ""),
                InstrumentList = _instrumentList
            });
        }

        return new MarketFilterInstrumentsResponse
        {
            Code = "0",
            Message = "",
            Response = new FilterInstrumentsResponse
            {
                InstrumentCategoryList = _instrumentCategoryList
            },
        };
    }

    public static Expression<Func<GeInstrument, bool>> GetTypeCategory(CuratedFilter curatedFilter)
    {
        return (curatedFilter.GroupName, curatedFilter.SubGroupName, curatedFilter.FilterName) switch
        {
            (InstrumentConstants.GlobalEquities, "Stocks", "All") => target => 
                target.InstrumentType == InstrumentConstants.GlobalEquity &&
                target.InstrumentCategory == InstrumentConstants.GlobalStocks,
            (InstrumentConstants.GlobalEquities, "Stocks", InstrumentConstants.NASDAQ) => target => 
                target.InstrumentType == InstrumentConstants.GlobalEquity &&
                target.InstrumentCategory == InstrumentConstants.GlobalStocks &&
                target.Venue == InstrumentConstants.NASDAQ,
            (InstrumentConstants.GlobalEquities, "Stocks", "Hong Kong") => target => 
                target.InstrumentType == InstrumentConstants.GlobalEquity &&
                target.InstrumentCategory == InstrumentConstants.GlobalStocks &&
                target.Venue == InstrumentConstants.HKEX,
            (InstrumentConstants.GlobalEquities, InstrumentConstants.ETFs, "All") => target => 
                target.InstrumentType == InstrumentConstants.GlobalEquity &&
                target.InstrumentCategory == InstrumentConstants.GlobalETFs,
            (InstrumentConstants.GlobalEquities, InstrumentConstants.ETFs, InstrumentConstants.NASDAQ) => target => 
                target.InstrumentType == InstrumentConstants.GlobalEquity &&
                target.InstrumentCategory == InstrumentConstants.GlobalETFs &&
                target.Venue == InstrumentConstants.NASDAQ,
            (InstrumentConstants.GlobalEquities, InstrumentConstants.ETFs, "ARCA") => target => 
                target.InstrumentType == InstrumentConstants.GlobalEquity &&
                target.InstrumentCategory == InstrumentConstants.GlobalETFs &&
                target.Venue == "ARCA",
            (InstrumentConstants.GlobalEquities, InstrumentConstants.ETFs, "BATS") => target => 
                target.InstrumentType == InstrumentConstants.GlobalEquity &&
                target.InstrumentCategory == InstrumentConstants.GlobalETFs &&
                target.Venue == "BATS",
            (InstrumentConstants.GlobalEquities, InstrumentConstants.NASDAQ, InstrumentConstants.NASDAQ) => target => 
                target.InstrumentType == InstrumentConstants.GlobalEquity &&
                target.InstrumentCategory == InstrumentConstants.GlobalStocks &&
                target.Venue == "NASDAQ",
            (InstrumentConstants.GlobalEquities, "Hong Kong", "Hong Kong") => target => 
                target.InstrumentType == InstrumentConstants.GlobalEquity &&
                target.InstrumentCategory == InstrumentConstants.GlobalStocks &&
                target.Venue == InstrumentConstants.HKEX,
            _ => target => false,
        };
    }
}