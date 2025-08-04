namespace Pi.WalletService.Application.Services.Bank;

public record DDPaymentResponse(
    string? ExternalStatusCode,
    string? ExternalStatusDescription,
    string InternalStatusCode,
    string InternalStatusDescription,
    bool Status,
    string TransactionNo,
    string TransactionRefCode,
    decimal Amount,
    string ExternalRefTime,
    string ExternalRefCode);
