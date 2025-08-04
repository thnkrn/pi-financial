namespace Pi.WalletService.Domain.Events.GlobalTransfer;

public record GlobalTransferSuccess(
    Guid CorrelationId,
    decimal? ConfirmedAmount
);
