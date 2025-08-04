namespace Pi.WalletService.Domain.Events.ForeignExchange;

public record FxConfirmed(string TransactionId, DateTime ConfirmedTime);