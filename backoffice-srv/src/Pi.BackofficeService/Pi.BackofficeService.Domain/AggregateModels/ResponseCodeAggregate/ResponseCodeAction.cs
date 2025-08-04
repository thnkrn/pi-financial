using Pi.BackofficeService.Domain.AggregateModels.TicketAggregate;
using Pi.BackofficeService.Domain.SeedWork;

namespace Pi.BackofficeService.Domain.AggregateModels.ResponseCodeAggregate;

public class ResponseCodeAction : IAggregateRoot
{
    public ResponseCodeAction(Guid id, Guid responseCodeId, Method action)
    {
        Id = id;
        ResponseCodeId = responseCodeId;
        Action = action;
    }

    public Guid Id { get; set; }

    public Guid ResponseCodeId { get; set; }

    public Method Action { get; set; }

    public ResponseCode ResponseCode { get; set; } = null!;
}
