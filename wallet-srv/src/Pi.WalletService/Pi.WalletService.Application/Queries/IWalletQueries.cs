using Pi.WalletService.Application.Services.UserService;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Application.Queries;

public record ExchangeRate(
    decimal Rate,
    decimal CounterAmount,
    string Currency,
    DateTime ExpiredAt,
    string TransactionId);

public record BankAccountInfo(
    string AccountNo,
    string Name,
    string ShortName,
    string Code,
    string IconUrl,
    string BranchCode
);

public record AccountLimitValue(
    decimal AccountLimit,
    decimal UnusedCash,
    decimal DepositLimit);

public interface IWalletQueries
{
    Task VerifyUserGeBalance(string userId, string custCode, string currency, decimal requestWithdrawAmount);
    [Obsolete("This can produces incorrect result, Use another overload instead")]
    Task<BankAccountInfo> GetBankAccount(string userId, string customerCode);
    Task<BankAccountInfo> GetBankAccount(string userId, string customerCode, Product product, TransactionType transactionType, User? user = null);
    Task<AccountLimitValue> GetGeDepositLimit(string userId, string custCode, string currency);
    Task<bool> GetAtsRegistrationStatus(string userId);
    Task<decimal> GetAvailableWithdrawalAmount(string userId, string custCode, Product product);
    Task<bool> CheckAtsAvailable(string userId, string customerCode, Product product);
}