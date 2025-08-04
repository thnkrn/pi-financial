using System.Linq.Expressions;
using Pi.Common.Domain;
using Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;
using Pi.SetService.Domain.AggregatesModel.TradingAggregate;

namespace Pi.SetService.Application.Filters;

public class SblOrderFilters : IQueryFilter<SblOrder>
{
    public string? TradingAccountNo { get; init; }
    public bool? Open { get; init; }
    public string? Symbol { get; init; }
    public SblOrderStatus[]? Statues { get; init; }
    public SblOrderType? Type { get; init; }
    public DateOnly? CreatedDateFrom { get; init; }
    public DateOnly? CreatedDateTo { get; init; }

    public List<Expression<Func<SblOrder, bool>>> GetExpressions()
    {
        var expressions = new List<Expression<Func<SblOrder, bool>>>();

        switch (Open)
        {
            case true:
                expressions.Add(q => SblOrderStatus.Pending == q.Status);
                break;
            case false:
                expressions.Add(q => new[] { SblOrderStatus.Approved, SblOrderStatus.Rejected }.Contains(q.Status));
                break;
        }

        if (TradingAccountNo != null)
        {
            expressions.Add(q => q.TradingAccountNo == TradingAccountNo);
        }

        if (Statues != null)
        {
            expressions.Add(q => Statues.Contains(q.Status));
        }

        if (Symbol is { Length: > 1 })
        {
            expressions.Add(q => q.Symbol.ToLower().Contains(Symbol));
        }

        if (Type != null)
        {
            expressions.Add(q => q.Type == Type);
        }

        if (CreatedDateFrom != null)
        {
            expressions.Add(q => DateOnly.FromDateTime(q.CreatedAt) >= CreatedDateFrom.Value);
        }

        if (CreatedDateTo != null)
        {
            expressions.Add(q => DateOnly.FromDateTime(q.CreatedAt) <= CreatedDateTo.Value);
        }

        return expressions;
    }
}
