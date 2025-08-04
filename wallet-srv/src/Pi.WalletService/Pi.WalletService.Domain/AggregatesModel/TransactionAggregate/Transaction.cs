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
using Pi.WalletService.Domain.AggregatesModel.UpBackAggregate;
using Pi.WalletService.Domain.AggregatesModel.WithdrawEntrypointAggregate;
using Pi.WalletService.Domain.Utilities;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;

public class DepositEntrypointTransaction
{
    public DepositEntrypointState? DepositEntrypoint { get; set; }
    public QrDepositState? QrDeposit { get; set; }
    public OddDepositState? OddDeposit { get; set; }
    public AtsDepositState? AtsDeposit { get; init; }
    public UpBackState? UpBack { get; set; }
    public GlobalTransferState? GlobalTransfer { get; set; }
    public RefundInfo? RefundInfo { get; set; }
}

public class WithdrawEntrypointTransaction
{
    public WithdrawEntrypointState? WithdrawEntrypoint { get; set; }
    public OddWithdrawState? OddWithdraw { get; set; }
    public AtsWithdrawState? AtsWithdraw { get; set; }
    public UpBackState? UpBack { get; set; }
    public GlobalTransferState? GlobalTransfer { get; set; }
    public RecoveryState? Recovery { get; set; }
}

public enum Status
{
    Success,
    Fail,
    Processing,
    Pending,
}

