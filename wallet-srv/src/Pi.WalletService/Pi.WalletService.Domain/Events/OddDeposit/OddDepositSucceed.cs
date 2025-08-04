namespace Pi.WalletService.Domain.Events.OddDeposit;

public record OddDepositSucceed(
    Guid CorrelationId,
    string UserId,
    string RefCode,
    decimal PaymentReceivedAmount,
    DateTime OddProcessedDateTime,
    decimal Fee);
