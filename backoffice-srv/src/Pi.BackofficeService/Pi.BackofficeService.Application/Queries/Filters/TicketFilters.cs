using System.Linq.Expressions;
using Pi.BackofficeService.Domain.AggregateModels.TicketAggregate;
using Pi.BackofficeService.Domain.SeedWork;

namespace Pi.BackofficeService.Application.Queries.Filters;

public class TicketFilters : IQueryFilter<TicketState>
{
    public TicketFilters(Guid? responseCode, string? customerCode, Status? status)
    {
        ResponseCode = responseCode;
        CustomerCode = customerCode;
        Status = status;
    }

    public Guid? ResponseCode { get; set; }
    public string? CustomerCode { get; set; }
    public Status? Status { get; set; }

    public List<Expression<Func<TicketState, bool>>> GetExpressions()
    {
        var result = new List<Expression<Func<TicketState, bool>>>();

        if (ResponseCode != null) result.Add(q => q.ResponseCodeId == ResponseCode);
        if (CustomerCode != null) result.Add(q => q.CustomerCode == CustomerCode);
        if (Status != null) result.Add(q => q.Status == Status);

        result.Add(q => q.TicketNo != null);

        return result;
    }
}
