using Pi.GlobalEquities.Services.Velexa;

namespace Pi.GlobalEquities.Services;

public interface IAccountService
{
    Task<IAccount> GetAccount(string userId, string accountId, CancellationToken ct);
    Task<IAccount> GetAccount(string userId, Provider provider, string providerAccountId, CancellationToken ct);
    Task<IEnumerable<IAccount>> GetAccounts(string userId, CancellationToken ct);
    Task<IAccountBalance> GetAccountBalance(string userId, string accountId, Currency currency, CancellationToken ct);
    Task<IAccountBalance> GetAccountBalance(string userId, string accountId, Currency currency,
        VelexaModel.PositionResponse accountSummary,
        IEnumerable<IOrder> activeOrders,
        CancellationToken ct);
    Task<IAccountBalance> GetAccountBalance(string userId, Provider provider,
        string providerAccount, Currency currency,
        CancellationToken ct);
}
