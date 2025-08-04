namespace Pi.WalletService.Application.Services.Bank;

public record QRPaymentResponse(
    string? ExternalStatusCode,
    string? ExternalStatusDescription,
    string InternalStatusCode,
    string InternalStatusDescription,
    bool Status,
    string QRValue);
