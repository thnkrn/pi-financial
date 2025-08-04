namespace Pi.WalletService.Domain.Events.Deposit;

public record DepositRetryFxTransfer(
    string TransactionNo
);