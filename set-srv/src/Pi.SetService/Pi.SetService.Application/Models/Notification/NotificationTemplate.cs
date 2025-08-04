namespace Pi.SetService.Application.Models.Notification;

public enum NotificationTemplate
{
    BuyOrderMatched = 13,
    SellOrderMatched = 14,
    ShortOrderMatched = 15,
    CoverOrderMatched = 16,
    BuyOrderRejected = 17,
    SellOrderRejected = 18,
    ShortOrderRejected = 19,
    CoverOrderRejected = 20,
    BuyOrderCanceledPartiallyMatched = 34,
    SellOrderCanceledPartiallyMatched = 35,
    ShortOrderCanceledPartiallyMatched = 36,
    CoverOrderCanceledPartiallyMatched = 37,
}
