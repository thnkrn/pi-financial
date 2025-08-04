using Pi.Financial.FundService.Domain.AggregatesModel.TradingAggregate;

namespace Pi.Financial.FundService.Domain.Events;

public record CreateSwitchInFundOrderReceived(FundOrderState FundOrderState);
