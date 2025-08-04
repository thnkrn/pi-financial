using Pi.Client.OnePort.GW.DB2.Model;
using Pi.Client.OnePort.GW.TCP.Model;
using Pi.SetService.Application.Models;
using Pi.SetService.Application.Utils;
using Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;
using Pi.SetService.Infrastructure.Utils;
using static Pi.Client.OnePort.GW.TCP.Model.PiOnePortTCPModelsPacketsDataTransferDataTransferNewOrderRequest7A;

namespace Pi.SetService.Infrastructure.Factories;

public static class OnePortFactory
{
    private const string None = " ";

    public static PiOnePortTCPModelsPacketsDataTransferDataTransferNewOrderRequest7A NewNewOrder7A(NewOrder requestedOrder)
    {
        return new PiOnePortTCPModelsPacketsDataTransferDataTransferNewOrderRequest7A(
            requestedOrder.OrderNo,
            requestedOrder.EnterId,
            requestedOrder.SecSymbol,
            NewSideEnum(requestedOrder.Side),
            (double?)requestedOrder.Price,
            NewConPriceEnum(requestedOrder.ConPrice),
            requestedOrder.Volume,
            requestedOrder.PublishVol,
            NewConditionEnum(requestedOrder.Condition),
            NewAccountNo(requestedOrder.AccountNo),
            NewTtfEnum(requestedOrder.Ttf),
            NewOrderTypeEnum(requestedOrder.OrderType),
            "0"
        );
    }

    public static PiOnePortTCPModelsPacketsDataTransferDataTransferOrderChangeRequest7M NewChangeOrderRequest7M(
        ChangeOrder requestedOrder)
    {
        return new PiOnePortTCPModelsPacketsDataTransferDataTransferOrderChangeRequest7M(
            requestedOrder.OrderNo,
            requestedOrder.BrokerOrderId,
            requestedOrder.EnterId,
            NewAccountNo(requestedOrder.TradingAccountNo),
            requestedOrder.Client,
            (PiOnePortTCPModelsPacketsDataTransferDataTransferOrderChangeRequest7M.TtfEnum?)NewTtfEnum(requestedOrder.Ttf),
            (PiOnePortTCPModelsPacketsDataTransferDataTransferOrderChangeRequest7M.OrderTypeEnum?)NewOrderTypeEnum(requestedOrder.OrderType),
            (double)requestedOrder.Price,
            (PiOnePortTCPModelsPacketsDataTransferDataTransferOrderChangeRequest7M.ConPriceEnum?)NewConPriceEnum(requestedOrder.ConPrice),
            requestedOrder.Volume,
            requestedOrder.PublishVol
        );
    }

    public static PiOnePortDb2ModelsOfflineOrderRequest NewPiOnePortDb2ModelsOfflineOrderRequest(NewOfflineOrder requestedOrder)
    {
        return new PiOnePortDb2ModelsOfflineOrderRequest(
            DateTimeHelper.ConvertThTimeFromUtc(requestedOrder.OrderDateTime),
            (long)requestedOrder.OrderNo,
            requestedOrder.EnterId,
            requestedOrder.SecSymbol,
            NewSideRaw(requestedOrder.Side),
            NewAccountNo(requestedOrder.AccountNo),
            requestedOrder.Price,
            requestedOrder.Volume,
            requestedOrder.PubVolume,
            NewServiceTypeRaw(requestedOrder.ServiceType),
            "S",
            "M",
            NewConPriceRaw(requestedOrder.ConditionPrice),
            NewConditionRaw(requestedOrder.Condition),
            requestedOrder.BrokerNo,
            NewTtfRaw(requestedOrder.TrusteeId),
            NewOrderTypeRaw(requestedOrder.OrderType),
            None,
            "D",
            "0000"
        );
    }

    public static PiOnePortDb2ModelsOfflineOrderRequest NewPiOnePortDb2ModelsOfflineOrderRequest(ChangeOfflineOrder requestedOrder)
    {
        return new PiOnePortDb2ModelsOfflineOrderRequest(
            DateTimeHelper.ConvertThTimeFromUtc(requestedOrder.OrderDateTime),
            (long)requestedOrder.BrokerOrderId,
            requestedOrder.EnterId,
            requestedOrder.SecSymbol,
            NewSideRaw(requestedOrder.Side),
            NewAccountNo(requestedOrder.TradingAccountNo),
            requestedOrder.Price,
            requestedOrder.Volume,
            requestedOrder.PubVolume,
            NewServiceTypeRaw(requestedOrder.ServiceType),
            "S",
            "M",
            NewConPriceRaw(requestedOrder.ConditionPrice),
            NewConditionRaw(requestedOrder.Condition),
            requestedOrder.BrokerNo,
            NewTtfRaw(requestedOrder.TrusteeId),
            NewOrderTypeRaw(requestedOrder.OrderType),
            None,
            "D",
            "0000"
        );
    }

