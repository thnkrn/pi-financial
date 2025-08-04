using Pi.Common.Http;
using Pi.WalletService.Application.Models;
using Pi.WalletService.Application.Queries;
using Pi.WalletService.Domain.AggregatesModel.AtsDepositAggregate;
using Pi.WalletService.Domain.AggregatesModel.AtsWithdrawAggregate;
using Pi.WalletService.Domain.AggregatesModel.DepositEntrypointAggregate;
using Pi.WalletService.Domain.AggregatesModel.GlobalTransfer;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletManualAllocationAggregate;
using Pi.WalletService.Domain.AggregatesModel.OddDepositAggregate;
using Pi.WalletService.Domain.AggregatesModel.OddWithdrawAggregate;
using Pi.WalletService.Domain.AggregatesModel.QrDepositAggregate;
using Pi.WalletService.Domain.AggregatesModel.RecoveryAggregate;
using Pi.WalletService.Domain.AggregatesModel.RefundInfoAggregate;
using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;
using Pi.WalletService.Domain.AggregatesModel.UpBackAggregate;
using Pi.WalletService.Domain.AggregatesModel.WithdrawEntrypointAggregate;
using Pi.WalletService.IntegrationEvents.AggregatesModel;
using Transaction = Pi.WalletService.Domain.AggregatesModel.TransactionAggregate.Transaction;

namespace Pi.WalletService.API.Models;

public class TransactionDto
{
    public Guid Id { get; set; }
    public string AccountNo { get; set; } = null!;
    public string? TransactionNo { get; set; }
    public Product Product { get; set; }
    public TransactionType TransactionType { get; set; }
    public decimal RequestedAmount { get; set; }
    public Currency RequestedCurrency { get; set; }
    public string? QrValue { get; set; }
    public string? Status { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? RequestExpiredAt { get; set; }
    public decimal? RequestedFxRate { get; set; }
    public Currency? RequestedFxCurrency { get; set; }
    public decimal? TransferAmount { get; set; }
    public decimal? ExchangeRate { get; set; }
    public decimal? Fee { get; set; }
    public decimal? TransferFee { get; set; }
}

public class TransactionPaginate : PaginateQuery
{
    public Channel? Channel { get; set; }
    public Product? Product { get; set; }
    public string? State { get; set; }
    public string? BankCode { get; set; }
    public string? BankAccountNo { get; set; }
    public string? CustomerCode { get; set; }
    public string? AccountCode { get; set; }
    public string? TransactionNo { get; set; }
    public TransactionStatus? Status { get; set; }
    public ProductType? ProductType { get; set; }
    public DateTime? EffectiveDateFrom { get; set; }
    public DateTime? EffectiveDateTo { get; set; }
    public DateTime? CreatedAtFrom { get; set; }
    public DateTime? CreatedAtTo { get; set; }
}

public class RefundTransactionPaginate : TransactionPaginate
{
    public string? DepositTransactionNo { get; set; }
}

public class DepositTransactionPaginate : TransactionPaginate
{
    public string? BankName { get; set; }
    public DateTime? PaymentReceivedFrom { get; set; }
    public DateTime? PaymentReceivedTo { get; set; }
}

public class TransactionSummaryRequest
{
    public Product Product { get; set; }
    public DateTime CreatedAtFrom { get; set; }
    public DateTime CreatedAtTo { get; set; }
}

public class TransactionSummaryResponse<T>
{
    public TransactionSummary TransactionSummary { get; set; } = null!;
    public List<T> Transactions { get; set; } = null!;
}

public record TransactionDetailsDto
{
    public TransactionDetailsDto(Transaction transaction)
    {
        CorrelationId = transaction.CorrelationId;
        CurrentState = transaction.GetState();
        TransactionNo = transaction.TransactionNo;
        UserId = transaction.UserId;
        AccountCode = transaction.AccountCode;
        CustomerCode = transaction.CustomerCode;
        Channel = transaction.Channel;
        Product = transaction.Product;
        Purpose = transaction.Purpose;
        RequestedAmount = transaction.RequestedAmount.ToString("0.00");
        NetAmount = transaction.NetAmount?.ToString("0.00");
        CustomerName = transaction.CustomerName;
        BankAccountName = transaction.BankAccountName;
        BankAccountNo = transaction.BankAccountNo;
        BankName = transaction.BankName;
        BankCode = transaction.BankCode;
        FailedReason = transaction.FailedReason;
        TransactionType = transaction.TransactionType;
        CreatedAt = transaction.CreatedAt.ToUniversalTime();
        UpdatedAt = transaction.UpdatedAt.ToUniversalTime();
        DepositEntrypoint = transaction.DepositEntrypoint;
        WithdrawEntrypoint = transaction.WithdrawEntrypoint;
        QrDeposit = transaction.QrDeposit;
        OddDeposit = transaction.OddDeposit;
        AtsDeposit = transaction.AtsDeposit;
        OddWithdraw = transaction.OddWithdraw;
        AtsWithdraw = transaction.AtsWithdraw;
        UpBack = transaction.UpBack;
        GlobalTransfer = transaction.GlobalTransfer;
        Recovery = transaction.Recovery;
        RefundInfo = transaction.RefundInfo;
        GlobalManualAllocationInfo = transaction.GlobalManualAllocate;
        Status = transaction.Status;
        RequestedCurrency = transaction.GetRequestedCurrency();
        ToCurrency = transaction.GetRequestedFxCurrency();
        Fee = transaction.GetFee()?.ToString("0.00") ?? "0.00";
        PaymentAt = transaction.GetEffectiveDateTime().ToUniversalTime();
        TransferAmount = transaction.GetTransferAmount()?.ToString("0.00");
        PaymentReceivedAmount = transaction.GetPaymentReceivedAmount()?.ToString("0.00");
        PaymentDisbursedAmount = transaction.GetPaymentDisbursedAmount()?.ToString("0.00");
        ConfirmedAmount = transaction.GetConfirmedAmount()?.ToString("0.00");
    }

