namespace Pi.TfexService.API.Models.Order;

// For Debug route only
public record MockSetTradeOrderStatus(
    string OrderNo,
    string AccountNo,
    string SeriesId,
    string Side,
    double? Price,
    long? Volume,
    long? BalanceVolume,
    long? MatchedVolume,
    long? CancelledVolume,
    string Status
);