using Pi.GlobalEquities.DomainModels;
using Pi.GlobalEquities.Models;

namespace Pi.GlobalEquities.Application.Repositories;

public interface IAccountRepository
{
    Task<IAccount?> GetAccount(string userId, string accountId, CancellationToken ct);
    Task<IEnumerable<IAccount>> GetAccounts(string userId, CancellationToken ct);

    Task<IAccount?> GetAccountByProviderAccount(string userId, Provider provider, string providerAccountId, CancellationToken ct);
    Task UpsertAccounts(IEnumerable<IAccount> accounts, CancellationToken ct);
}
