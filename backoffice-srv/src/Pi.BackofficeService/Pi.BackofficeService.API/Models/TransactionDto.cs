using Pi.BackofficeService.API.Factories;
using Pi.BackofficeService.Application.Models;
using Pi.BackofficeService.Application.Queries;
using Pi.BackofficeService.Domain.AggregateModels.TransactionAggregate;
using DepositChannel = Pi.BackofficeService.Application.Models.DepositChannel;
using DepositTransaction = Pi.BackofficeService.Application.Models.DepositTransaction;
using Product = Pi.BackofficeService.Domain.AggregateModels.TransactionAggregate.Product;
using ProductDto = Pi.BackofficeService.Application.Models.Product;
using Transaction = Pi.BackofficeService.Application.Models.Transaction;
using WithdrawChannel = Pi.BackofficeService.Application.Models.WithdrawChannel;
using WithdrawTransaction = Pi.BackofficeService.Application.Models.WithdrawTransaction;

namespace Pi.BackofficeService.API.Models;

public record TransactionFilterRequest(
    Channel? Channel,
    TransactionType? TransactionType,
    ProductType? ProductType,
    Product? AccountType,
    Guid? ResponseCodeId,
    string? BankCode,
    string? AccountNumber,
    string? CustomerCode,
    string? AccountCode,
    string? TransactionNumber,
    string? Status,
    DateOnly? EffectiveDateFrom,
    DateOnly? EffectiveDateTo,
    DateOnly? PaymentReceivedDateFrom,
    DateOnly? PaymentReceivedDateTo,
    DateOnly? CreatedAtFrom,
    DateOnly? CreatedAtTo);

public record TransactionPaginateRequest(
    string? OrderBy,
    string? OrderDir,
    TransactionFilterRequest? Filters,
    int? Page,
    int? PageSize);

public record TransactionResponseBase
{
    public TransactionResponseBase(Transaction transaction, NameAliasResponse? channel)
    {
        Id = transaction.Id;
        AccountNo = transaction.AccountNo;
        TransactionNo = transaction.TransactionNo;
        Amount = transaction.Amount;
        Currency = transaction.Currency.ToString();
        Status = transaction.Status;
        CustomerCode = transaction.CustomerCode;
        Channel = channel;
        AccountType = DtoFactory.NewNameAliasResponse(transaction.Product);
        TransactionType = transaction.TransactionType;
        BankAccountNo = transaction.BankAccountNo;
        BankName = transaction.BankName;
        UserId = transaction.UserId;
        FailedReason = transaction.FailedReason;
        EffectiveDate = transaction.EffectiveDate;
        CreatedAt = transaction.CreatedAt;
    }

    public Guid Id { get; init; }
    public string AccountNo { get; init; }
    public string TransactionNo { get; init; }
    public decimal? Amount { get; init; }
    public string? Currency { get; init; }
    public string Status { get; init; }
    public string CustomerCode { get; init; }
    public NameAliasResponse? Channel { get; init; }
    public NameAliasResponse AccountType { get; init; }
    public TransactionType TransactionType { get; init; }
    public string? BankAccountNo { get; init; }
    public string? BankName { get; init; }
    public string UserId { get; init; }
    public string? FailedReason { get; init; }
    public DateTime? EffectiveDate { get; init; }
    public DateTime CreatedAt { get; init; }
}

public record WithdrawTransactionBase : TransactionResponseBase
{
    public WithdrawTransactionBase(WithdrawTransaction transaction) : base(transaction,
        transaction.Channel != null ? DtoFactory.NewNameAliasResponse((WithdrawChannel)transaction.Channel) : null)
    {
        PaymentDisbursedDateTime = transaction.PaymentDisbursedDateTime;
        PaymentDisbursedAmount = transaction.PaymentDisbursedAmount;
        CustomerName = transaction.CustomerName;
    }

    public DateTime? PaymentDisbursedDateTime { get; init; }
    public decimal? PaymentDisbursedAmount { get; init; }
    public string? CustomerName { get; init; }
}

public record WithdrawTransactionResponse : WithdrawTransactionBase
{
    public WithdrawTransactionResponse(TransactionResult<WithdrawTransaction> transactionResult) : base(
        transactionResult.Transaction)
    {
        EffectiveDate = transactionResult.Transaction.PaymentDisbursedDateTime;
        ResponseCode = transactionResult.ResponseCode != null
            ? DtoFactory.NewResponseCodeResponse(transactionResult.ResponseCode)
            : null;
    }

    public ResponseCodeResponse? ResponseCode { get; init; }
}

