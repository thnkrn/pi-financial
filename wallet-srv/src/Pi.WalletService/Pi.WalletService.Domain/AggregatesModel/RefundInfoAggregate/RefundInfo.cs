using Pi.Common.SeedWork;
namespace Pi.WalletService.Domain.AggregatesModel.RefundInfoAggregate;

public class RefundInfo : Entity
{
    public RefundInfo(Guid ticketId, decimal amount, string transferToAccountNo, string transferToAccountName, decimal fee, string currentState)
    {
        Id = Guid.NewGuid();
        TicketId = ticketId;
        Amount = amount;
        TransferToAccountNo = transferToAccountNo;
        TransferToAccountName = transferToAccountName;
        Fee = fee;
        CurrentState = currentState;
    }

    public Guid Id { get; private set; }
    public Guid TicketId { get; private set; }
    public decimal Amount { get; private set; }
    public string TransferToAccountNo { get; private set; }
    public string TransferToAccountName { get; private set; }
    public decimal Fee { get; private set; }
    public string? CurrentState { get; private set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
