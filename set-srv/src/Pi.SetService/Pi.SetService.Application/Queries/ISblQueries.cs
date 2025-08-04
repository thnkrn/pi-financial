using Pi.SetService.Application.Filters;
using Pi.SetService.Domain.AggregatesModel.CommonAggregate;
using Pi.SetService.Domain.AggregatesModel.InstrumentAggregate;
using Pi.SetService.Domain.AggregatesModel.TradingAggregate;

namespace Pi.SetService.Application.Queries;

public interface ISblQueries
{
    Task<PaginateResult<SblOrder>> GetSblOrdersAsync(PaginateQuery paginateQuery, SblOrderFilters filters, CancellationToken ct = default);
    Task<PaginateResult<SblInstrument>> GetSblInstrumentsAsync(PaginateQuery paginateQuery, SblInstrumentFilters filters, CancellationToken ct = default);
}
