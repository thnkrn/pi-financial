using Pi.BackofficeService.Application.Models.Commons;
using Pi.BackofficeService.Application.Models.Sbl;
using Pi.BackofficeService.Application.Queries.Filters;
using Pi.BackofficeService.Application.Services.SblService;

namespace Pi.BackofficeService.Application.Queries;

public class SblQueries(ISblService sblService) : ISblQueries
{
    public async Task<PaginateResult<SblOrder>> GetSblOrderPaginate(SblOrderFilters filters, int pageNum, int pageSize,
        string? orderBy, string? orderDir, CancellationToken ct = default)
    {
        return await sblService.GetSblOrderPaginateAsync(filters, pageNum, pageSize, orderBy, orderDir, ct);
    }

    public async Task<PaginateResult<SblInstrument>> GetSblInstrumentsPaginate(SblInstrumentFilters filters, int pageNum, int pageSize, string? orderBy,
        string? orderDir, CancellationToken ct = default)
    {
        return await sblService.GetSblInstrumentsPaginateAsync(filters, pageNum, pageSize, orderBy, orderDir, ct);
    }
}
