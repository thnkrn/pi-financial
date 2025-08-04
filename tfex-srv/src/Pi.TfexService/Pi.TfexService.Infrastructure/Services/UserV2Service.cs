using Microsoft.Extensions.Logging;
using Pi.Client.UserSrvV2.Api;
using Pi.TfexService.Application.Services.UserService;
using Pi.TfexService.Domain.Exceptions;

namespace Pi.TfexService.Infrastructure.Services;

public class UserV2Service(
    IUserApi userApi,
    ITradingAccountApi tradingAccountApi,
    ILogger<UserV2Service> logger) : IUserV2Service
{
    public async Task<User> GetUserById(string userId)
    {
        try
        {
            var userInfoResponse = await userApi.SecureV1UsersGetAsync(userId);

            if (userInfoResponse == null)
            {
                throw new InvalidDataException($"Not found user with userId: {userId}");
            }

            return new User(
                Guid.Parse(userInfoResponse.Data.Id),
                userInfoResponse.Data.CustCodes,
                userInfoResponse.Data.TradingAccounts.Select(a => a.Replace("-", "")).ToList(),
                userInfoResponse.Data.FirstnameTh,
                userInfoResponse.Data.LastnameTh,
                userInfoResponse.Data.FirstnameEn,
                userInfoResponse.Data.LastnameEn,
                userInfoResponse.Data.PhoneNumber,
                userInfoResponse.Data.Email
            );
        }
        catch (Exception e)
        {
            throw HandleException(e, "GetUserById");
        }
    }

    public async Task<User> GetUserByCustomerCode(string customerCode)
    {
        try
        {
            var userInfoResponse = await userApi.InternalV1UsersGetAsync(null, customerCode);
            var user = userInfoResponse.Data.FirstOrDefault(u => u.CustCodes.Contains(customerCode));

            if (user == null)
            {
                throw new InvalidDataException($"Not found user with customerCode: {customerCode}");
            }

            return new User(
                Guid.Parse(user.Id),
                user.CustCodes,
                user.TradingAccounts.Select(a => a.Replace("-", "")).ToList(),
                user.FirstnameTh,
                user.LastnameTh,
                user.FirstnameEn,
                user.LastnameEn,
                user.PhoneNumber,
                user.Email
            );
        }
        catch (Exception e)
        {
            throw HandleException(e, "GetUserByCustomerCode");
        }
    }

    public async Task<List<UserTradingAccountInfo>> GetTradingAccounts(string userId)
    {
        try
        {
            var userTradingAccountInfo = await tradingAccountApi.InternalV1TradingAccountsGetAsync(userId);

            if (userTradingAccountInfo == null)
            {
                throw new InvalidDataException($"Not found user trading account info with userId: {userId}");
            }

            var userTradingAccountInfoList = userTradingAccountInfo.Data.Select(u =>
                new UserTradingAccountInfo(u.CustomerCode, u.TradingAccounts?.Select(t =>
                    new TradingAccountDetails(Guid.Parse(t.Id), t.TradingAccountNo, t.AccountType, t.AccountTypeCode, t.ExchangeMarketId, t.ProductName.ToString(), t.ExternalAccounts.Select(e =>
                        new ExternalAccountDetails(Guid.Parse(e.Id), e.Account, e.ProviderId)))) ?? [])).ToList();

            return userTradingAccountInfoList;
        }
        catch (Exception e)
        {
            throw HandleException(e, "GetTradingAccounts");
        }
    }

    private UserApiException HandleException(Exception e, string methodName)
    {
        logger.LogError("UserV2Service {MethodName}: {Message}", methodName, e.Message);
        return new UserApiException($"UserV2Service {methodName}: {e.Message}");
    }
}