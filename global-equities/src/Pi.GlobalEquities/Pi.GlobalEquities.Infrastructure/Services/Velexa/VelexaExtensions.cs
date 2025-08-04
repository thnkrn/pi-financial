using System.ComponentModel;
using System.Globalization;
using Pi.Common.CommonModels;
using Pi.GlobalEquities.DomainModels;
using Pi.GlobalEquities.Models;
using Pi.GlobalEquities.Models.MarketData;
using OrderResponse = Pi.GlobalEquities.Infrastructure.Services.Velexa.VelexaModel.OrderResponse;
using TransactionResponse = Pi.GlobalEquities.Infrastructure.Services.Velexa.VelexaModel.TransactionResponse;
using VelexaPosition = Pi.GlobalEquities.Infrastructure.Services.Velexa.VelexaModel.VelexaPosition;

namespace Pi.GlobalEquities.Infrastructure.Services.Velexa;

public static class VelexaExtensions
{
    internal static Order MapToOrder(this OrderResponse response, OrderTagInfo? orderTagInfo, Channel channel = Channel.Online)
    {
        if (response == null)
            throw new ArgumentNullException(nameof(response));

        var parameters = response.orderParameters;
        if (parameters == null)
            throw new Exception($"Invalid Velexa response, {nameof(response.orderParameters)} is null");

        var state = response.orderState;
        if (state == null)
            throw new Exception($"Invalid Velexa response, ${nameof(response.orderState)} is null");

        var pvdInfo = new ProviderInfo
        {
            ModificationId = response.currentModificationId,
            ProviderName = Provider.Velexa,
            AccountId = response.accountId,
            OrderId = response.orderId,
            Status = state.status,
            StatusReason = state.reason,
            CreatedAt = response.placeTime,
            ModifiedAt = state.lastUpdate,
        };

        decimal? limitPrice = !string.IsNullOrWhiteSpace(parameters.limitPrice)
            ? decimal.Parse(parameters.limitPrice, NumberStyles.Float)
            : null;
        decimal? stopPrice = !string.IsNullOrWhiteSpace(parameters.stopPrice)
            ? decimal.Parse(parameters.stopPrice, NumberStyles.Float)
            : null;
        var quantity = decimal.Parse(parameters.quantity, NumberStyles.Float);
        var fills = state.fills?.Select(x => new OrderFill
        {
            Quantity = decimal.Parse(x.quantity, NumberStyles.Float),
            Price = decimal.Parse(x.price, NumberStyles.Float),
            Timestamp = x.timestamp
        });

        return new Order
        {
            UserId = orderTagInfo?.UserId,
            AccountId = orderTagInfo?.AccountId,
            Channel = channel,
            Id = response.orderId,
            GroupId = response.orderParameters.ocoGroup,
            Venue = parameters.Venue,
            Symbol = parameters.Symbol,
            OrderType = orderTagInfo?.OrderType ?? GetOrderType(response.orderParameters.orderType),
            Side = GetOrderSide(parameters.side),
            LimitPrice = limitPrice,
            StopPrice = stopPrice,
            Quantity = quantity,
            Status = GetOrderStatus(state.status,
                state.fills != null && state.fills.Any()),
            Duration = GetOrderDuration(parameters.duration),
            Fills = fills,
            ProviderInfo = pvdInfo,
            StatusReason = GetOrderStatusReason(state.reason),
            UpdatedAt = response.orderState.lastUpdate,
            CreatedAt = response.placeTime,
        };
    }

    internal static TransactionItem MapToTransaction(this TransactionResponse vTran)
    {
        var symbolParts = vTran.symbolId?.Split(".");
        var symbol = symbolParts?.Length >= 1 ? symbolParts[0] : null;
        var venue = symbolParts?.Length == 2 ? symbolParts[1] : null;

        return new TransactionItem
        {
            Id = vTran.uuid,
            ParentId = vTran.parentUuid,
            OrderId = vTran.orderId,
            Venue = venue,
            Symbol = symbol,
            Asset = vTran.asset,
            Value = decimal.TryParse(vTran.sum, out decimal value) ? value : 0,
            OperationType = GetOperationType(vTran.operationType),
            Timestamp = vTran.timestamp
        };
    }

    internal static PositionSummary MapToAccountSummaryPosition(
        this VelexaPosition vpos)
    {
        return new PositionSummary
        {
            Symbol = vpos.symbolId?.Split(".")[0],
            Venue = vpos.symbolId?.Split(".")[1],
            Currency = string.IsNullOrWhiteSpace(vpos.currency) ? 0 : (Currency)Enum.Parse(typeof(Currency), vpos.currency, true),
            EntireQuantity = Convert.ToDecimal(vpos.quantity ?? "0"),
            SymbolType = vpos.symbolType,
            LastPrice = Convert.ToDecimal(vpos.price ?? "0"),
            MarketValue = Convert.ToDecimal(vpos.value ?? "0"),
            ConvertedMarketValue = Convert.ToDecimal(vpos.convertedValue ?? "0"),
            AveragePrice = Convert.ToDecimal(vpos.averagePrice ?? "0"),
            Upnl = Convert.ToDecimal(vpos.pnl ?? "0"),
            ConvertedUpnl = Convert.ToDecimal(vpos.convertedPnl ?? "0"),
            Cost = Convert.ToDecimal(vpos.value ?? "0") - Convert.ToDecimal(vpos.pnl ?? "0"),
            ConvertedCost = Convert.ToDecimal(vpos.convertedValue ?? "0") - Convert.ToDecimal(vpos.convertedPnl ?? "0"),
            UpnlPercentage = Convert.ToDecimal(vpos.convertedPnl ?? "0")
        };
    }

