namespace Pi.WalletService.Domain.Events.ATS;

public record AtsDepositRequest(Guid CorrelationId);

public record AtsDepositRequestSuccess(
    string ReferenceId,
    string TransactionId,
    string ResultCode,
    string Reason,
    string SendDate,
    string SendTime,
    decimal Fee
);