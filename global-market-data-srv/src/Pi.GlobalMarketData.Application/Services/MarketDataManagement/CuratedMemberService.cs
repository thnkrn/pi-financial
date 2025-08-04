using Pi.GlobalMarketData.Domain.Entities;
using Pi.GlobalMarketData.Domain.Models.Response.MarketDataManagement;

namespace Pi.GlobalMarketData.Application.Services.MarketDataManagement;

public static class CuratedMemberService
{
    public static List<CuratedMemberResponse> GetResult(
        List<GeInstrument> geInstruments,
        List<string> logos
    )
    {
        return geInstruments.Select((target, index) => new CuratedMemberResponse
        {
            Symbol = target.Symbol,
            FriendlyName = target.Name,
            Logo = index < logos.Count ? logos[index] : "",
            Figi = target.Figi,
            Units = target.Currency,
            Exchange = target.Exchange,
            DataVendorCode = ""
        }).ToList();
    }
}