    internal static OrderStatus GetOrderStatus(this string status, bool hasFilled)
    {
        if (status is "cancelled" && hasFilled)
            return OrderStatus.PartiallyMatched;

        return status switch
        {
            "pending" or "placing" => OrderStatus.Queued,
            "working" => OrderStatus.Processing,
            "cancelled" => OrderStatus.Cancelled,
            "filled" => OrderStatus.Matched,
            "rejected" => OrderStatus.Rejected,
            _ => OrderStatus.Unknown
        };
    }

    static OrderType GetOrderType(string type)
    {
        return type switch
        {
            "market" => OrderType.Market,
            "limit" => OrderType.Limit,
            "stop_limit" => OrderType.StopLimit,
            "stop" => OrderType.Stop,
            "twap" => OrderType.Twap,
            "trailing_stop" => OrderType.TrailingStop,
            "iceberg" => OrderType.Iceberg,
            "pov" => OrderType.Pov,
            "vwap" => OrderType.Vwap,
            _ => OrderType.Unknown
        };
    }


    internal static string GetVelexaOrderType(this OrderType type)
    {
        return type switch
        {
            OrderType.Limit or OrderType.TakeProfit => "limit",
            OrderType.Market => "market",
            OrderType.StopLimit => "stop_limit",
            OrderType.Stop or OrderType.StopLoss => "stop",
            OrderType.Twap => "twap",
            OrderType.TrailingStop => "trailing_stop",
            OrderType.Iceberg => "iceberg",
            _ => throw new NotSupportedException(type.ToString())
        };
    }

    internal static string GetVelexaOrderDuration(this OrderDuration duration)
    {
        return duration switch
        {
            OrderDuration.Day => "day",
            OrderDuration.Fok => "fill_or_kill",
            OrderDuration.Ioc => "immediate_or_cancel",
            OrderDuration.Gtc => "good_till_cancel",
            OrderDuration.Gtt => "good_till_time",
            OrderDuration.Ato => "at_the_opening",
            OrderDuration.Atc => "at_the_close",
            _ => throw new NotSupportedException(duration.ToString())
        };
    }

    private static OrderSide GetOrderSide(this string side)
    {
        return side switch
        {
            "buy" => OrderSide.Buy,
            "sell" => OrderSide.Sell,
            _ => throw new InvalidEnumArgumentException($"Unknown order side from Velexa: {side}")
        };
    }

    internal static OrderDuration GetOrderDuration(this string duration)
    {
        return duration switch
        {
            "day" => OrderDuration.Day,
            "fill_or_kill" => OrderDuration.Fok,
            "immediate_or_cancel" => OrderDuration.Ioc,
            "good_till_cancel" => OrderDuration.Gtc,
            "good_till_time" => OrderDuration.Gtt,
            "at_the_opening" => OrderDuration.Ato,
            "at_the_close" => OrderDuration.Atc,
            _ => OrderDuration.Unknown
        };
    }

    private static OperationType GetOperationType(this string operationType)
    {
        return operationType switch
        {
            "TRADE" => OperationType.Trade,
            "COMMISSION" => OperationType.Commission,
            "AUTOCONVERSION" => OperationType.AutoConversion,
            "STOCK SPLIT" => OperationType.StockSplit,
            "DIVIDEND" => OperationType.Dividend,
            "US TAX" => OperationType.DividendTax,
            "TAX" => OperationType.Tax,
            "CORPORATE ACTION" => OperationType.CorporateAction,
            _ => OperationType.Unknown
        };
    }

    internal static Session GetSession(this string session)
    {
        return session switch
        {
            "PreMarket" => Session.PreMarket,
            "MainSession" => Session.MainSession,
            "AfterMarket" => Session.AfterMarket,
            "Clearing" => Session.Clearing,
            "Offline" => Session.Offline,
            "Online" => Session.Online,
            "Expired" => Session.Expired,
            _ => Session.Unknown
        };
    }

    private static OrderReason? GetOrderStatusReason(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            return null;

        return s_reasonMaps.FirstOrDefault(kvp => reason.Contains(kvp.Key, StringComparison.OrdinalIgnoreCase))
            .Value is var mapped
            ? mapped
            : OrderReason.Unknown;
    }

    private static readonly Dictionary<string, OrderReason> s_reasonMaps = new()
    {
        { "Insufficient margin", OrderReason.InsufficientFund },
        { "Order quantity doesn't respect lot size", OrderReason.IncorrectQuantity },
        { "Invalid price", OrderReason.InvalidPrice },
        { "Operation rejected", OrderReason.OperationRejected },
        { "Waiting for the parent order execution", OrderReason.WaitingParentExecution },
        { "Waiting for the next trading session", OrderReason.WaitingNextTradingSession }
    };
}
