using System.Globalization;
using Pi.GlobalEquities.DomainModels;
using Pi.GlobalEquities.Models;

namespace Pi.GlobalEquities.Worker.ExternalServices.Notification;

public class NotificationPayload
{
    public NotificationTemplate TemplateNo { get; init; }
    public IOrder Order { get; init; }
    public IEnumerable<string> Payload
    {
        get
        {
            switch (TemplateNo)
            {
                case NotificationTemplate.SellMatched or NotificationTemplate.BuyMatched:
                    ValidateOrder();
                    return new List<string>
                    {
                        Order.AverageFillPrice!.Value.ToString("n2"),
                        Order.Quantity.ToString(CultureInfo.InvariantCulture),
                        Order.SymbolId,
                        Order.Currency.ToString()
                    };

                case NotificationTemplate.SellCancelled or NotificationTemplate.BuyCancelled
                    or NotificationTemplate.SellRejected or NotificationTemplate.BuyRejected:
                    return new List<string>
                    {
                        Order.SymbolId,
                        Order.Quantity.ToString(CultureInfo.InvariantCulture)
                    };

                case NotificationTemplate.SellOrderPartiallyMatched
                    or NotificationTemplate.BuyOrderPartiallyMatched:
                    ValidateOrder();
                    return new List<string>
                    {
                        Order.SymbolId,
                        Order.QuantityCancelled.ToString(CultureInfo.InvariantCulture),
                        Order.Quantity.ToString(CultureInfo.InvariantCulture),
                        Order.QuantityFilled.ToString(CultureInfo.InvariantCulture),
                        Order.Currency.ToString(),
                        Order.AverageFillPrice!.Value.ToString("n2")
                    };

                default:
                    throw new InvalidOperationException($"Unsupported template: {TemplateNo}");
            }
        }
    }

    private void ValidateOrder()
    {
        if (Order.AverageFillPrice == null)
            throw new ArgumentNullException(nameof(Order.AverageFillPrice));

        if (Order.Quantity == default)
            throw new ArgumentNullException(nameof(Order.Quantity));

        if (Order.Currency == default)
            throw new ArgumentNullException(nameof(Order.Currency));
    }
}
