using Pi.Financial.Client.FundConnext.Model;
using Pi.Financial.FundService.Domain.AggregatesModel.TradingAggregate;

namespace Pi.Financial.FundService.Infrastructure.Factories;

public static class FundconnextFactory
{
    public static ApiSubscriptionsV2PostRequest.PaymentTypeEnum NewSubscriptionPaymentTypeEnum(PaymentType paymentType)
    {
        return paymentType switch
        {
            PaymentType.AtsSa => ApiSubscriptionsV2PostRequest.PaymentTypeEnum.ATSSA,
            PaymentType.AtsAmc => ApiSubscriptionsV2PostRequest.PaymentTypeEnum.ATSAMC,
            _ => throw new ArgumentOutOfRangeException(nameof(paymentType))
        };
    }

    public static ApiSubscriptionsV2PostRequest.ChannelEnum NewSubscriptionChannelEnum(Channel channel)
    {
        return channel switch
        {
            Channel.MOB => ApiSubscriptionsV2PostRequest.ChannelEnum.MOB,
            _ => throw new ArgumentOutOfRangeException(nameof(channel))
        };
    }

    public static ApiRedemptionsPostRequest.ChannelEnum NewRedemptionChannelEnum(Channel channel)
    {
        return channel switch
        {
            Channel.MOB => ApiRedemptionsPostRequest.ChannelEnum.MOB,
            _ => throw new ArgumentOutOfRangeException(nameof(channel))
        };
    }

    public static ApiSwitchingsPostRequest.ChannelEnum NewSwitchingChannelEnum(Channel channel)
    {
        return channel switch
        {
            Channel.MOB => ApiSwitchingsPostRequest.ChannelEnum.MOB,
            _ => throw new ArgumentOutOfRangeException(nameof(channel))
        };
    }

    public static ApiRedemptionsPostRequest.PaymentTypeEnum NewRedemptionPaymentTypeEnum(PaymentType paymentType)
    {
        return paymentType switch
        {
            PaymentType.AtsSa => ApiRedemptionsPostRequest.PaymentTypeEnum.ATSSA,
            PaymentType.AtsAmc => ApiRedemptionsPostRequest.PaymentTypeEnum.ATSAMC,
            _ => throw new ArgumentOutOfRangeException(nameof(paymentType))
        };
    }

    public static FundOrderErrorCode? NewErrorCode(OrderSide side, string fundConnextErrorCode)
    {
        return fundConnextErrorCode.ToUpper() switch
        {
            "E003" => FundOrderErrorCode.FOE214,
            "E115" => FundOrderErrorCode.FOE202,
            "E102" => FundOrderErrorCode.FOE216,
            "E116" => FundOrderErrorCode.FOE203,
            "E203" => FundOrderErrorCode.FOE211,
            "E206" => FundOrderErrorCode.FOE212,
            "E207" => FundOrderErrorCode.FOE206,
            "E251" or "E252" => side == OrderSide.Buy ? FundOrderErrorCode.FOE204 : FundOrderErrorCode.FOE205,
            "E257" => FundOrderErrorCode.FOE217,
            "E260" => FundOrderErrorCode.FOE213,
            "E261" => FundOrderErrorCode.FOE215,
            "E269" or "E270" or "E271" => FundOrderErrorCode.FOE207,
            "E301" => FundOrderErrorCode.FOE208,
            "E302" => FundOrderErrorCode.FOE209,
            "E303" => FundOrderErrorCode.FOE210,
            _ => null
        };
    }
}
