namespace Pi.BackofficeService.Domain.AggregateModels.TransactionAggregate;

public class RefundTransaction : Transaction
{
    public RefundTransaction(string accountNo, string status, string customerCode, string userId) : base(accountNo, status, customerCode, userId)
    {
    }

    public string? DepositTransactionNo { get; set; }
    public DateTime? RefundedAt { get; set; }
}
