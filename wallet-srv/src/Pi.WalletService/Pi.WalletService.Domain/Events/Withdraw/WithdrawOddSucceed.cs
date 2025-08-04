namespace Pi.WalletService.Domain.Events.Withdraw;

public record WithdrawOddSucceed(string TransactionNo, string TransactionRef, DateTime DateTime);