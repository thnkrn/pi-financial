namespace Pi.WalletService.Domain.Events.ATS;

public record AtsWithdrawRequest(Guid CorrelationId);

public record AtsWithdrawRequestSuccess(
    string ReferenceId,
    string TransactionId,
    string ResultCode,
    string Reason,
    string SendDate,
    string SendTime,
    decimal Fee
);