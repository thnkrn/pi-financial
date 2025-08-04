using Pi.WalletService.Application.Models;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Application.Services.UserService;

public record User(
    Guid Id,
    List<string> CustCodes,
    List<string> ListTradingAccountNo,
    string FirstnameTh,
    string LastnameTh,
    string FirstnameEn,
    string LastnameEn,
    string GlobalAccount,
    string PhoneNumber,
    string Email
);

public interface IUserService
{
    public Task<User> GetUserInfoById(string userId);
    public Task<User> GetUserInfoByCustomerCode(string customerCode);
    public Task<string> GetUserCitizenId(Guid userId, CancellationToken cancellationToken);
    public Task<decimal> GetCreditLimit(string userId, string customerCode, Product product);
    public Task<BankAccount?> GetBankAccount(string userId);
}