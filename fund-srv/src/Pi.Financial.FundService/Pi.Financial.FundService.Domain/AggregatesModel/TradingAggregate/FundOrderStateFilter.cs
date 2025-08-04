using System.Linq.Expressions;
using Pi.Common.Domain;

namespace Pi.Financial.FundService.Domain.AggregatesModel.TradingAggregate;

public class FundOrderStateFilter : IQueryFilter<FundOrderState>
{
    public bool? DummyUnitHolder { get; set; }
    public FundOrderStatus? Status { get; set; }
    public FundOrderStatus[]? Statuses { get; set; }

    public List<Expression<Func<FundOrderState, bool>>> GetExpressions()
    {
        var expressions = new List<Expression<Func<FundOrderState, bool>>>();

        if (DummyUnitHolder != null) expressions.Add(q => q.UnitHolderId != null && q.UnitHolderId.ToLower().StartsWith("dm"));
        if (Status != null) expressions.Add(q => q.OrderStatus == Status);
        if (Statuses != null) expressions.Add(q => Statuses.Contains(q.OrderStatus));

        return expressions;
    }
}
