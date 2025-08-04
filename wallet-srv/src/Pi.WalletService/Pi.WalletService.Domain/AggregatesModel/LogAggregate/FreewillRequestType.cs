namespace Pi.WalletService.Domain.AggregatesModel.LogAggregate;

public enum FreewillRequestType
{
    QueryCustomerBankAccountInfo,
    QueryCustomerAccountInfo,
    QueryATS,
    DepositCash,
    WithdrawAnyPaytype,
    DepositATS,
    WithdrawATS,
    DepositCashCallback,
    WithdrawAnyPaytypeCallback,
    QueryWithdrawalAmount,
}