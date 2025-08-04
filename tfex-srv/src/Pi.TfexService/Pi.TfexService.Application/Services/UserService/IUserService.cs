namespace Pi.TfexService.Application.Services.UserService;

public interface IUserService
{
    public Task<User> GetUserById(string userId);
    public Task<User> GetUserByCustomerCode(string customerCode);
    public Task<List<UserTradingAccountInfo>> GetTradingAccounts(string userId);
}