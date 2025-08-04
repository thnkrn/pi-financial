using Pi.BackofficeService.Application.Queries;
using Pi.BackofficeService.Domain.AggregateModels.ResponseCodeAggregate;
using Pi.BackofficeService.Domain.AggregateModels.TicketAggregate;
using Pi.BackofficeService.Domain.AggregateModels.User;

namespace Pi.BackofficeService.Application.Factories;

public static class QueriesFactory
{
    public static ResponseCodeDetail NewResponseCodeDetail(ResponseCode responseCode, List<ResponseCodeAction> handlerActions)
    {
        return new ResponseCodeDetail(
            responseCode.Id,
            responseCode.Machine,
            responseCode.State,
            responseCode.Suggestion,
            responseCode.Description,
            handlerActions
        );
    }

    public static TicketResult NewTicketResponse(TicketState ticketState, User? maker, User? checker, ResponseCode? responseCode)
    {
        return new TicketResult(
            ticketState.CorrelationId,
            ticketState.TicketNo!,
            ticketState.TransactionId,
            ticketState.TransactionNo,
            ticketState.TransactionType,
            ticketState.CustomerName,
            ticketState.CustomerCode,
            ticketState.Status,
            ticketState.RequestAction,
            ticketState.RequestedAt,
            maker,
            ticketState.MakerRemark,
            ticketState.CheckerAction,
            ticketState.CheckedAt,
            checker,
            ticketState.CheckerRemark,
            responseCode,
            ticketState.CreatedAt,
            ticketState.Payload
        );
    }
}
