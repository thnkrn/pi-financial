
namespace Pi.SetService.Domain.AggregatesModel.CommonAggregate;

public record PaginateQuery
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? OrderBy { get; init; }
    public OrderDirection? OrderDir { get; init; }
}

public record PaginateResult<T> where T : class
{
    public required IEnumerable<T> Data { get; init; }
    public required int Page { get; init; }
    public required int PageSize { get; init; }
    public required int Total { get; init; }
    public string? OrderBy { get; init; }
    public OrderDirection? OrderDir { get; init; }
}

public enum OrderDirection
{
    Asc,
    Desc
}
