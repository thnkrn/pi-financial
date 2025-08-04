using Pi.BackofficeService.Domain.SeedWork;

namespace Pi.BackofficeService.Domain.AggregateModels.TicketAggregate;

[Serializable]
public class DuplicateTicketNoException : Exception
{
    public DuplicateTicketNoException(string message, Exception? innerException) : base(message, innerException)
    {
    }

    public DuplicateTicketNoException()
    {
    }

    public DuplicateTicketNoException(string message)
        : base(message)
    {
    }
}

public interface ITicketRepository : IRepository
{
    Task<List<TicketState>> Get(int pageNum, int pageSize, string? orderBy, string? orderDir, IQueryFilter<TicketState>? filters);
    Task<int> Count(IQueryFilter<TicketState>? filters);
    Task<int> Count();
    Task<string?> GetLatestTicketNo();
    Task<TicketState?> GetByTicketNo(string ticketNo);
    Task<TicketState?> GetById(Guid correlationId);
    Task<List<TicketState>> GetByTransactionId(Guid transactionId);
    Task<List<TicketState>> GetByTransactionNo(string transactionNo);
    Task<TicketState> Create(TicketState ticketState);
    Task UpdateTicketNo(Guid correlationId, string ticketNo);
    void Update(TicketState ticketState);
}
