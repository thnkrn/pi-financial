namespace Pi.WalletService.Domain.Events.Withdraw;

public record UpdateTradingAccountBalanceSuccess(
    string ReferenceId,
    string TransactionId,
    string ResultCode,
    string Reason,
    string SendDate,
    string SendTime
);