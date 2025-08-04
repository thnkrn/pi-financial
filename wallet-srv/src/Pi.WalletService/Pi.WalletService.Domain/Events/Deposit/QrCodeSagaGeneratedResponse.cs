namespace Pi.WalletService.Domain.Events.Deposit;

public record QrCodeSagaGeneratedResponse(string TransactionNo, string QrValue, string QrTransactionRef, string QrTransactionNo, DateTime QrGenerateDateTime);