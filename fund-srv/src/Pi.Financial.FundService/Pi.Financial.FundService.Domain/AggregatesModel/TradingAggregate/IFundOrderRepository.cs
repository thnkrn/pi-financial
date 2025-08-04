using Pi.Common.Domain;
using Pi.Common.SeedWork;

namespace Pi.Financial.FundService.Domain.AggregatesModel.TradingAggregate;

public interface IFundOrderRepository : IRepository<FundOrderState>
{
    Task<FundOrderState?> GetAsync(Guid correlationId, CancellationToken cancellationToken = default);
    Task<List<FundOrderState>> GetAsync(IQueryFilter<FundOrderState> filters, CancellationToken cancellationToken = default);
    Task<List<DateOnly>> GetEffectiveDates(IQueryFilter<FundOrderState> filters, CancellationToken cancellationToken = default);
    Task UpdateOrderNoAsync(Guid correlationId, string orderNo, CancellationToken cancellationToken = default);
    Task UpdateUnitHolderIdAsync(string oldUnitHolderId, string newUnitHolderId, CancellationToken cancellationToken = default);
    Task<FundOrderState> CreateAsync(FundOrderState fundOrderState, CancellationToken cancellationToken = default);
    Task<FundOrderState?> GetByBrokerOrderIdAndOrderTypeAsync(string brokerOrderId, FundOrderType orderType,
        CancellationToken cancellationToken = default);
    Task<List<FundOrderState>> GetByBrokerOrderIds(string[] brokerOrderIds, CancellationToken cancellationToken = default);
    Task<List<FundOrderState>> GetByBrokerOrderIdAndOrderSideAsync(string brokerOrderId, OrderSide orderSide,
        CancellationToken cancellationToken = default);
    Task<List<FundOrderState>> GetByOrderNoAsync(string[] orderNos, CancellationToken cancellationToken = default);
    Task Update(FundOrderState fundOrderState);
}