public record WithdrawTransactionDetailResponse : WithdrawTransactionBase
{
    public WithdrawTransactionDetailResponse(TransactionDetailResult<WithdrawTransaction> transactionResult) : base(
        transactionResult.Transaction)
    {
        ResponseCode = transactionResult.ResponseCodeDetail != null
            ? DtoFactory.NewResponseCodeDetailResponse(transactionResult.ResponseCodeDetail)
            : null;
        GlobalTransfer = transactionResult.Transaction.GlobalTransfer;
    }

    public ResponseCodeDetailResponse? ResponseCode { get; init; }
    public GlobalTransfer? GlobalTransfer { get; init; }
}

public record DepositTransactionBase : TransactionResponseBase
{
    public DepositTransactionBase(DepositTransaction transaction) : base(transaction,
        transaction.Channel != null ? DtoFactory.NewNameAliasResponse((DepositChannel)transaction.Channel) : null)
    {
        RequestedAmount = transaction.RequestedAmount;
        ReceivedAmount = transaction.ReceivedAmount;
        PaymentReceivedDateTime = transaction.PaymentReceivedDateTime;
        CustomerName = transaction.CustomerName;
        BankAccountName = transaction.BankAccountName;
        EffectiveDate = transaction.EffectiveDate;
    }

    public decimal RequestedAmount { get; init; }
    public decimal? ReceivedAmount { get; init; }
    public DateTime? PaymentReceivedDateTime { get; init; }
    public string CustomerName { get; init; }
    public string? BankAccountName { get; init; }
}

public record DepositTransactionResponse : DepositTransactionBase
{
    public DepositTransactionResponse(TransactionResult<DepositTransaction> transactionResult) : base(transactionResult
        .Transaction)
    {
        ResponseCode = transactionResult.ResponseCode != null
            ? DtoFactory.NewResponseCodeResponse(transactionResult.ResponseCode)
            : null;
    }

    public ResponseCodeResponse? ResponseCode { get; init; }
}

public record DepositTransactionDetailResponse : DepositTransactionBase
{
    public DepositTransactionDetailResponse(TransactionDetailResult<DepositTransaction> transactionResult) : base(
        transactionResult.Transaction)
    {
        ResponseCode = transactionResult.ResponseCodeDetail != null
            ? DtoFactory.NewResponseCodeDetailResponse(transactionResult.ResponseCodeDetail)
            : null;
        GlobalTransfer = transactionResult.Transaction.GlobalTransfer;
        RefundedAt = transactionResult.Transaction.RefundAt;
    }

    public DateTime? RefundedAt { get; init; }
    public ResponseCodeDetailResponse? ResponseCode { get; init; }
    public GlobalTransfer? GlobalTransfer { get; init; }
}

public record TransactionV2Base
{
    public TransactionV2Base(TransactionV2 transaction)
    {
        State = transaction.State;
        Product = DtoFactory.NewNameAliasResponse((ProductDto)transaction.Product!);
        TransactionNo = transaction.TransactionNo;
        Status = transaction.Status;
        TransactionType = transaction.TransactionType;
        Channel = DtoFactory.NewNameAliasResponse((TransactionChannel)transaction.Channel!);
        AccountCode = transaction.AccountCode;
        GlobalAccount = transaction.GlobalAccount;
        CustomerCode = transaction.CustomerCode;
        CustomerName = transaction.CustomerName;
        BankAccountNo = transaction.BankAccountNo;
        BankAccountName = transaction.BankAccountName;
        BankName = transaction.BankName;
        RequestedAmount = transaction.RequestedAmount;
        RequestedCurrency = transaction.RequestedCurrency;
        ToCurrency = transaction.ToCurrency;
        PaymentReceivedAmount = transaction.PaymentReceivedAmount;
        PaymentDisbursedAmount = transaction.PaymentDisbursedAmount;
        ConfirmedAmount = transaction.ConfirmedAmount;
        TransferAmount = transaction.TransferAmount;
        Fee = transaction.Fee;
        TransferFee = transaction.TransferFee;
        FailedReason = transaction.FailedReason;
        EffectiveDateTime = transaction.EffectiveDate;
        CreatedAt = transaction.CreatedAt;
        QrDeposit = transaction.QrDeposit;
        OddDeposit = transaction.OddDeposit;
        AtsDeposit = transaction.AtsDeposit;
        OddWithdraw = transaction.OddWithdraw;
        AtsWithdraw = transaction.AtsWithdraw;
        GlobalTransfer = transaction.GlobalTransfer;
        UpBack = transaction.UpBack;
        Recovery = transaction.Recovery;
        RefundInfo = transaction.Refund;
        BillPayment = transaction.BillPayment;
    }

