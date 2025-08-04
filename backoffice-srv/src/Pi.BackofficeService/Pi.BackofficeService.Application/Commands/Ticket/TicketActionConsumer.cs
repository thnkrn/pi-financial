using MassTransit;
using Pi.BackofficeService.Domain.AggregateModels.TicketAggregate;
using Pi.BackofficeService.Domain.AggregateModels.User;
using Pi.BackofficeService.Domain.Events.Ticket;
using Pi.BackofficeService.Domain.Exceptions;

namespace Pi.BackofficeService.Application.Commands.Ticket;

public record MakerRequestActionRequest(Guid CorrelationId, Guid UserId, Method Method, string? Remark);
public record CheckerSelectActionRequest(string TicketNo, Guid UserId, Method Method, string? Remark);

public class TicketActionConsumer : IConsumer<MakerRequestActionRequest>, IConsumer<CheckerSelectActionRequest>
{
    private readonly IUserRepository _userRepository;

    public TicketActionConsumer(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task Consume(ConsumeContext<MakerRequestActionRequest> context)
    {
        var user = await _userRepository.Get(context.Message.UserId) ?? throw new NotFoundException("User not found");

        await context.Publish(new TicketPendingEvent(
            context.Message.CorrelationId,
            user.Id,
            context.Message.Method,
            context.Message.Remark
        ), publishContext =>
        {
            publishContext.RequestId = context.RequestId;
            publishContext.ResponseAddress = context.ResponseAddress;
        });
    }

    public async Task Consume(ConsumeContext<CheckerSelectActionRequest> context)
    {
        var user = await _userRepository.Get(context.Message.UserId) ?? throw new NotFoundException("User not found");

        await context.Publish(new CheckTicketEvent(
            context.Message.TicketNo,
            user.Id,
            context.Message.Method,
            context.Message.Remark
        ), publishContext =>
        {
            publishContext.RequestId = context.RequestId;
            publishContext.ResponseAddress = context.ResponseAddress;
        });
    }
}
