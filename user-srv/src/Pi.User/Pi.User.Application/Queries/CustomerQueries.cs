
using Microsoft.Extensions.Logging;
using Pi.User.Application.Services.LegacyUserInfo;

namespace Pi.User.Application.Queries;

public class CustomerQueries : ICustomerQueries
{
    private readonly IUserInfoService _userInfoService;
    private readonly IUserBankAccountService _userBankAccountService;

    public CustomerQueries(
        IUserInfoService userInfoService,
        IUserBankAccountService userBankAccountService)
    {
        _userInfoService = userInfoService;
        _userBankAccountService = userBankAccountService;
    }

    public async Task<long> GetCustomerIdFromAccountStatus(string referId, string transId)
    {
        var customerId = await _userInfoService.GetCustomerIdBpm(referId, transId);

        return customerId;
    }

    public async Task<BankAccountInfo> GetBankAccountInfoByCustomerCode(string customerCode, CancellationToken cancellationToken = default)
    {
        return await _userBankAccountService.GetBankAccountInfoAsync(customerCode, cancellationToken) ?? throw new InvalidDataException($"Cannot get bank account with customer code: {customerCode}");
    }
}