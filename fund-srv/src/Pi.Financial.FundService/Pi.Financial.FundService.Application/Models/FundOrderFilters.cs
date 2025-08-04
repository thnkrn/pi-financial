using Pi.Financial.FundService.Domain.AggregatesModel.TradingAggregate;

namespace Pi.Financial.FundService.Application.Models;

public record FundOrderFilters(DateOnly EffectiveDateFrom, DateOnly EffectiveDateTo, FundOrderStatus[] FundStatus, FundOrderType? OrderType = null);
