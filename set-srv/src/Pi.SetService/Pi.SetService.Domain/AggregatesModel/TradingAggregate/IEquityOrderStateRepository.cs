using Pi.Common.Domain;

namespace Pi.SetService.Domain.AggregatesModel.TradingAggregate;

public interface IEquityOrderStateRepository
{
    Task<IEnumerable<EquityOrderState>> GetEquityOrderStates(IQueryFilter<EquityOrderState> filters);
    Task UpdateOrderNoAsync(Guid correlationId, string orderNo, CancellationToken cancellationToken = default);
}
