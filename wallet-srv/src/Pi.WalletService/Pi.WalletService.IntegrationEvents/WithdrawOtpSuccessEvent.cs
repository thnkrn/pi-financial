using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.IntegrationEvents;

public record WithdrawOtpSuccessEvent(Product Product, string TransactionNo);