namespace Pi.TfexService.Application.Models;

public record SetTradePlaceOrderRequest(
    string UserId,
    string CustomerCode,
    string AccountCode,
    SetTradePlaceOrderRequest.PlaceOrderInfo OrderInfo)
{
    public record PlaceOrderInfo(
        string Series,
        Side Side,
        Position Position,
        PriceType PriceType,
        decimal Price,
        int Volume,
        int IcebergVol = 0,
        Validity? ValidityType = null,
        string? ValidityDateCondition = null,
        TriggerCondition? StopCondition = null,
        string? StopSymbol = null,
        decimal? StopPrice = null,
        TriggerSession? TriggerSession = null,
        bool? BypassWarning = true);
}

public record SetTradePlaceOrderResponse(long OrderNo);