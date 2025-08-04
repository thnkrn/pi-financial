using Pi.Common.Domain;
using Pi.Common.SeedWork;
using Pi.SetService.Domain.AggregatesModel.CommonAggregate;

namespace Pi.SetService.Domain.AggregatesModel.TradingAggregate;

public interface ISblOrderRepository : IRepository<SblOrder>
{
    Task<PaginateResult<SblOrder>> Paginate(PaginateQuery paginateQuery, IQueryFilter<SblOrder> filters, CancellationToken ct = default);
    Task<List<SblOrder>> GetSblOrdersAsync(string tradingAccountNo, IQueryFilter<SblOrder> filters, CancellationToken ct = default);
    Task<SblOrder?> GetSblOrder(Guid id, CancellationToken ct = default);
    void Create(SblOrder sblOrder);
    void Update(SblOrder sblOrder);
    Task<int> DeleteAsync(SblOrder sblOrder, CancellationToken ct = default);
    Task<SblOrder> CreateAsync(SblOrder sblOrder, CancellationToken ct = default);
}
