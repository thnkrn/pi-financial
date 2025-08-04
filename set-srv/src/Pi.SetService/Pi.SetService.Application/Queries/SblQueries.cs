using Pi.SetService.Application.Filters;
using Pi.SetService.Domain.AggregatesModel.CommonAggregate;
using Pi.SetService.Domain.AggregatesModel.InstrumentAggregate;
using Pi.SetService.Domain.AggregatesModel.TradingAggregate;

namespace Pi.SetService.Application.Queries;

public class SblQueries(
    ISblOrderRepository sblOrderRepository,
    IInstrumentRepository instrumentRepository) : ISblQueries
{
    public async Task<PaginateResult<SblOrder>> GetSblOrdersAsync(PaginateQuery paginateQuery, SblOrderFilters filters,
        CancellationToken ct = default)
    {
        return await sblOrderRepository.Paginate(paginateQuery, filters, ct);
    }

    public async Task<PaginateResult<SblInstrument>> GetSblInstrumentsAsync(PaginateQuery paginateQuery,
        SblInstrumentFilters filters, CancellationToken ct = default)
    {
        return await instrumentRepository.PaginateSblInstruments(paginateQuery, filters, ct);
    }
}
