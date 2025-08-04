namespace Pi.BackofficeService.Domain.AggregateModels.TransactionAggregate;

public enum Channel
{
    QR,
    ODD,
    AtsBatch,
    BillPayment,
    SetTrade,
    OnlineTransfer,
    EForm,
    TransferApp,
    // For Sirius as they might not sent one
    Unknown,
}