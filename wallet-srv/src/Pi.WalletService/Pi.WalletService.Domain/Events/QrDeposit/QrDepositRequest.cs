namespace Pi.WalletService.Domain.Events.QrDeposit;

public record QrDepositRequest(
    Guid CorrelationId,
    string TransactionNo
);
