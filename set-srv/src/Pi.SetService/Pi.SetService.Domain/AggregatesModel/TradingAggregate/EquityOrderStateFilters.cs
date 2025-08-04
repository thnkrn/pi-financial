using System.Linq.Expressions;
using Pi.Common.Domain;

namespace Pi.SetService.Domain.AggregatesModel.TradingAggregate;

public class EquityOrderStateFilters : IQueryFilter<EquityOrderState>
{
    public string? BrokerOrderId { get; set; }
    public DateOnly? CreatedDate { get; set; }

    public List<Expression<Func<EquityOrderState, bool>>> GetExpressions()
    {
        var expressions = new List<Expression<Func<EquityOrderState, bool>>>();

        if (BrokerOrderId != null)
        {
            expressions.Add(q => q.BrokerOrderId == BrokerOrderId);
        }

        if (CreatedDate != null)
        {
            expressions.Add(q => q.CreatedAtDate == CreatedDate);
        }

        return expressions;
    }
}