public class Transaction
{
    public Transaction(
        Guid correlationId, string? currentState, string? transactionNo, string userId,
        string accountCode, string customerCode, Channel channel, Product product, Purpose purpose,
        decimal requestedAmount, decimal? netAmount, string? customerName, string? bankAccountName,
        string? bankAccountNo, string? bankName, string? bankCode, string responseAddress,
        Guid? requestId, Guid? requesterDeviceId, TransactionType transactionType, DateTime createdAt,
        DateTime updatedAt)
    {
        CorrelationId = correlationId;
        CurrentState = currentState;
        TransactionNo = transactionNo;
        UserId = userId;
        AccountCode = accountCode;
        CustomerCode = customerCode;
        Channel = channel;
        Product = product;
        Purpose = purpose;
        RequestedAmount = requestedAmount;
        NetAmount = netAmount;
        CustomerName = customerName;
        BankAccountName = bankAccountName;
        BankAccountNo = bankAccountNo;
        BankName = bankName;
        BankCode = bankCode;
        ResponseAddress = responseAddress;
        RequestId = requestId;
        RequesterDeviceId = requesterDeviceId;
        TransactionType = transactionType;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public Guid CorrelationId { get; }
    private string? CurrentState { get; }
    public string? TransactionNo { get; }
    public string UserId { get; }
    public string AccountCode { get; }
    public string CustomerCode { get; }
    public Channel Channel { get; }
    public Product Product { get; }
    public Purpose Purpose { get; }
    public decimal RequestedAmount { get; }
    public decimal? NetAmount { get; }
    public string? CustomerName { get; }
    public string? BankAccountName { get; }
    public string? BankAccountNo { get; }
    public string? BankName { get; }
    public string? BankCode { get; }
    public string ResponseAddress { get; }
    public Guid? RequestId { get; }
    public Guid? RequesterDeviceId { get; }
    public TransactionType TransactionType { get; }
    public DateTime CreatedAt { get; }
    public DateTime UpdatedAt { get; }
    public DepositEntrypointState? DepositEntrypoint { get; set; }
    public WithdrawEntrypointState? WithdrawEntrypoint { get; set; }
    public QrDepositState? QrDeposit { get; set; }
    public OddDepositState? OddDeposit { get; set; }
    public AtsDepositState? AtsDeposit { get; set; }
    public OddWithdrawState? OddWithdraw { get; set; }
    public AtsWithdrawState? AtsWithdraw { get; set; }
    public UpBackState? UpBack { get; set; }
    public GlobalTransferState? GlobalTransfer { get; set; }
    public RecoveryState? Recovery { get; set; }
    public RefundInfo? RefundInfo { get; set; }
    public GlobalManualAllocationState? GlobalManualAllocate { get; set; }

    public Status Status
    {
        get
        {
            return GetState() switch
            {
                "DepositSucceed"
                    or "WithdrawSucceed"
                    or "Final"
                    or "CashDepositCompleted" // SetTrade E-Pay
                    => Status.Success,
                "DepositFailed"
                    or "WithdrawFailed"
                    or "RevertTransferSuccess"
                    or "RefundSuccess"
                    or "DepositPaymentNotReceived"
                    or "OtpValidationNotReceived"
                    or "CashDepositPaymentNotReceived" // SetTrade E-Pay
                    => Status.Fail,
                "DepositFailedNameMismatch"
                    or "DepositFailedInvalidSource"
                    or "DepositFailedAmountMismatch"
                    or "UpBackFailedRequireActionRevert"
                    or "FxFailed"
                    or "FxTransferFailed"
                    or "FxRateCompareFailed"
                    or "WaitingForAtsGatewayConfirmation"
                    or "DepositWaitingForGateway"
                    or "WithdrawWaitingForGateway"
                    or "RevertTransferFailed"
                    or "UpBackFailedRequireActionSba"
                    or "UpBackFailedRequireActionSetTrade"
                    or "WithdrawFailedRequireActionRecovery"
                    or "ManualAllocationFailed"
                    or "CashDepositWaitingForGateway" // SetTrade E-Pay
                    or "CashDepositFailed" // SetTrade E-Pay
                    or "TfexCashDepositFailed" // SetTrade E-Pay
                    => Status.Pending,
                _ => Status.Processing,
            };
        }
    }

    public string? GetState()
    {
        switch (CurrentState)
        {
            case "DepositProcessing":
                switch (Channel)
                {
                    case Channel.QR:
                        return QrDeposit?.CurrentState ?? CurrentState;
                    case Channel.ODD:
                        return OddDeposit?.CurrentState ?? CurrentState;
                    case Channel.ATS:
                        return AtsDeposit?.CurrentState ?? CurrentState;
                    case Channel.SetTrade:
                    case Channel.OnlineViaKKP:
                    case Channel.EForm:
                    case Channel.TransferApp:
                    case Channel.Unknown:
                        throw new NotImplementedException();
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            case "WithdrawValidating":
            case "WithdrawProcessing":
                switch (Channel)
                {
                    case Channel.OnlineViaKKP:
                        return OddWithdraw?.CurrentState ?? CurrentState;
                    case Channel.ATS:
                        return AtsWithdraw?.CurrentState ?? CurrentState;
                    case Channel.SetTrade:
                    case Channel.ODD:
                    case Channel.EForm:
                    case Channel.TransferApp:
                    case Channel.Unknown:
                        throw new NotImplementedException();
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            case "WithdrawFailedRequireActionRecovery" or "WithdrawFailed":
                // Check if we have any recovery state
                // - if not, return failed as is
                // - otherwise, return recovery status 
                return GlobalManualAllocate?.CurrentState == "Final" ? "WithdrawFailed" : Recovery?.CurrentState ?? CurrentState;
            case "DepositFailed":
                // Check if we have refund the transaction
                // - if not, return failed as is
                // - otherwise, return refund success status 
                return RefundInfo != null ? RefundInfo.CurrentState : CurrentState;
            case "UpBackProcessing":
                return UpBack?.CurrentState ?? CurrentState;
            case "GlobalTransferProcessing":
                return GlobalTransfer?.CurrentState ?? CurrentState;
            default:
                return CurrentState;
        }
    }

    public string? FailedReason
    {
        get
        {
            return GetState() switch
            {
                "DepositFailedNameMismatch" => FailedDescription.NameMismatch,
                "DepositFailedInvalidSource" => FailedDescription.IncorrectSource,
                "DepositFailedAmountMismatch" => FailedDescription.AmountMismatch,
                "UpBackFailedRequireActionRevert" => FailedDescription.RevertUpBackTransaction,
                "FxFailed" => FailedDescription.UnableToFx,
                "FxTransferFailed" or "RevertTransferFailed" => FailedDescription.ManualAllocationXnt,
                "FxRateCompareFailed" => FailedDescription.FxRateOver,
                "WaitingForAtsGatewayConfirmation" => FailedDescription.ApproveAts,
                "UpBackFailedRequireActionSba" => FailedDescription.ManualAllocationSba,
                "UpBackFailedRequireActionSetTrade" => FailedDescription.ManualAllocationSetTrade,
                "RefundSuccess" => FailedDescription.RefundSuccess,
                "WithdrawFailedRequireActionRecovery" or "RevertTransferFailed" => FailedDescription.RevertTransaction,
                _ => GetInternalFailedReason()
            };
        }
    }

    private string? GetInternalFailedReason()
    {
        return TransactionType switch
        {
            TransactionType.Deposit => DepositEntrypoint?.FailedReason
                                       ?? OddDeposit?.FailedReason
                                       ?? AtsDeposit?.FailedReason
                                       ?? QrDeposit?.FailedReason
                                       ?? UpBack?.FailedReason
                                       ?? GlobalTransfer?.FailedReason,
            TransactionType.Withdraw => WithdrawEntrypoint?.FailedReason
                                        ?? OddWithdraw?.FailedReason
                                        ?? AtsWithdraw?.FailedReason
                                        ?? UpBack?.FailedReason
                                        ?? GlobalTransfer?.FailedReason
                                        ?? Recovery?.FailedReason,
            _ => null
        };
    }

    /// <summary>
    /// Get Customer Received Amount
    /// GE Deposit -> Received Amount in USD
    /// GE Withdraw -> Received Amount in THB
    /// NetAmount for Non-Global 
    /// </summary>
    /// <returns></returns>
    public decimal? GetTransferAmount()
    {
        // todo: we should not re-calculate amount again, after v2 migrate to use confirmed amount 
        if (Product == Product.GlobalEquities && GlobalTransfer != null)
        {
            return TransactionType == TransactionType.Deposit
                ? GlobalTransfer?.TransferAmount ??
                  GlobalTransfer?.FxConfirmedAmount ??
                  RoundingUtils.RoundExchangeTransaction(
                      TransactionType,
                      Currency.THB,
                      RequestedAmount,
                      Currency.USD,
                      GlobalTransfer!.FxConfirmedExchangeRate ?? GlobalTransfer!.RequestedFxRate)
                : RoundingUtils.RoundExchangeTransaction(
                      TransactionType,
                      Currency.USD,
                      RequestedAmount,
                      Currency.THB,
                      GlobalTransfer!.RequestedFxRate,
                      false);
        }
        return TransactionType == TransactionType.Deposit
            ? RequestedAmount - RefundInfo?.Amount ?? RequestedAmount
            : NetAmount ?? RequestedAmount;
    }

    public decimal? GetFxConfirmedAmount()
    {
        return Product == Product.GlobalEquities && GlobalTransfer != null
            ? GlobalTransfer.FxConfirmedAmount : null;
    }

    public decimal? GetPaymentReceivedAmount()
    {
        return TransactionType == TransactionType.Deposit
            ? QrDeposit?.PaymentReceivedAmount
              ?? OddDeposit?.PaymentReceivedAmount
              ?? AtsDeposit?.PaymentReceivedAmount
            : null;
    }

    public decimal? GetPaymentDisbursedAmount()
    {
        return TransactionType == TransactionType.Withdraw
            ? OddWithdraw?.PaymentDisbursedAmount ?? AtsWithdraw?.PaymentDisbursedAmount
            : null;
    }

    public decimal? GetConfirmedAmount()
    {
        return TransactionType == TransactionType.Deposit
            ? QrDeposit?.PaymentReceivedAmount
              ?? OddDeposit?.PaymentReceivedAmount
              ?? AtsDeposit?.PaymentReceivedAmount
            : OddWithdraw?.PaymentDisbursedAmount ?? AtsWithdraw?.PaymentDisbursedAmount;
    }

    public Currency GetRequestedCurrency()
    {
        return Product == Product.GlobalEquities
            ? TransactionType == TransactionType.Deposit
                ? Currency.THB
                : GlobalTransfer?.RequestedCurrency ?? Currency.USD
            : Currency.THB;
    }

    public Currency? GetRequestedFxCurrency()
    {
        return Product == Product.GlobalEquities && GlobalTransfer != null
            ? TransactionType == TransactionType.Deposit
                ? Currency.USD
                : Currency.THB
            : null;
    }

    public string? GetBankAccount()
    {
        if (string.IsNullOrWhiteSpace(BankName) || string.IsNullOrWhiteSpace(BankAccountNo))
        {
            return null;
        }
        return $"{BankName} • {BankAccountNo?.Substring(BankAccountNo.Length - 4, 4)}";
    }

    public DateTime GetEffectiveDateTime()
    {
        var effectiveDateTime = TransactionType == TransactionType.Deposit
            ? DepositEntrypoint?.EffectiveDate ?? UpdatedAt
            : WithdrawEntrypoint?.EffectiveDate ?? UpdatedAt;

        return effectiveDateTime.ToUniversalTime();
    }

    public string? GetGlobalAccount()
    {
        return Product == Product.GlobalEquities
            ? GlobalTransfer?.GlobalAccount
            : null;
    }

    public decimal? GetFee()
    {
        return QrDeposit?.Fee ?? OddDeposit?.Fee ?? OddWithdraw?.Fee ?? AtsDeposit?.Fee ?? AtsWithdraw?.Fee;
    }


    public DateTime? GetQrExpiredTime()
    {
        return QrDeposit?.DepositQrGenerateDateTime?.AddMinutes(
            QrDeposit?.QrCodeExpiredTimeInMinute ?? 0).ToUniversalTime();
    }

    public decimal? GetExchangeRate()
    {
        if (Product != Product.GlobalEquities)
        {
            return null;
        }

        if (GlobalTransfer?.FxMarkUpRate > 0)
        {
            if (GlobalTransfer?.CurrentState != "FxRateCompareFailed")
            {
                return GlobalTransfer?.RequestedFxRate;
            }

            return FxSpreadUtils.CalculateMarkedUp(
                TransactionType,
                GlobalTransfer?.FxConfirmedExchangeRate ?? GlobalTransfer?.RequestedFxRate ?? 0,
                GlobalTransfer?.FxMarkUpRate ?? 0,
                true);
        }

        return GlobalTransfer?.FxConfirmedExchangeRate ??
               GlobalTransfer?.RequestedFxRate;
    }

    // RequestedFxRate always has 2 digits
    public decimal? GetRequestedFxRate()
    {
        if (Product != Product.GlobalEquities)
        {
            return null;
        }

        return RoundingUtils.RoundExchangeRate(GlobalTransfer!.TransactionType, GlobalTransfer!.RequestedFxRate, 2);
    }

    public string? GetWorkAroundState()
    {
        List<string> pendingAsFailedStates = new()
        {
            "UpBackFailedRequireActionRevert",
            "UpBackFailedRequireActionSba",
            "UpBackFailedRequireActionSetTrade",
            "WithdrawFailedRequireActionRecovery",
        };
        var state = GetState();
        if (state == null)
        {
            return state;
        }

        return TransactionType switch
        {
            TransactionType.Deposit when pendingAsFailedStates.Contains(state) => "DepositFailed",
            TransactionType.Withdraw when pendingAsFailedStates.Contains(state) => "WithdrawFailed",
            _ => state
        };
    }

    public string? GetNameTh()
    {
        return CustomerName?.Split(new[] { '(', ')' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault()?.Trim();
    }

    public string? GetNameEn()
    {
        return CustomerName?.Split(new[] { '(', ')' }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault()?.Trim();
    }
}