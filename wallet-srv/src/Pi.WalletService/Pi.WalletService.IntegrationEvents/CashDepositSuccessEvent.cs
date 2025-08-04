namespace Pi.WalletService.IntegrationEvents;

public record CashDepositSuccessEvent(
    Guid TicketId,
    string UserId,
    string TransactionNo,
    DateTime PaymentReceivedDateTime,
    string Product,
    decimal Amount
);