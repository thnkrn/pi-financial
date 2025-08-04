using Pi.GlobalMarketData.Application.Utils;
using Pi.GlobalMarketData.Domain.Entities;
using Pi.GlobalMarketData.Domain.Models.Response;

namespace Pi.GlobalMarketData.Application.Services.MarketData.MarketProfileOverview;

public class MarketProfileOverviewParams
{
    public double High52W { get; set; } = 0;
    public double Low52W { get; set; } = 0;
}

public static class MarketProfileOverviewService
{
    public static MarketProfileOverviewResponse GetResult(
        GeInstrument instruments,
        string minimumOrderSize,
        GeVenueMapping geVenueMapping,
        ExchangeTimezone exchangeTimezone,
        MarketProfileOverviewParams marketProfileOverviewParams,
        StreamingBody streamingBody
    )
    {
        var exchangeTime = DataManipulation.GetDateFromTimezone(exchangeTimezone.Timezone);

        return new MarketProfileOverviewResponse
        {
            Code = "0",
            Message = string.Empty,
            Response = new ProfileOverviewResponse
            {
                Market = DataManipulation.RemoveSpace(
                    instruments.InstrumentCategory ?? string.Empty
                ),
                Exchange = geVenueMapping.Exchange ?? string.Empty,
                ExchangeTime = exchangeTime.ToString("HH:mm"),
                LastPrice = streamingBody.Price ?? "0",
                PriorClose = streamingBody.PreClose ?? "0",
                PriceChange = streamingBody.PriceChanged ?? "0",
                PriceChangePercentage = streamingBody.PriceChangedRate ?? "0",
                MinimumOrderSize = minimumOrderSize ?? "0",
                High52W = DataManipulation.FormatDecimals(
                    marketProfileOverviewParams.High52W.ToString(),
                    2
                ),
                Low52W = DataManipulation.FormatDecimals(
                    marketProfileOverviewParams.Low52W.ToString(),
                    2
                ),
                ContractMonth = string.Empty, // Not available in GE
                Currency = instruments.Currency ?? string.Empty,
                CorporateActions = new List<CorporateActionResponse>()
                {
                    new CorporateActionResponse { Type = "", Date = "" }
                }, // Not available in GE
                TradingSign = [], // Not available in GE
            }
        };
    }
}
