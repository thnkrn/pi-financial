using System.Linq.Expressions;
using Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;

namespace Pi.SetService.Application.Models;

public record SetOrderHistoriesFilters(DateOnly EffectiveDateFrom, DateOnly EffectiveDateTo, int Limit, int OffSet);

public record SetTradeHistoriesFilters(DateOnly EffectiveDateFrom, DateOnly EffectiveDateTo, int Limit, int OffSet);

public record SetOrderFilters
{
    public DateOnly? EffectiveDateFrom { get; init; }
    public DateOnly? EffectiveDateTo { get; init; }
    public bool? OpenOrder { get; init; }
    public bool? HistoryOrder { get; init; }

    public bool Execute(BaseOrder order)
    {
        if (OpenOrder != null && !order.IsOpenOrder())
            return false;

        if (HistoryOrder != null && order.IsOpenOrder())
            return false;

        if (EffectiveDateFrom != null && EffectiveDateTo != null)
        {
            if (!order.OrderDateTime.HasValue) return false;

            if (EffectiveDateFrom.Value > DateOnly.FromDateTime(order.OrderDateTime.Value) ||
                EffectiveDateTo.Value < DateOnly.FromDateTime(order.OrderDateTime.Value))
                return false;
        }

        return true;
    }

    public IEnumerable<Expression<Func<BaseOrder, bool>>> GetExpressions()
    {
        var result = new List<Expression<Func<BaseOrder, bool>>>();

        if (EffectiveDateFrom != null)
            result.Add(q =>
                q.OrderDateTime.HasValue && DateOnly.FromDateTime(q.OrderDateTime.Value) <= EffectiveDateFrom);

        if (EffectiveDateFrom != null && EffectiveDateTo != null)
            result.Add(q =>
                q.OrderDateTime.HasValue &&
                (EffectiveDateFrom.Value > DateOnly.FromDateTime(q.OrderDateTime.Value) ||
                 EffectiveDateTo.Value < DateOnly.FromDateTime(q.OrderDateTime.Value)));

        if (OpenOrder != null) result.Add(q => q.IsOpenOrder());

        if (HistoryOrder != null) result.Add(q => !q.IsOpenOrder());

        return result;
    }
}