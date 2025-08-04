using Pi.BackofficeService.Domain.AggregateModels.ResponseCodeAggregate;
using Pi.BackofficeService.Domain.AggregateModels.TransactionAggregate;

namespace Pi.BackofficeService.API.Models;

public record ResponseCodeResponse(
    Guid Id,
    Machine? Machine,
    ProductType? ProductType,
    string? Suggestion,
    string Description,
    string? State
);

public record ResponseCodeDetailResponse(
    Guid Id,
    string? Suggestion,
    string Description,
    List<ResponseCodeActionsResponse> Actions
);

public record ResponseCodeActionsResponse : NameAliasResponse<TicketAction>
{
    public ResponseCodeActionsResponse(Guid id, TicketAction data) : base(data)
    {
        Id = id;
    }

    public Guid Id { get; set; }
}
