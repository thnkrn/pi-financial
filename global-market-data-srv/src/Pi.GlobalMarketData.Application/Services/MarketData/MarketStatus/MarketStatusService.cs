using Pi.GlobalMarketData.Domain.Models.Response;

namespace Pi.GlobalMarketData.Application.Services.MarketData.MarketStatus;

public static class MarketStatusService
{
    public static MarketStatusResponse GetResult(
        Domain.Entities.MarketSessionStatus sessionStatus)
    {
        var response = new StatusResponse();
        if (sessionStatus != null)
        {
            response.MarketStatus = sessionStatus.MarketSession ?? string.Empty;
        }
        else
            response.MarketStatus = string.Empty;

        return new MarketStatusResponse
        {
            Code = "0",
            Message = "",
            Response = response
        };
    }
}
