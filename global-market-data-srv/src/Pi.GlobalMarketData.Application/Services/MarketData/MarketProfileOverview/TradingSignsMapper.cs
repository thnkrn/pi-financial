using Pi.GlobalMarketData.Application.Constants;
using Pi.GlobalMarketData.Domain.Entities;

namespace Pi.GlobalMarketData.Application.Services.MarketData.MarketProfileOverview
{
    public class TradingSignsMapper
    {
        public List<TradingSign> MapTradingSigns(List<TradingSign> message)
        {
            foreach (var item in message)
            {
                switch (item.Sign)
                {
                    case InstrumentState.HALT_E:
                        item.Sign = InstrumentState.H;
                        break;
                    case InstrumentState.SUSPEND_E:
                        item.Sign = InstrumentState.SP;
                        break;
                    case InstrumentState.PAUSE_E:
                    case InstrumentState.FULLHALT_E:
                    case InstrumentState.PRE_OPEN_E:
                    case InstrumentState.EXPIRED_E:
                    case InstrumentState.PRE_OPEN_CB_E:
                        item.Sign = string.Empty;
                        break;
                    case "":
                        item.Sign = InstrumentState.TBD;
                        break;
                    default:
                        item.Sign = InstrumentState.FUTURE_USED;
                        break;
                }
            }
            return message;
        }


    }

}