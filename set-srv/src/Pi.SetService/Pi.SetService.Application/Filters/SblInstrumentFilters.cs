using System.Linq.Expressions;
using Pi.Common.Domain;
using Pi.SetService.Domain.AggregatesModel.InstrumentAggregate;

namespace Pi.SetService.Application.Filters;

public class SblInstrumentFilters : IQueryFilter<SblInstrument>
{
    public string? Symbol { get; init; }

    public List<Expression<Func<SblInstrument, bool>>> GetExpressions()
    {
        var expressions = new List<Expression<Func<SblInstrument, bool>>>();

        if (Symbol != null)
        {
            expressions.Add(q => q.Symbol.ToLower().Contains(Symbol));
        }

        return expressions;
    }
}
