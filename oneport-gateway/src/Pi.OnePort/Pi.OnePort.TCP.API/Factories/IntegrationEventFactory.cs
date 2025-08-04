using Pi.OnePort.IntegrationEvents;
using Pi.OnePort.IntegrationEvents.Models;
using Pi.OnePort.TCP.API.Extensions;
using Pi.OnePort.TCP.Models.Packets.DataTransfer;

namespace Pi.OnePort.TCP.API.Factories;

public static class IntegrationEventFactory
{
    public static OnePortBrokerOrderCreated NewOnePortBrokerOrderCreated(DataTransferNewOrderResponse6A data,
        DateTime packetDateTime)
    {
        return new OnePortBrokerOrderCreated
        {
            RefOrderId = TrimPrefixWithoutEmpty(data.RefOrderId),
            AccountId = data.Account,
            Side = data.Side.ToIntegration(),
            Type = data.OrderType.ToIntegration(),
            Symbol = data.SecSymbol,
            Volume = data.Volume,
            PubVolume = data.PublishVolume,
            Price = data.Price,
            ConPrice = data.ConPrice.ToIntegration(),
            Condition = data.Condition.ToIntegration(),
            Ttf = data.Ttf.ToIntegration(),
            CheckFlag = data.CheckFlag,
            EnterId = data.EnterId,
            Channel = NewChannel(data.EnterId),
            FisOrderId = TrimPrefix(data.FisOrderId),
            TransactionDateTime = packetDateTime
        };
    }

    public static OnePortOrderMatched NewOnePortOrderMatched(DataTransferExecutionReportResponse7E data,
        DateTime packetDateTime)
    {
        return new OnePortOrderMatched
        {
            RefOrderId = TrimPrefixWithoutEmpty(data.RefOrderId),
            ExecutionTransType = data.ExecutionTransType.ToIntegration(),
            Source = data.Source.ToIntegration(),
            Symbol = data.SecSymbol,
            Side = data.Side.ToIntegration(),
            Volume = data.Volume,
            Price = data.Price,
            FisOrderId = TrimPrefix(data.FisOrderId),
            TransactionDateTime = packetDateTime
        };
    }

    public static OnePortOrderCanceled NewOnePortOrderCancel(DataTransferExecutionReportResponse7E data,
        DateTime packetDateTime)
    {
        return new OnePortOrderCanceled
        {
            RefOrderId = TrimPrefixWithoutEmpty(data.RefOrderId),
            ExecutionTransType = data.ExecutionTransType.ToIntegration(),
            Source = data.Source.ToIntegration(),
            Symbol = data.SecSymbol,
            Side = data.Side.ToIntegration(),
            CancelVolume = data.Volume,
            FisOrderId = TrimPrefix(data.FisOrderId),
            TransactionDateTime = packetDateTime
        };
    }

    public static OnePortOrderRejected NewOnePortOrderRejected(DataTransferOrderAcknowledgementResponse7K data, DateTime packetDateTime)
    {
        return new OnePortOrderRejected
        {
            RefOrderId = TrimPrefixWithoutEmpty(data.RefOrderId),
            ExecutionTransType = data.ExecutionTransType.ToIntegration(),
            Source = data.Source.ToIntegration(),
            ExecutionTransRejectType = data.ExecutionTransRejectType.ToIntegration(),
            Reason = data.Reason,
            FisOrderId = TrimPrefix(data.FisOrderId),
            TransactionDateTime = packetDateTime
        };
    }

    public static OnePortOrderRejected NewOnePortOrderRejected(DataTransferOrderChangeResponse7N data, DateTime packetDateTime)
    {
        return new OnePortOrderRejected
        {
            RefOrderId = TrimPrefixWithoutEmpty(data.RefOrderId),
            ExecutionTransType = data.ExecutionTransType.ToIntegration(),
            Source = data.Source.ToIntegration(),
            ExecutionTransRejectType = data.ExecutionTransRejectType.ToIntegration(),
            Reason = data.Reason,
            FisOrderId = TrimPrefix(data.FisOrderId),
            TransactionDateTime = packetDateTime
        };
    }

    public static OnePortOrderChanged NewOnePortOrderChanged(DataTransferOrderChangeResponse7N data, DateTime packetDateTime)
    {
        return new OnePortOrderChanged
        {
            RefOrderId = TrimPrefixWithoutEmpty(data.RefOrderId),
            AccountId = data.Account,
            Volume = data.Volume,
            Price = data.Price,
            EnterId = data.EnterId,
            Channel = NewChannel(data.EnterId),
            Ttf = data.Ttf.ToIntegration(),
            FisOrderId = TrimPrefix(data.FisOrderId),
            TransactionDateTime = packetDateTime
        };
    }

    public static OnePortOrderChanged NewOnePortOrderChanged(DataTransferOrderChangeByBrokerResponse6T data, DateTime packetDateTime)
    {
        return new OnePortOrderChanged
        {
            RefOrderId = TrimPrefixWithoutEmpty(data.RefOrderId),
            AccountId = data.Account,
            Volume = data.Volume,
            Price = data.Price,
            EnterId = data.EnterId,
            Channel = NewChannel(data.EnterId),
            Ttf = data.Ttf.ToIntegration(),
            FisOrderId = TrimPrefix(data.FisOrderId),
            TransactionDateTime = packetDateTime
        };
    }

    public static string? TrimPrefixWithoutEmpty(string orderId)
    {
        var trimmed = TrimPrefix(orderId);
        return trimmed != "" ? trimmed : null;
    }

    private static string TrimPrefix(string orderId)
    {
        return orderId.TrimStart('0').Trim(' ');
    }

    private static OrderChannel NewChannel(string channel)
    {
        return channel switch
        {
            "9000" => OrderChannel.Ifise,
            "9004" => OrderChannel.Settrade,
            "9005" => OrderChannel.Mt5,
            "9006" => OrderChannel.EFinTradePlus,
            "9009" => OrderChannel.PiFinancialApp,
            _ => OrderChannel.Ic
        };
    }
}
