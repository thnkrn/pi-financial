
using System.Reflection.Metadata.Ecma335;
using Pi.SetMarketData.Domain.Entities;
using Pi.SetMarketData.Domain.Models.Response;

namespace Pi.SetMarketData.Application.Services;
public static class MarketGlobalEquityInstrumentInfoService
{
    public static MarketGlobalEquityInstrumentInfoResponse GetResult(
        Instrument instrument
    ){
        return new MarketGlobalEquityInstrumentInfoResponse
        {
            Code = "0",
            Message = string.Empty,
            Response = new GlobalEquityInstrumentInfoResponse
            {
                MinimalPriceIncrement = instrument.MinBidUnit ?? "0.00",
                MinimalQuantityIncrement = instrument.TradingUnit ?? "0",
                Currency = instrument.Currency ?? ""
            }
        };
    }
    
}