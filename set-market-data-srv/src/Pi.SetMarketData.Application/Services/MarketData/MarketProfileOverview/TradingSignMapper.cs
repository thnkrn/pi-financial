using Pi.SetMarketData.Application.Constants;
using Pi.SetMarketData.Domain.Entities;

namespace Pi.SetMarketData.Application.Services.MarketData.MarketProfileOverview;

public class TradingSignsMapper
{
    public List<string> MapTradingSigns(TradingSign message)
    {
        var tradingSigns = new List<string>();
        var signs = message?.Sign?.Split(',') ?? [];
        if (signs.Length == 0) return [""];

        foreach (var sign in signs)
        {
            switch (sign.ToUpper())
            {
                case InstrumentState.HALT_E:
                    tradingSigns.Add(InstrumentState.H);
                    break;
                case InstrumentState.SUSPEND_E:
                    tradingSigns.Add(InstrumentState.SP);
                    break;
                case InstrumentState.PAUSE_E:
                case InstrumentState.FULLHALT_E:
                case InstrumentState.PRE_OPEN_E:
                case InstrumentState.EXPIRED_E:
                case InstrumentState.PRE_OPEN_CB_E:
                    tradingSigns.Add(string.Empty);
                    break;
                case "":
                case InstrumentState.T:
                case InstrumentState.Z:
                    break;
                default:
                    tradingSigns.Add(sign.ToUpper());
                    break;
            }
        }
        return tradingSigns;
    }
}
