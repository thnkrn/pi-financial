using Pi.GlobalEquities.DomainModels;
using Pi.GlobalEquities.Models;

namespace Pi.GlobalEquities.Repositories;

public interface IWorkerAccountRepository : IAccountRepository
{
    Task<IAccount> GetAccountByProviderAccount(Provider provider, string providerAccountId, CancellationToken ct);
}
