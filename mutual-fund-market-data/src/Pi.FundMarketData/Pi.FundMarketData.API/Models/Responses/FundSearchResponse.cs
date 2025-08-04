using Pi.FundMarketData.Constants;
using Pi.FundMarketData.DomainModels;
using Pi.FundMarketData.Models;
using Pi.FundMarketData.Repositories;

namespace Pi.FundMarketData.API.Models.Responses;

public class FundSearchResponse
{
    public string Symbol { get; init; }
    public string Currency { get; init; }
    public string Name { get; init; }
    public decimal? NavChange { get; init; }
    public decimal Nav { get; init; }
    public double? NavChangePercentage { get; init; }
    public string AmcLogo { get; init; }

    public FundSearchResponse(FundSearchData fund, string logo)
    {
        Symbol = fund.Symbol;
        Name = fund.Name;
        Nav = fund.Nav ?? 0;
        NavChange = fund.ValueChange;
        NavChangePercentage = fund.NavChangePercentage;
        Currency = fund.Currency;
        AmcLogo = logo;
    }
}
