using Pi.WalletService.API.Models;
using Pi.WalletService.Application.Queries;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.API.Factories;

public static class QueryFactory
{
    public static TransactionFilters NewTransactionFilters(TransactionPaginate paginate, TransactionType? transactionType = null)
    {
        return new TransactionFilters(
            paginate.Channel,
            paginate.Product,
            paginate.State,
            paginate.BankCode,
            paginate.BankAccountNo,
            paginate.CustomerCode,
            paginate.AccountCode,
            paginate.TransactionNo,
            paginate.Status,
            paginate.EffectiveDateFrom,
            paginate.EffectiveDateTo,
            paginate.CreatedAtFrom,
            paginate.CreatedAtTo,
            transactionType,
            null,
            null,
            paginate.ProductType
        );
    }
    public static TransactionFilters NewTransactionFilters(DepositTransactionPaginate paginate, TransactionType? transactionType = null)
    {
        return new TransactionFilters(
            paginate.Channel,
            paginate.Product,
            paginate.State,
            paginate.BankCode,
            paginate.BankAccountNo,
            paginate.CustomerCode,
            paginate.AccountCode,
            paginate.TransactionNo,
            paginate.Status,
            paginate.EffectiveDateFrom,
            paginate.EffectiveDateTo,
            paginate.CreatedAtFrom,
            paginate.CreatedAtTo,
            transactionType,
            null,
            null,
            paginate.ProductType,
            paginate.PaymentReceivedFrom,
            paginate.PaymentReceivedTo,
            paginate.BankName
        );
    }
    public static TransactionFilters NewTransactionFilters(RefundTransactionPaginate paginate, TransactionType? transactionType = null)
    {
        return new TransactionFilters(
            paginate.Channel,
            paginate.Product,
            paginate.State,
            paginate.BankCode,
            paginate.BankAccountNo,
            paginate.CustomerCode,
            paginate.AccountCode,
            paginate.TransactionNo,
            paginate.Status,
            paginate.EffectiveDateFrom,
            paginate.EffectiveDateTo,
            paginate.CreatedAtFrom,
            paginate.CreatedAtTo,
            transactionType,
            null,
            null,
            paginate.ProductType,
            null,
            null,
            null,
            paginate.DepositTransactionNo
        );
    }
}
