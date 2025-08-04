using Pi.BackofficeService.Application.Queries.Filters;
using Pi.BackofficeService.Domain.AggregateModels.ResponseCodeAggregate;
using Pi.BackofficeService.Domain.AggregateModels.TicketAggregate;
using Pi.BackofficeService.Domain.AggregateModels.TransactionAggregate;
using Pi.BackofficeService.Domain.AggregateModels.User;

namespace Pi.BackofficeService.Application.Queries;

public enum TicketOrderable
{
    TransactionNo
}

public record TicketResult(Guid CorrelationId, string TicketNo, Guid? TransactionId, string TransactionNo,
    TransactionType TransactionType, string? CustomerName, string? CustomerCode, Status? Status, Method? RequestAction,
    DateTime? RequestedAt, User? Maker, string? MakerRemark, Method? CheckerAction, DateTime? CheckedAt, User? Checker,
    string? CheckerRemark, ResponseCode? ResponseCode, DateTime CreatedAt, string? Payload);

public interface ITicketQueries
{
    Task<List<TicketResult>> GetTickets(int pageNum, int pageSize, string? orderBy,
        string? orderDirection, TicketFilters? filters);

    Task<int> CountTickets(TicketFilters? filters);

    Task<List<TicketResult>> GetTicketsByTransactionNo(string transactionNo);
    Task<TicketResult> GetTicketByTicketNo(string ticketNo);
}
