using System.Linq.Expressions;
using Pi.BackofficeService.Domain.AggregateModels.ResponseCodeAggregate;
using Pi.BackofficeService.Domain.AggregateModels.TransactionAggregate;
using Pi.BackofficeService.Domain.SeedWork;

namespace Pi.BackofficeService.Application.Queries.Filters;

public class ResponseCodeFilter : IQueryFilter<ResponseCode>
{
    public Machine? Machine { get; set; }
    public ProductType? ProductType { get; set; }
    public bool? HasAction { get; set; }
    public bool? Filterable { get; set; } = true;

    public List<Expression<Func<ResponseCode, bool>>> GetExpressions()
    {
        var result = new List<Expression<Func<ResponseCode, bool>>>();

        if (Machine != null) result.Add(q => q.Machine == Machine);
        if (ProductType != null) result.Add(q => new[] { ProductType, null }.Contains(q.ProductType));
        if (HasAction == true)
        {
            result.Add(q => q.Actions != null && q.Actions.Count != 0);
        }
        if (Filterable != null)
        {
            result.Add(q => q.IsFilterable == Filterable);
        }

        return result;
    }
}
