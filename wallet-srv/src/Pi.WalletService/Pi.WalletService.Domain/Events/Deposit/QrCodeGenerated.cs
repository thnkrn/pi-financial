namespace Pi.WalletService.Domain.Events.Deposit;

public record QrCodeGenerated(string QrValue, string QrTransactionRef, string QrTransactionNo, DateTime QrGenerateDateTime);
public record QrCodeGeneratedV2(string TransactionNo, string QrValue, string QrTransactionRef, string QrTransactionNo, DateTime QrGenerateDateTime);
