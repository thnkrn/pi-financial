namespace Pi.TfexService.Application.Services.UserService;

public interface IUserV2Service
{
    public Task<User> GetUserById(string userId);
    public Task<User> GetUserByCustomerCode(string customerCode);
    public Task<List<UserTradingAccountInfo>> GetTradingAccounts(string userId);
}