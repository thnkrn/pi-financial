namespace Pi.GlobalEquities.Worker.ExternalServices.Notification;

public enum NotificationTemplate
{
    MarketBuyPlaced = 60,
    LimitBuyPlaced = 61,
    StopLimitBuyPlaced = 62,
    BuyCancelled = 63,
    BuyMatched = 64,
    BuyRejected = 65,
    BuyModified = 66,
    BuyOrderPartiallyMatched = 67,
    MarketSellPlaced = 68,
    LimitSellPlaced = 69,
    StopLimitSellPlaced = 70,
    SellCancelled = 71,
    SellMatched = 72,
    SellRejected = 73,
    SellModified = 74,
    SellOrderPartiallyMatched = 75,
}
