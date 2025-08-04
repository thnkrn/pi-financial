using Pi.Common.SeedWork;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Domain.AggregatesModel.UtilityAggregate;

public class EmailHistory : Entity
{
    public EmailHistory(Guid ticketId, string transactionNo, EmailType emailType, DateTime sentAt)
    {
        Id = Guid.NewGuid();
        TicketId = ticketId;
        TransactionNo = transactionNo;
        EmailType = emailType;
        SentAt = sentAt;
    }

    public Guid Id { get; private set; }
    public Guid TicketId { get; private set; }
    public string TransactionNo { get; private set; }
    public EmailType EmailType { get; private set; }
    public DateTime SentAt { get; private set; }
}