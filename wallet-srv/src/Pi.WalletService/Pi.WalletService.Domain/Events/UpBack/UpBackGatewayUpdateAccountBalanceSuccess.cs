namespace Pi.WalletService.Domain.Events.UpBack;

public record GatewayUpdateAccountBalanceSuccessEvent(
    string ReferenceId,
    string TransactionId,
    string ResultCode,
    string Reason,
    string SendDate,
    string SendTime
);
