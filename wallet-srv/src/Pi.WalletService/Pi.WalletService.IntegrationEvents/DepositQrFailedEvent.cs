using Pi.WalletService.IntegrationEvents.AggregatesModel;
namespace Pi.WalletService.IntegrationEvents;

public record DepositQrFailedEvent(
    Guid CorrelationId,
    string FailedReason,
    string? BankName,
    string? BankAccountNo,
    string? BankAccountName,
    decimal Amount
);
