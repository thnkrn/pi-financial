using Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;
using Pi.SetService.Domain.AggregatesModel.InstrumentAggregate;

namespace Pi.SetService.Application.Models;

public class OrderInfo
{
    public required long OrderId { get; init; }
    public required string CustCode { get; init; }
    public required OrderAction Action { get; init; }
    public required decimal AvgPrice { get; init; }
    public required long BrokerOrderId { get; init; }
    public required bool IsNVDR { get; init; }
    public required string OrderNo { get; init; }

    public required string Symbol { get; init; }

    public required decimal Quantity { get; init; }
    public DateTime? OrderDateTime { get; init; }
    public decimal? InterestRate { get; private set; }
    public InstrumentProfile? InstrumentProfile { get; private set; }

    public void SetInterestRate(decimal interestRate)
    {
        InterestRate = interestRate;
    }

    public void SetInstrumentProfile(InstrumentProfile instrumentProfile)
    {
        InstrumentProfile = instrumentProfile;
    }
}

public class SetOrderInfo() : OrderInfo
{
    public required OrderStatus Status { get; init; }
    public required decimal Price { get; init; }
    public required ConditionPrice ConditionPrice { get; init; }
    public required OrderState OrderState { get; init; }
    public int? QuantityExecuted { get; init; }
    public string? RealizedPL { get; init; }
    public decimal? CancelVolume { get; init; }
    public string? Detail { get; init; }
}

public class SblOrderInfo() : OrderInfo
{
    public required SblOrderStatus Status { get; init; }
}
