using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.IntegrationEvents;

public record WithdrawOtpValidationSuccess(
    Guid? RequestId,
    string RequestRef,
    string UserId,
    string TransactionNo,
    Product Product,
    DateTime? PaymentDisbursedDateTime,
    decimal? PaymentDisbursedAmount,
    string CustomerCode,
    string? AccountCode,
    string? BankName
);