using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Domain.Events.GlobalTransfer;

public record GlobalTransferRequest(Guid CorrelationId);
