using Pi.BackofficeService.Application.Models.Commons;
using Pi.BackofficeService.Application.Models.Sbl;
using Pi.BackofficeService.Application.Queries.Filters;

namespace Pi.BackofficeService.Application.Services.SblService;

public interface ISblService
{
    Task<PaginateResult<SblOrder>> GetSblOrderPaginateAsync(SblOrderFilters filters, int pageNum, int pageSize, string? orderBy, string? orderDir, CancellationToken ct = default);
    Task<PaginateResult<SblInstrument>> GetSblInstrumentsPaginateAsync(SblInstrumentFilters filters, int pageNum, int pageSize, string? orderBy, string? orderDir, CancellationToken ct = default);
    Task<int> UploadSblInstruments(string fileName, Stream content, CancellationToken ct = default);
    Task<SblOrder> SubmitReviewSblOrderAsync(SubmitReview submitReview, CancellationToken ct = default);
}