    public static string NewAccountNo(string tradingAccountNo)
    {
        return tradingAccountNo.Replace("-", "");
    }

    private static SideEnum NewSideEnum(OrderSide side)
    {
        return side switch
        {
            OrderSide.Buy => SideEnum.Buy,
            OrderSide.Sell => SideEnum.Sell,
            _ => throw new ArgumentOutOfRangeException(nameof(side), side, null)
        };
    }

    private static ConPriceEnum NewConPriceEnum(ConditionPrice conditionPrice)
    {
        return conditionPrice switch
        {
            ConditionPrice.Limit => ConPriceEnum.None,
            ConditionPrice.Ato => ConPriceEnum.Ato,
            ConditionPrice.Atc => ConPriceEnum.Atc,
            ConditionPrice.Mkt => ConPriceEnum.Mkt,
            ConditionPrice.Mtl => ConPriceEnum.Mtl,
            _ => throw new ArgumentOutOfRangeException(nameof(conditionPrice), conditionPrice, null)
        };
    }

    private static ConditionEnum NewConditionEnum(Condition condition)
    {
        return condition switch
        {
            Condition.None => ConditionEnum.None,
            Condition.Ioc => ConditionEnum.Ioc,
            Condition.Fok => ConditionEnum.Fok,
            Condition.Odd => ConditionEnum.Odd,
            Condition.Gtc => ConditionEnum.Gtc,
            Condition.Gtd => ConditionEnum.Gtd,
            _ => throw new ArgumentOutOfRangeException(nameof(condition), condition, null)
        };
    }

    private static TtfEnum NewTtfEnum(Ttf ttf)
    {
        return ttf switch
        {
            Ttf.None => TtfEnum.None,
            Ttf.Nvdr => TtfEnum.Nvdr,
            _ => throw new ArgumentOutOfRangeException(nameof(ttf), ttf, null)
        };
    }

    private static OrderTypeEnum NewOrderTypeEnum(OrderType orderType)
    {
        return orderType switch
        {
            OrderType.ShortCover => OrderTypeEnum.ShortLendingBuyCover,
            OrderType.SellLending => OrderTypeEnum.SellLendingStock,
            OrderType.Program => OrderTypeEnum.ProgramTrading,
            OrderType.MarketProgram => OrderTypeEnum.MarketMakingWithProgramTrade,
            OrderType.Market => OrderTypeEnum.MarketMaking,
            OrderType.Normal => OrderTypeEnum.Normal,
            _ => throw new ArgumentOutOfRangeException(nameof(orderType), orderType, null)
        };
    }

    private static string NewServiceTypeRaw(ServiceType serviceType)
    {
        return serviceType switch
        {
            ServiceType.Dealer => "D",
            ServiceType.Internet => "I",
            ServiceType.Vip => "V",
            _ => throw new ArgumentOutOfRangeException(nameof(serviceType), serviceType, null)
        };
    }

    private static string NewSideRaw(OrderSide side)
    {
        return side switch
        {
            OrderSide.Buy => "B",
            OrderSide.Sell => "S",
            _ => throw new ArgumentOutOfRangeException(nameof(side), side, null)
        };
    }

    private static string NewConPriceRaw(ConditionPrice conditionPrice)
    {
        return conditionPrice switch
        {
            ConditionPrice.Limit => None,
            ConditionPrice.Ato => "A",
            ConditionPrice.Mp => "M",
            ConditionPrice.Atc => "C",
            _ => throw new ArgumentOutOfRangeException(nameof(conditionPrice), conditionPrice, null)
        };
    }

    private static string NewConditionRaw(Condition condition)
    {
        return condition switch
        {
            Condition.None => None,
            Condition.Ioc => "I",
            Condition.Fok => "F",
            Condition.Odd => "O",
            _ => throw new ArgumentOutOfRangeException(nameof(condition), condition, null)
        };
    }

    private static string NewTtfRaw(Ttf ttf)
    {
        return ttf switch
        {
            Ttf.None => None,
            Ttf.Nvdr => "2",
            _ => throw new ArgumentOutOfRangeException(nameof(ttf), ttf, null)
        };
    }

    private static string NewOrderTypeRaw(OrderType orderType)
    {
        return orderType switch
        {
            OrderType.ShortCover => "S",
            OrderType.Normal => None,
            _ => throw new ArgumentOutOfRangeException(nameof(orderType), orderType, null)
        };
    }
}
