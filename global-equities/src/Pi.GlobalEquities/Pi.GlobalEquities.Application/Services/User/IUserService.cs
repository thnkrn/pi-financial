using Pi.GlobalEquities.DomainModels;

namespace Pi.GlobalEquities.Application.Services.User;

public interface IUserService
{
    Task<IEnumerable<IAccount>> GetGeAccounts(string userId, CancellationToken ct);
    Task<IEnumerable<IAccount>> GetGeAccountsV2(string userId, CancellationToken ct);
}
