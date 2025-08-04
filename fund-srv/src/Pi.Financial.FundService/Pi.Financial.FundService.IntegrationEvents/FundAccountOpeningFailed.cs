namespace Pi.Financial.FundService.IntegrationEvents;

public record FundAccountOpeningFailed
{
    public Guid TicketId { get; init; }
    public string? CustomerCode { get; init; }
    public string? ErrorMessage { get; init; }
}
