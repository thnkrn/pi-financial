using MassTransit;
using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;
using Pi.WalletService.Domain.SeedWork;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Domain.AggregatesModel.WalletAggregate;

public class TransactionHistory : BaseEntity, SagaStateMachineInstance
{
    public TransactionHistory(
        Guid id,
        Guid correlationId,
        string transactionNo,
        TransactionType transactionType,
        string userId,
        string accountCode,
        string customerCode,
        string globalAccount,
        Channel channel,
        Product product,
        Purpose purpose,
        string state,
        decimal requestedAmount,
        decimal? bankFee,
        DateTime? transactionDateTime,
        decimal? transactionAmount,
        string? customerName,
        string? bankAccountName,
        string? bankName,
        string? bankCode,
        string? bankAccountNo,
        string? failedReason,
        Guid? requesterDeviceId
    )
    {
        Id = id;
        CorrelationId = correlationId;
        TransactionNo = transactionNo;
        TransactionType = transactionType;
        UserId = userId;
        AccountCode = accountCode;
        CustomerCode = customerCode;
        GlobalAccount = globalAccount;
        Channel = channel;
        Product = product;
        Purpose = purpose;
        State = state;
        RequestedAmount = requestedAmount;
        BankFee = bankFee;
        TransactionDateTime = transactionDateTime;
        TransactionAmount = transactionAmount;
        CustomerName = customerName;
        BankAccountName = bankAccountName;
        BankName = bankName;
        BankCode = bankCode;
        BankAccountNo = bankAccountNo;
        FailedReason = failedReason;
        RequesterDeviceId = requesterDeviceId;
    }

    public Guid Id { get; set; }
    public Guid CorrelationId { get; set; }
    public string? TransactionNo { get; set; }
    public TransactionType TransactionType { get; set; }
    public string UserId { get; set; }
    public string AccountCode { get; set; }
    public string CustomerCode { get; set; }
    public string? GlobalAccount { get; set; }
    public Channel Channel { get; set; }
    public Product Product { get; set; }
    public Purpose Purpose { get; set; }
    public string State { get; set; }
    public decimal RequestedAmount { get; set; }
    public decimal? BankFee { get; set; }
    public DateTime? TransactionDateTime { get; set; }
    public decimal? TransactionAmount { get; set; }
    public string? CustomerName { get; set; }
    public string? BankAccountName { get; set; }
    public string? BankName { get; set; }
    public string? BankCode { get; set; }
    public string? BankAccountNo { get; set; }
    public string? FailedReason { get; set; }
    public new DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid? RequesterDeviceId { get; private set; }
}
