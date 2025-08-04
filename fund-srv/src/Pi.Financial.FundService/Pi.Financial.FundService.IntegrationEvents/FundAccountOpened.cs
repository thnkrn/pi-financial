namespace Pi.Financial.FundService.IntegrationEvents;
public record FundAccountOpened
{
    public FundAccountOpened(Guid ticketId, string accountCode, string customerCode)
    {
        TicketId = ticketId;
        AccountCode = accountCode;
        CustomerCode = customerCode;
    }

    public Guid TicketId { get; init; }
    public string AccountCode { get; init; }
    public string CustomerCode { get; init; }
}
