namespace Pi.BackofficeService.Domain.AggregateModels.TransactionAggregate;

public abstract class Transaction
{
    protected Transaction(string accountNo, string status, string customerCode, string userId)
    {
        AccountNo = accountNo;
        Status = status;
        CustomerCode = customerCode;
        UserId = userId;
    }

    public Guid Id { get; set; }
    public string? CurrentState { get; set; }
    public string AccountNo { get; set; }
    public string? TransactionNo { get; set; }
    public decimal? Amount { get; set; }
    public Currency? Currency { get; set; }
    public string Status { get; set; }
    public string CustomerCode { get; set; }
    public Product Product { get; set; }
    public TransactionType TransactionType { get; set; }
    public string? BankAccountNo { get; set; }
    public string? BankName { get; set; }
    public string UserId { get; set; }
    public string? FailedReason { get; set; }
    public DateTime? EffectiveDateTime { get; set; }
    public DateTime CreatedAt { get; set; }

    public bool IsFailed()
    {
        return Status.ToLower() == "fail";
    }
    public bool IsSuccess()
    {
        return Status.ToLower() == "success";
    }

    public bool IsRefundState()
    {
        return CurrentState != null && CurrentState.ToLower().Contains("refund");
    }
}
