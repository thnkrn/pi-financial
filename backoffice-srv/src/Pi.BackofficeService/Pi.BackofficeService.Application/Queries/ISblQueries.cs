using Pi.BackofficeService.Application.Models.Commons;
using Pi.BackofficeService.Application.Models.Sbl;
using Pi.BackofficeService.Application.Queries.Filters;

namespace Pi.BackofficeService.Application.Queries;

public interface ISblQueries
{
    Task<PaginateResult<SblOrder>> GetSblOrderPaginate(SblOrderFilters filters, int pageNum, int pageSize,
        string? orderBy, string? orderDir, CancellationToken ct = default);
    Task<PaginateResult<SblInstrument>> GetSblInstrumentsPaginate(SblInstrumentFilters filters, int pageNum,
        int pageSize, string? orderBy, string? orderDir, CancellationToken ct = default);
}
