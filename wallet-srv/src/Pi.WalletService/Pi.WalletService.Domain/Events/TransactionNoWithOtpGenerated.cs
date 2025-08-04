namespace Pi.WalletService.Domain.Events;

public record TransactionNoWithOtpGenerated(string TransactionNo, string OtpRef);