using Pi.GlobalMarketData.Domain.Entities;
using Pi.GlobalMarketData.Domain.Models.Response;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pi.GlobalMarketData.Domain.Models.Response.Velexa;

namespace Pi.GlobalMarketData.Application.Services.MarketData.MarketInstrumentInfo;

public static class MarketInstrumentInfoService
{
    public static MarketInstrumentInfoResponse GetResult(
        VelexaInstrument instrumentData,
        VelexaInstrumentSpecification instrumentSpecData
    )
    {
        var response = new InstrumentInfoResponse()
        {
            MinimalPriceIncrement = instrumentData.MinPriceIncrement,
            MinimalQuantityIncrement = instrumentSpecData.LotSize,
            Currency = instrumentData.Currency
        };

        return new MarketInstrumentInfoResponse
        {
            Code = "0",
            Message = "",
            Response = response
        };
    }
}
