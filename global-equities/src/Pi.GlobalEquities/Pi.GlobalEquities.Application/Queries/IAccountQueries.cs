using Pi.Common.CommonModels;
using Pi.GlobalEquities.Application.Models.Dto;
using Pi.GlobalEquities.DomainModels;
using Pi.GlobalEquities.Models;

namespace Pi.GlobalEquities.Application.Queries;

public interface IAccountQueries
{
    Task<AccountSummaryDto> GetAccountSummary(string userId, string accountId, IEnumerable<Currency> currencies,
        CancellationToken ct);
    Task<IAccount?> GetAccountByAccountId(string userId, string accountId, CancellationToken ct);
    Task<IAccount?> GetAccountByProviderAccount(string userId, Provider provider, string providerAccountId, CancellationToken ct);
    Task<IAccountBalance?> GetAccountBalanceByAccountId(string userId, string accountId, Currency currency, CancellationToken ct);
    Task<IAccountBalance?> GetAccountBalanceByProviderAccount(string userId, Provider provider, string providerAccount,
        Currency currency, CancellationToken ct);
}
