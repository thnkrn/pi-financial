using Pi.FundMarketData.DomainModels;

namespace Pi.FundMarketData.API.Models.Responses;

public class FundHistoricalNavResponse
{
    public decimal? NavChange { get; init; }
    public double? NavChangePercentage { get; init; }
    public IEnumerable<HistoricalNavResponse> NavList { get; init; } = Enumerable.Empty<HistoricalNavResponse>();
    public class HistoricalNavResponse
    {
        public long Timestamp { get; init; }
        public decimal Nav { get; init; }
    }

    public FundHistoricalNavResponse(HistoricalNavInfo model = null)
    {
        if (model is null)
            return;

        NavChange = model.NavChange;
        NavChangePercentage = model.NavChangePercentage;
        NavList = model.NavList.Any()
            ? model.NavList.Select(x => new HistoricalNavResponse
            {
                Timestamp = ((DateTimeOffset)x.Date).ToUnixTimeSeconds(),
                Nav = x.Nav
            })
            : Enumerable.Empty<HistoricalNavResponse>();
    }
}

public class FundWebHistoricalNavResponse : FundHistoricalNavResponse
{
    public string Symbol { get; init; }

    public FundWebHistoricalNavResponse(HistoricalNavInfo model = null) : base(model)
    {
        if (model is null) return;
        Symbol = model.Symbol;
    }
}
