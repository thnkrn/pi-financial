using Pi.BackofficeService.Application.Models.Sbl;
using Pi.Client.SetService.Model;

namespace Pi.BackofficeService.Infrastructure.Factories;

public static class SetFactory
{
    public static bool TryNewSourceSblOrderSide(
        SblOrderStatus status, out PiSetServiceDomainAggregatesModelFinancialAssetAggregateSblOrderStatus? result)
    {
        result = status switch
        {
            SblOrderStatus.Pending => PiSetServiceDomainAggregatesModelFinancialAssetAggregateSblOrderStatus.Pending,
            SblOrderStatus.Approved => PiSetServiceDomainAggregatesModelFinancialAssetAggregateSblOrderStatus.Approved,
            SblOrderStatus.Rejected => PiSetServiceDomainAggregatesModelFinancialAssetAggregateSblOrderStatus.Rejected,
            _ => null
        };

        return result != null;
    }

    public static PiSetServiceAPIModelsSblSubmitReviewStatus NewSubmitReviewStatus(SblOrderStatus status)
    {
        return status switch
        {
            SblOrderStatus.Approved => PiSetServiceAPIModelsSblSubmitReviewStatus.Approved,
            SblOrderStatus.Rejected => PiSetServiceAPIModelsSblSubmitReviewStatus.Rejected,
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, string.Empty)
        };
    }

    public static bool
        TryNewSourceSblOrderType(SblOrderType? orderType,
            out PiSetServiceDomainAggregatesModelFinancialAssetAggregateSblOrderType? result)
    {
        result = orderType switch
        {
            SblOrderType.Borrow => PiSetServiceDomainAggregatesModelFinancialAssetAggregateSblOrderType.Borrow,
            SblOrderType.Return => PiSetServiceDomainAggregatesModelFinancialAssetAggregateSblOrderType.Return,
            _ => null
        };

        return result != null;
    }

    public static SblOrder NewSblOrder(PiSetServiceDomainAggregatesModelTradingAggregateSblOrder setResponse)
    {
        return new SblOrder
        {
            Id = setResponse.Id,
            TradingAccountId = setResponse.TradingAccountId,
            TradingAccountNo = setResponse.TradingAccountNo,
            CustomerCode = setResponse.CustomerCode,
            OrderId = (ulong)setResponse.OrderId,
            Volume = setResponse.Volume,
            Symbol = setResponse.Symbol,
            Type = NewSblOrderType((PiSetServiceDomainAggregatesModelFinancialAssetAggregateSblOrderType)setResponse.Type!),
            OrderStatus = NewSblOrderStatus((PiSetServiceDomainAggregatesModelFinancialAssetAggregateSblOrderStatus)setResponse.Status!),
            RejectedReason = setResponse.RejectedReason,
            CreatedAt = setResponse.CreatedAt,
            UpdatedAt = setResponse.UpdatedAt
        };
    }

    public static SblOrder NewSblOrder(PiSetServiceApplicationCommandsReviewSblOrderResponse setResponse)
    {
        return new SblOrder
        {
            Id = setResponse.Id,
            TradingAccountId = setResponse.TradingAccountId,
            TradingAccountNo = setResponse.TradingAccountNo,
            CustomerCode = setResponse.CustomerCode,
            OrderId = (ulong)setResponse.OrderId,
            Volume = setResponse.Volume,
            Symbol = setResponse.Symbol,
            Type = NewSblOrderType(setResponse.Type),
            OrderStatus = NewSblOrderStatus(setResponse.Status),
            RejectedReason = setResponse.RejectedReason,
            CreatedAt = setResponse.CreatedAt,
            UpdatedAt = setResponse.UpdatedAt
        };
    }

    public static SblInstrument NewSblInstrument(PiSetServiceDomainAggregatesModelInstrumentAggregateSblInstrument setResponse)
    {
        return new SblInstrument
        {
            Symbol = setResponse.Symbol,
            InterestRate = setResponse.InterestRate,
            RetailLender = setResponse.RetailLender,
            BorrowOutstanding = setResponse.BorrowOutstanding,
            AvailableLending = setResponse.AvailableLending,
            CreatedAt = setResponse.CreatedAt,
            UpdatedAt = setResponse.UpdatedAt
        };
    }

    public static SblOrderType NewSblOrderType(PiSetServiceDomainAggregatesModelFinancialAssetAggregateSblOrderType type)
    {
        return type switch
        {
            PiSetServiceDomainAggregatesModelFinancialAssetAggregateSblOrderType.Borrow => SblOrderType.Borrow,
            PiSetServiceDomainAggregatesModelFinancialAssetAggregateSblOrderType.Return => SblOrderType.Return,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    public static SblOrderStatus NewSblOrderStatus(PiSetServiceDomainAggregatesModelFinancialAssetAggregateSblOrderStatus status)
    {
        return status switch
        {
            PiSetServiceDomainAggregatesModelFinancialAssetAggregateSblOrderStatus.Pending => SblOrderStatus.Pending,
            PiSetServiceDomainAggregatesModelFinancialAssetAggregateSblOrderStatus.Approved => SblOrderStatus.Approved,
            PiSetServiceDomainAggregatesModelFinancialAssetAggregateSblOrderStatus.Rejected => SblOrderStatus.Rejected,
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };
    }
}
