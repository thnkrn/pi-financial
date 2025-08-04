using Pi.SetMarketData.Domain.Models.Response;

namespace Pi.SetMarketData.Application.Services.MarketData.MarketStatus;

public static class MarketStatusService
{
    public static MarketStatusResponse GetResult(
        Domain.Entities.MarketStatus marketStatus
    )
    {
        return new MarketStatusResponse
        {
            Code = "0",
            Message = "",
            Response = new StatusResponse { MarketStatus = marketStatus.Status ?? "" }
        };
    }
}
