using Pi.BackofficeService.Application.Models.Sbl;

namespace Pi.BackofficeService.Application.Queries.Filters;

public class SblOrderFilters
{
    public string? TradingAccountNo { get; init; }
    public bool? Open { get; init; }
    public string? Symbol { get; init; }
    public SblOrderType? Type { get; init; }
    public SblOrderStatus[]? Statues { get; init; }
    public DateOnly? CreatedDateFrom { get; init; }
    public DateOnly? CreatedDateTo { get; init; }
}
