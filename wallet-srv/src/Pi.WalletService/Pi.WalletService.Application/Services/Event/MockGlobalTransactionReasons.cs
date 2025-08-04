namespace Pi.WalletService.Application.Models;

public enum MockGlobalTransactionReasons
{
    FxRateCompareFailed,
    FxTransferFailed,
    TransferRequestFailed,
    FxTransferInsufficientBalance,
    FxFailed,
    RevertTransferFailed
}