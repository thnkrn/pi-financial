namespace Pi.TfexService.Application.Models;

public record SendNotificationRequest(
    string UserId,
    string CustomerCode,
    SetTradeOrderStatus Order);

public record SendNotificationSuccess(string OrderNo, string OrderStatus);