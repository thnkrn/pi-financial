using System.Globalization;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pi.SetService.Application.Models;
using Pi.SetService.Application.Models.Notification;
using Pi.SetService.Application.Options;
using Pi.SetService.Application.Services.FeatureService;
using Pi.SetService.Application.Services.NotificationService;
using Pi.SetService.Application.Services.UserService;
using Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;
using Pi.SetService.Domain.AggregatesModel.TradingAggregate;

namespace Pi.SetService.Application.Commands;

public abstract record NotifyEvent
{
    public required string CustCode { get; init; }
    public required string Symbol { get; init; }
    public required OrderAction OrderAction { get; init; }
}

public record NotifyOrderMatched : NotifyEvent
{
    public required decimal Volume { get; init; }
    public required decimal VolumeMatched { get; init; }
    public required decimal AvgPrice { get; init; }
}

public record NotifyOrderCancelledPartially : NotifyEvent
{
    public required decimal OrderVolume { get; init; }
    public required decimal CancelledVolume { get; init; }
    public required decimal VolumeMatched { get; init; }
    public required decimal AvgPrice { get; init; }
}

public record NotifyOrderRejected : NotifyEvent
{
    public required decimal Volume { get; init; }
    public string? Reason { get; init; }
    public Source? Source { get; init; }
}

