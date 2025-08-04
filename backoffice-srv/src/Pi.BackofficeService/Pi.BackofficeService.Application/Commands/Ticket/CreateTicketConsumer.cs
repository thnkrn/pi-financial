using MassTransit;
using Pi.BackofficeService.Domain.AggregateModels.TicketAggregate;
using Pi.BackofficeService.Domain.AggregateModels.TransactionAggregate;
using Pi.BackofficeService.Domain.Events;
using Pi.BackofficeService.Domain.Events.Ticket;
using Pi.BackofficeService.Domain.Events.Transaction;

namespace Pi.BackofficeService.Application.Commands.Ticket;

public record TicketCreateRequest(Guid CorrelationId, string TransactionNo, TransactionType TransactionType, string? Payload);
public record CannotCreateTicketResponse(string ErrorMessage);

public class CreateTicketConsumer : IConsumer<FailedTransactionEvent>, IConsumer<TicketCreateRequest>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IBus _bus;

    public CreateTicketConsumer(IBus bus, ITicketRepository ticketRepository)
    {
        _ticketRepository = ticketRepository;
        _bus = bus;
    }

    public Task Consume(ConsumeContext<FailedTransactionEvent> context)
    {
        // Will subscribe wallet event in the future
        throw new NotImplementedException();
    }

    public async Task Consume(ConsumeContext<TicketCreateRequest> context)
    {
        var tickets = await _ticketRepository.GetByTransactionNo(context.Message.TransactionNo);

        if (tickets.Count != 0 && tickets.Exists(q => q.Status != Status.Rejected && q.Status != Status.Approved))
        {
            await context.RespondAsync(new CannotCreateTicketResponse("Still have ticket in progress"));

            return;
        }

        Response response = await _bus.CreateRequestClient<CreateTicketEvent>()
        .GetResponse<TicketNoGeneratedResponse, FailedErrorResponse>(new CreateTicketEvent(
            context.Message.CorrelationId,
            context.Message.TransactionNo,
            context.Message.TransactionType,
            context.Message.Payload
        ));

        switch (response.Message)
        {
            case TicketNoGeneratedResponse success:
                await context.RespondAsync(success);
                break;
            case FailedErrorResponse failed:
                await context.RespondAsync(new CannotCreateTicketResponse(failed.ErrorMsg));
                break;
        }
    }
}
