namespace Pi.Financial.FundService.Domain.Events
{
    public record AccountOpeningRequestReceived(
        Guid TicketId,
        string CustomerCode,
        bool Ndid,
        string? NdidRequestId,
        DateTime? NdidDateTime,
        string? OpenAccountRegisterUid);
}
