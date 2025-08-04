using System.Linq.Expressions;
using Pi.SetMarketData.Application.Constants;
using Pi.SetMarketData.Domain.Entities;
using Pi.SetMarketData.Domain.Models.Response;

namespace Pi.SetMarketData.Application.Services.MarketData.MarketFilterInstruments;
public static class MarketFilterInstrumentsService
{
    public static MarketFilterInstrumentsResponse GetResult
    (
        List<CuratedFilter> curatedFilters,
        List<Instrument> instruments,
        List<PriceResponse?> priceResponses,
        List<string> logosList
    )
    {
        var _instrumentCategoryList = new List<InstrumentCategoryList>();
        var groupedInstruments = instruments
            .GroupBy(instrument => new { instrument.InstrumentType, instrument.InstrumentCategory, instrument.InstrumentId })
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
                        FriendlyName = string.IsNullOrEmpty(instruments[j].FriendlyName) ?
                            instruments[j].LongName :
                            instruments[j].FriendlyName,
                        Logo = j < logosList.Count ? logosList[j] : "",
                        Price = priceResponse?.Price ?? "0.00",
                        PriceChange = priceResponse?.PriceChanged ?? "0.00",
                        PriceChangeRatio = priceResponse?.PriceChangedRate ?? "0.00",
                        IsFavorite = false,
                        Unit = instruments[j].Currency ?? "",
                        LatestNavTimestamp = 0, // TBD
                        IsMainSession = false, // TBD
                        TotalValue = priceResponse?.TotalAmount ?? "0.00",
                        TotalVolume = priceResponse?.TotalVolume ?? "0"
                    });
                }
            }

            _instrumentCategoryList.Add(new InstrumentCategoryList
            {
                Order = curatedFilters.Find(x => x.FilterId == instrument.Key.InstrumentId)?.Ordering ?? 0,
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

    public static Expression<Func<Instrument, bool>> GetTypeCategory(CuratedFilter curatedFilter)
    {
        return (curatedFilter.GroupName, curatedFilter.SubGroupName, curatedFilter.FilterName) switch
        {
            (InstrumentConstants.ThaiEquities, InstrumentConstants.Stocks, "All") => target =>
                target.InstrumentType == InstrumentConstants.VenueOfSetStock &&
                target.InstrumentCategory == InstrumentConstants.ThaiStocks,
            (InstrumentConstants.ThaiEquities, InstrumentConstants.DerivativeWarrants, "All") => target =>
                target.InstrumentType == InstrumentConstants.VenueOfSetStock &&
                (
                    target.InstrumentCategory == InstrumentConstants.ThaiDerivativeWarrants ||
                    target.InstrumentCategory == InstrumentConstants.ForeignDerivativeWarrants
                ),
            (InstrumentConstants.ThaiEquities, InstrumentConstants.DerivativeWarrants, "SET50 Index") => target =>
                target.InstrumentType == InstrumentConstants.VenueOfSetStock &&
                target.InstrumentCategory == InstrumentConstants.ThaiDerivativeWarrants &&
                (target.Symbol ?? "").StartsWith("SET50"),
            (InstrumentConstants.ThaiEquities, InstrumentConstants.DerivativeWarrants, "Thai Stock") => target =>
                target.InstrumentType == InstrumentConstants.VenueOfSetStock &&
                target.InstrumentCategory == InstrumentConstants.ThaiDerivativeWarrants &&
                !(target.Symbol ?? "").StartsWith("SET50"),
            (InstrumentConstants.ThaiEquities, InstrumentConstants.DerivativeWarrants, "Foreign") => target =>
                target.InstrumentType == InstrumentConstants.VenueOfSetStock &&
                target.InstrumentCategory == InstrumentConstants.ForeignDerivativeWarrants,
            (InstrumentConstants.ThaiEquities, InstrumentConstants.DepositaryReceipts, InstrumentConstants.DepositaryReceipts) => target =>
                target.InstrumentType == InstrumentConstants.VenueOfSetStock &&
                target.InstrumentCategory == InstrumentConstants.DRs,
            (InstrumentConstants.ThaiEquities, InstrumentConstants.ETFs, InstrumentConstants.ETFs) => target =>
                target.InstrumentType == InstrumentConstants.VenueOfSetStock &&
                target.InstrumentCategory == InstrumentConstants.ThaiETFs,
            (InstrumentConstants.ThaiEquities, InstrumentConstants.Warrants, InstrumentConstants.Warrants) => target =>
                target.InstrumentType == InstrumentConstants.VenueOfSetStock &&
                target.InstrumentCategory == InstrumentConstants.ThaiStockWarrants,
            (InstrumentConstants.Derivatives, "S50 Futures", "SET50 Futures") => target =>
                target.InstrumentType == InstrumentConstants.VenueOfTfexStock &&
                target.InstrumentCategory == InstrumentConstants.SET50IndexFutures,
            (InstrumentConstants.Derivatives, "S50 Options", "SET50 Options") => target =>
                target.InstrumentType == InstrumentConstants.VenueOfTfexStock &&
                target.InstrumentCategory == InstrumentConstants.SET50IndexOptions,
            (InstrumentConstants.Derivatives, "Sectors", InstrumentConstants.SectorIndexFutures) => target =>
                target.InstrumentType == InstrumentConstants.VenueOfTfexStock &&
                target.InstrumentCategory == InstrumentConstants.SectorIndexFutures,
            (InstrumentConstants.Derivatives, "Stocks", InstrumentConstants.SingleStockFutures) => target =>
                target.InstrumentType == InstrumentConstants.VenueOfTfexStock &&
                target.InstrumentCategory == InstrumentConstants.SingleStockFutures,
            (InstrumentConstants.Derivatives, "Metals", InstrumentConstants.PreciousMetalFutures) => target =>
                target.InstrumentType == InstrumentConstants.VenueOfTfexStock &&
                target.InstrumentCategory == InstrumentConstants.PreciousMetalFutures,
            (InstrumentConstants.Derivatives, "Currency", InstrumentConstants.CurrencyFutures) => target =>
                target.InstrumentType == InstrumentConstants.VenueOfTfexStock &&
                target.InstrumentCategory == InstrumentConstants.CurrencyFutures,
            _ => target => false,
        };
    }
}