using Pi.Common.Http;
using Pi.WalletService.Application.Queries;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.API.Models;

public class TransactionHistoryPaginate : PaginateQuery
{
    public TransactionStatus? Status { get; set; }
    public string? State { get; set; }
    public TransactionType? TransactionType { get; set; }
    public DateTime? CreatedAtFrom { get; set; }
    public DateTime? CreatedAtTo { get; set; }
    public string? AccountCode { get; set; }
    public string? CustCode { get; set; }

    // Sirius Specific ID to query Transaction History
    public string? AccountId { get; set; }
}

public class TransactionHistoryV2Paginate : PaginateQuery
{
    public string? TransactionNo { get; set; }
    public Product[]? Products { get; set; }
    public Status? Status { get; set; }
    public string? State { get; set; }
    public TransactionType? TransactionType { get; set; }
    public Channel? Channel { get; set; }
    public DateTime? CreatedAtFrom { get; set; }
    public DateTime? CreatedAtTo { get; set; }
    public DateTime? EffectiveDateFrom { get; set; }
    public DateTime? EffectiveDateTo { get; set; }
    public DateTime? PaymentReceivedFrom { get; set; }
    public DateTime? PaymentReceivedTo { get; set; }
    public string? CustomerCode { get; set; }
    public string? AccountCode { get; set; }
    public string? BankAccountNo { get; set; }
    public string? BankCode { get; set; }
    public string? BankName { get; set; }
    public string[]? NotStates { get; set; }

    // Sirius Specific ID to query Transaction History
    public string? AccountId { get; set; }
}

public class TransactionHistory
{
    public string? TransactionNo { get; set; }
    public TransactionType TransactionType { get; set; }
    public string? RequestedAmount { get; set; }
    public Currency RequestedCurrency { get; set; }
    public string? Status { get; set; }
    public DateTime? CreatedAt { get; set; }
    public Currency? ToCurrency { get; set; }
    public string? TransferAmount { get; set; }
    public Channel? Channel { get; set; }
    public string? BankAccount { get; set; }
    public string? Fee { get; set; }
    public string? TransferFee { get; set; }
}

public class TransactionHistoryV2 : TransactionHistory
{
    public string? State { get; set; }
    public Product? Product { get; set; }
    public string? AccountCode { get; set; }
    public string? CustomerName { get; set; }
    public string? BankAccountNo { get; set; }
    public string? BankAccountName { get; set; }
    public string? BankName { get; set; }
    public DateTime? EffectiveDateTime { get; set; }
    public string? GlobalAccount { get; set; }
    public string? FailedReason { get; set; }

}