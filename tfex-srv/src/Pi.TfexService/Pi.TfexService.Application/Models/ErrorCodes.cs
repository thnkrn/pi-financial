namespace Pi.TfexService.Application.Models;

public static class ErrorCodes
{
    public const string SetTradeInternalError = "TFEX0000";
    public const string PlaceOrderBothSide = "TFEX0001";
    public const string NotEnoughExcessEquity = "TFEX0002";
    public const string NotEnoughPosition = "TFEX0003";
    public const string PriceOutOfRange = "TFEX0004";
    public const string PriceOutOfRangeFromLastDone = "TFEX0005";
    public const string OutsideTradingHours = "TFEX0006";
    public const string InvalidData = "TFEX0007";
    public const string SeriesNotFound = "TFEX0008";
    public const string UpdateOrderNoValueChanged = "TFEX0009";
    public const string NotEnoughLineAvailable = "TFEX0010";
    public const string TradeNotFound = "TFEX0011";
}