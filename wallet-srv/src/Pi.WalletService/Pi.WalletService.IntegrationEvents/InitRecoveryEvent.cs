using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.IntegrationEvents;

public record InitRecoveryEvent(Guid CorrelationId, TransactionType TransactionType, Product Product);
