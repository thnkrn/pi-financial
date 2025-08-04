using Pi.SetMarketData.Domain.Entities;
using Pi.SetMarketData.Domain.Models.Response.MarketDataManagement;

namespace Pi.SetMarketData.Application.Services.MarketDataManagement;

public static class CuratedMemberService
{
    public static List<CuratedMemberResponse> GetResult(
        List<Instrument> instruments,
        List<string> logos
    )
    {
        return instruments.Select((target, index) => new CuratedMemberResponse
        {
            Symbol = target.Symbol,
            FriendlyName = target.FriendlyName,
            Logo = index < logos.Count ? logos[index] : string.Empty,
            Figi = string.Empty,
            Units = target.Currency,
            Exchange = target.Exchange,
            DataVendorCode = string.Empty,
            DataVendorCode2 = string.Empty,
        }).ToList();
    }
}