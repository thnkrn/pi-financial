namespace Pi.BackofficeService.Domain.AggregateModels.TicketAggregate;

public enum Method
{
    Approve,
    Refund,
    CcyAllocationTransfer,
    RetrySbaDeposit,
    RetrySbaWithdraw,
    RetrySetTradeDeposit,
    RetrySetTradeWithdraw,
    RetryKkpWithdraw,
    SbaConfirm,
    SbaDepositAtsCallbackConfirm,
    DepositKkpConfirm,
    ChangeStatusToPending,
    ChangeStatusToSuccess,
    ChangeStatusToFail,
    ChangeSetTradeStatusToPending,
    ChangeSetTradeStatusToSuccess,
    ChangeSetTradeStatusToFail,
    UpdateBillPaymentReference,
}
