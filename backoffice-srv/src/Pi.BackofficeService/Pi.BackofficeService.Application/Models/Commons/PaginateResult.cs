namespace Pi.BackofficeService.Application.Models.Commons;

public record PaginateResult<T>(List<T> Records, int Page, int PageSize, int Total, string? OrderBy, string? OrderDir);
