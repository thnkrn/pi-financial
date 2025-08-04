using Pi.User.Application.Services.LegacyUserInfo;

namespace Pi.User.Application.Queries;

public interface ICustomerQueries
{
    Task<long> GetCustomerIdFromAccountStatus(string referId, string transId);
    Task<BankAccountInfo> GetBankAccountInfoByCustomerCode(string customerCode, CancellationToken cancellationToken = default);
}