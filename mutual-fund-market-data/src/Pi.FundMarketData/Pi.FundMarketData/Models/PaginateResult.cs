namespace Pi.FundMarketData.Models;

public record PaginateResult<T>(IEnumerable<T> Records, int Page, int PageSize, int Total);