    public string? State { get; init; }
    public string? Status { get; init; }
    public string? TransactionNo { get; init; }
    public NameAliasResponse? Product { get; init; }
    public NameAliasResponse? Channel { get; init; }
    public string? AccountCode { get; init; }
    public string? CustomerCode { get; init; }
    public string? CustomerName { get; init; }
    public string? BankAccountNo { get; init; }
    public string? BankAccountName { get; init; }
    public string? BankName { get; init; }
    public string? GlobalAccount { get; init; }
    public TransactionType TransactionType { get; init; }
    public string? RequestedAmount { get; init; }
    public Currency RequestedCurrency { get; init; }
    public Currency? ToCurrency { get; init; }
    public string? PaymentReceivedAmount { get; init; }
    public string? PaymentDisbursedAmount { get; init; }
    public string? ConfirmedAmount { get; init; }
    public string? TransferAmount { get; init; }
    public string? Fee { get; init; }
    public string? TransferFee { get; init; }
    public string? FailedReason { get; init; }
    public DateOnly? EffectiveDateTime { get; init; }
    public DateTime? CreatedAt { get; init; }
    public QrDepositState? QrDeposit { get; init; }
    public OddDepositState? OddDeposit { get; init; }
    public AtsDepositState? AtsDeposit { get; init; }
    public OddWithdrawState? OddWithdraw { get; init; }
    public AtsWithdrawState? AtsWithdraw { get; init; }
    public GlobalTransferState? GlobalTransfer { get; init; }
    public UpBackState? UpBack { get; init; }
    public RecoveryState? Recovery { get; init; }
    public RefundInfo? RefundInfo { get; init; }
    public BillPaymentState? BillPayment { get; init; }
}

public record TransactionV2DetailResponse : TransactionV2Base
{
    public TransactionV2DetailResponse(TransactionDetailResult<TransactionV2> transactionResult) : base(transactionResult.Transaction)
    {
        ResponseCode = transactionResult.ResponseCodeDetail != null
            ? DtoFactory.NewResponseCodeDetailResponse(transactionResult.ResponseCodeDetail)
            : null;
    }
    public ResponseCodeDetailResponse? ResponseCode { get; init; }
}

public record TransactionHistoryV2Base
{
    public TransactionHistoryV2Base(TransactionHistoryV2 transactionHistory)
    {
        State = transactionHistory.State;
        Product = DtoFactory.NewNameAliasResponse((ProductDto)transactionHistory.Product!);
        AccountCode = transactionHistory.AccountCode;
        CustomerName = transactionHistory.CustomerName;
        BankAccountNo = transactionHistory.BankAccountNo;
        BankAccountName = transactionHistory.BankAccountName;
        BankName = transactionHistory.BankName;
        EffectiveDateTime = transactionHistory.PaymentDateTime;
        GlobalAccount = transactionHistory.GlobalAccount;
        TransactionNo = transactionHistory.TransactionNo;
        TransactionType = transactionHistory.TransactionType;
        RequestedAmount = transactionHistory.RequestedAmount;
        RequestedCurrency = transactionHistory.RequestedCurrency;
        Status = transactionHistory.Status;
        CreatedAt = transactionHistory.CreatedAt;
        ToCurrency = transactionHistory.ToCurrency;
        TransferAmount = transactionHistory.TransferAmount;
        Channel = DtoFactory.NewNameAliasResponse((TransactionChannel)transactionHistory.Channel!);
        BankAccount = transactionHistory.BankAccount;
        Fee = transactionHistory.Fee;
        TransferFee = transactionHistory.TransferFee;
    }

    public string? State { get; init; }
    public NameAliasResponse? Product { get; init; }
    public string? AccountCode { get; init; }
    public string? CustomerName { get; init; }
    public string? BankAccountNo { get; init; }
    public string? BankAccountName { get; init; }
    public string? BankName { get; init; }
    public DateTime? EffectiveDateTime { get; init; }
    public string? GlobalAccount { get; init; }
    public string? TransactionNo { get; init; }
    public TransactionType TransactionType { get; init; }
    public string? RequestedAmount { get; init; }
    public Currency RequestedCurrency { get; init; }
    public string? Status { get; init; }
    public DateTime? CreatedAt { get; init; }
    public Currency? ToCurrency { get; init; }
    public string? TransferAmount { get; init; }
    public NameAliasResponse? Channel { get; init; }
    public string? BankAccount { get; init; }
    public string? Fee { get; init; }
    public string? TransferFee { get; init; }
}

public record TransactionHistoryV2Response : TransactionHistoryV2Base
{
    public TransactionHistoryV2Response(TransactionResult<TransactionHistoryV2> transactionResult) : base(transactionResult.Transaction)
    {
        ResponseCode = transactionResult.ResponseCode != null
            ? DtoFactory.NewResponseCodeResponse(transactionResult.ResponseCode)
            : null;
    }
    public ResponseCodeResponse? ResponseCode { get; init; }
}
