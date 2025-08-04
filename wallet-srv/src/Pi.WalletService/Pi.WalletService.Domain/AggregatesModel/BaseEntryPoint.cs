using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;
using Pi.WalletService.Domain.SeedWork;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Domain.AggregatesModel;

public class BaseEntryPoint : BaseEntity
{
    private string? _bankAccountNo;
    public Guid CorrelationId { get; set; }
    public string? CurrentState { get; set; }
    public string? TransactionNo { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string AccountCode { get; set; } = string.Empty;
    public string CustomerCode { get; set; } = string.Empty;
    public Channel Channel { get; set; }
    public Product Product { get; set; }
    public Purpose Purpose { get; set; }
    public decimal RequestedAmount { get; set; }
    public decimal ConfirmedAmount { get; set; } // NonGE = RequestedAmount, GE = Amount in THB
    public decimal? NetAmount { get; set; } // ConfirmedAmount - Fee
    public string? CustomerName { get; set; }
    public string? BankAccountName { get; set; }
    public string? BankAccountTaxId { get; set; }
    public string? BankAccountNo
    {
        get
        {
            return _bankAccountNo;
        }
        set
        {
            _bankAccountNo = value?.Trim(Convert.ToChar(" ")).Replace(" ", "").Replace("-", "");
        }
    }
    public string? BankName { get; set; }
    public string? BankCode { get; set; }
    public string? BankBranchCode { get; set; }
    public string FailedReason { get; set; } = string.Empty;
    public string ResponseAddress { get; set; } = string.Empty;
    public Guid? RequestId { get; set; }
    public Guid? RequesterDeviceId { get; set; }
    public DateTime EffectiveDate { get; set; }
}