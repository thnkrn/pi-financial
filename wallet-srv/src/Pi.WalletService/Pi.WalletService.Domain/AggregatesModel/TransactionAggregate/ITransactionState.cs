using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;

public interface ITransactionState
{
    public string? TransactionNo { get; set; }
    public string UserId { get; set; }
    public string AccountCode { get; set; }
    public string CustomerCode { get; set; }
    public Channel Channel { get; set; }
    public Product Product { get; set; }
    public string? CurrentState { get; set; }
    public string? BankCode { get; set; }
    public string? BankAccountNo { get; set; }
    public DateTime CreatedAt { get; set; }
}