    public Guid CorrelationId { get; init; }
    public string? CurrentState { get; init; }
    public string? TransactionNo { get; init; }
    public string? UserId { get; init; }
    public string? AccountCode { get; init; }
    public string? CustomerCode { get; init; }
    public Channel? Channel { get; init; }
    public Product? Product { get; init; }
    public Purpose? Purpose { get; init; }
    public string? RequestedAmount { get; init; }
    public Currency? RequestedCurrency { get; init; }
    public Currency? ToCurrency { get; init; }
    public string? ConfirmedAmount { get; init; }
    public string? PaymentReceivedAmount { get; init; }
    public string? PaymentDisbursedAmount { get; init; }
    public string? TransferAmount { get; init; }
    public string? Fee { get; init; }
    public string? NetAmount { get; init; }
    public string? CustomerName { get; init; }
    public string? BankAccountName { get; init; }
    public string? BankAccountNo { get; init; }
    public string? BankName { get; init; }
    public string? BankCode { get; init; }
    public string? FailedReason { get; init; }
    public Status? Status { get; init; }
    public TransactionType TransactionType { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime PaymentAt { get; set; }
    public DateTime UpdatedAt { get; init; }
    public DepositEntrypointState? DepositEntrypoint { get; init; }
    public WithdrawEntrypointState? WithdrawEntrypoint { get; init; }
    public QrDepositState? QrDeposit { get; init; }
    public OddDepositState? OddDeposit { get; init; }
    public AtsDepositState? AtsDeposit { get; init; }
    public OddWithdrawState? OddWithdraw { get; init; }
    public AtsWithdrawState? AtsWithdraw { get; init; }
    public UpBackState? UpBack { get; init; }
    public GlobalTransferState? GlobalTransfer { get; init; }
    public RecoveryState? Recovery { get; init; }
    public RefundInfo? RefundInfo { get; init; }
    public GlobalManualAllocationState? GlobalManualAllocationInfo { get; init; }
}
