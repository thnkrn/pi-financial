using Pi.User.Application.Models;

namespace Pi.User.Application.Queries;

public interface IUserTradingAccountQueries
{
    Task<UserTradingAccountInfo?> GetUserTradingAccountInfoAsync(Guid userId, string customerCode, CancellationToken cancellationToken = default);

    Task<IEnumerable<string>> GetTradingAccountNoListAsync(string customerCode,
        CancellationToken cancellationToken = default);

    Task<List<UserTradingAccountInfoWithExternalAccounts>>
        GetUserTradingAccountsWithExternalAccountsByUserId(Guid userId, CancellationToken cancellationToken = default);

    Task<List<CustomerCodeHasPin>> CheckHasPin(Guid userId, CancellationToken cancellationToken);

}