namespace Pi.User.Application.Services.LegacyUserInfo;

public interface IUserTradingAccountService
{
    public Task<UserTradingAccount> GetUserTradingAccountByCustomerCodeAsync(string customerCode, CancellationToken cancellationToken = default);

}