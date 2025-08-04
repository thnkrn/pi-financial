namespace Pi.TfexService.Application.Models;

public record SetTradeOrderStatus(
    string OrderNo,
    string AccountNo,
    string SeriesId,
    SetTradeListenerOrderEnum.Side Side,
    double Price,
    long Volume,
    long BalanceVolume,
    long MatchedVolume,
    long CancelledVolume,
    string Status
);