public class NotificationConsumer(
    INotificationService notificationService,
    IUserService userService,
    ICustomFeatureService customFeatureService,
    IOptions<NotificationIconOptions> options,
    ILogger<NotificationConsumer> logger) :
    IConsumer<NotifyOrderMatched>,
    IConsumer<NotifyOrderCancelledPartially>,
    IConsumer<NotifyOrderRejected>
{
    private readonly string _iconBaseUrl = options.Value.S3IconBaseUrl;
    private const string NumberFormat = "N";

    public async Task Consume(ConsumeContext<NotifyOrderMatched> context)
    {
        var template = context.Message.OrderAction switch
        {
            OrderAction.Cover => NotificationTemplate.CoverOrderMatched,
            OrderAction.Short => NotificationTemplate.ShortOrderMatched,
            OrderAction.Buy => NotificationTemplate.BuyOrderMatched,
            OrderAction.Sell => NotificationTemplate.SellOrderMatched,
            _ => (NotificationTemplate?)null
        };

        if (template == null)
        {
            return;
        }

        string[] body = [
            context.Message.Symbol,
            context.Message.VolumeMatched.ToString(NumberFormat, CultureInfo.InvariantCulture),
            context.Message.AvgPrice.ToString(NumberFormat, CultureInfo.InvariantCulture),
            "THB"
        ];

        await SendNotification(context, (NotificationTemplate)template, body);
    }

    public async Task Consume(ConsumeContext<NotifyOrderCancelledPartially> context)
    {
        var template = context.Message.OrderAction switch
        {
            OrderAction.Cover => NotificationTemplate.CoverOrderCanceledPartiallyMatched,
            OrderAction.Short => NotificationTemplate.ShortOrderCanceledPartiallyMatched,
            OrderAction.Buy => NotificationTemplate.BuyOrderCanceledPartiallyMatched,
            OrderAction.Sell => NotificationTemplate.SellOrderCanceledPartiallyMatched,
            _ => (NotificationTemplate?)null
        };

        if (template == null)
        {
            return;
        }

        string[] body = [
            context.Message.Symbol,
            context.Message.VolumeMatched.ToString(NumberFormat, CultureInfo.InvariantCulture),
            context.Message.OrderVolume.ToString(NumberFormat, CultureInfo.InvariantCulture),
            context.Message.AvgPrice.ToString(NumberFormat, CultureInfo.InvariantCulture),
            "THB",
            (context.Message.OrderVolume - context.Message.VolumeMatched).ToString(NumberFormat, CultureInfo.InvariantCulture)
        ];

        await SendNotification(context, (NotificationTemplate)template, body);
    }

    public async Task Consume(ConsumeContext<NotifyOrderRejected> context)
    {
        var template = context.Message.OrderAction switch
        {
            OrderAction.Cover => NotificationTemplate.CoverOrderRejected,
            OrderAction.Short => NotificationTemplate.ShortOrderRejected,
            OrderAction.Buy => NotificationTemplate.BuyOrderRejected,
            OrderAction.Sell => NotificationTemplate.SellOrderRejected,
            _ => (NotificationTemplate?)null
        };

        if (template == null)
        {
            return;
        }

        string[] body = [
            context.Message.Symbol,
            context.Message.Volume.ToString(NumberFormat, CultureInfo.InvariantCulture),
        ];

        await SendNotification(context, (NotificationTemplate)template, body);
    }

    private async Task SendNotification<T>(ConsumeContext<T> context, NotificationTemplate template, string[] body) where T : NotifyEvent
    {
        var userId = await userService.GetUserIdByCustCode(context.Message.CustCode, context.CancellationToken);
        if (userId == null) return;

        customFeatureService.UpsertUserIdAttribute(userId.Value);
        if (customFeatureService.IsOff(Features.OrderNotification))
        {
            return;
        }

        try
        {
            await notificationService.SendNotification(new NotificationPayload
            {
                Template = template,
                UserId = userId.Value,
                Type = NotificationType.Order,
                IsPush = true,
                StoreDb = true,
                Body = body,
                Tags = [
                    NewOrderSideTag(template),
                    NewOrderSymbolTag(context.Message.Symbol)
                ]
            });
        }
        catch (ArgumentOutOfRangeException e)
        {
            logger.LogError(e, "Can't send order notification because of ArgumentOutOfRangeException");
        }
    }

    private NotificationTag NewOrderSymbolTag(string symbol)
    {
        return new NotificationTag()
        {
            Icon = $"{_iconBaseUrl}{symbol}.svg",
            Payload = symbol,
            TextColor = "#000000",
            Type = NotificationTagType.InstrumentBadge
        };
    }

    private static NotificationTag NewOrderSideTag(NotificationTemplate notificationTemplate)
    {
        CmsAppContentId tagPayload;
        string backgroundColor, textColor;
        switch (notificationTemplate)
        {
            case NotificationTemplate.SellOrderMatched:
            case NotificationTemplate.SellOrderRejected:
            case NotificationTemplate.SellOrderCanceledPartiallyMatched:
                tagPayload = CmsAppContentId.SellOrder;
                backgroundColor = "#FBECEB";
                textColor = "#D83A2C";
                break;
            case NotificationTemplate.BuyOrderMatched:
            case NotificationTemplate.BuyOrderRejected:
            case NotificationTemplate.BuyOrderCanceledPartiallyMatched:
                tagPayload = CmsAppContentId.BuyOrder;
                backgroundColor = "#DEF8F0";
                textColor = "#158463";
                break;
            case NotificationTemplate.ShortOrderMatched:
            case NotificationTemplate.ShortOrderRejected:
            case NotificationTemplate.ShortOrderCanceledPartiallyMatched:
                tagPayload = CmsAppContentId.ShortOrder;
                backgroundColor = "#FBECEB";
                textColor = "#B42318";
                break;
            case NotificationTemplate.CoverOrderMatched:
            case NotificationTemplate.CoverOrderRejected:
            case NotificationTemplate.CoverOrderCanceledPartiallyMatched:
                tagPayload = CmsAppContentId.CoverOrder;
                backgroundColor = "#ECDFEC";
                textColor = "#473E72";
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(notificationTemplate), notificationTemplate, null);
        }

        return new NotificationTag()
        {
            Payload = ((int)tagPayload).ToString(),
            BackgroundColor = backgroundColor,
            TextColor = textColor,
            Type = NotificationTagType.LocalizedTextBadge
        };
    }
}
