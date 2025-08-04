using Microsoft.Extensions.Logging;
using Pi.Client.UserService.Api;
using Pi.TfexService.Application.Services.UserService;
using Pi.TfexService.Domain.Exceptions;

namespace Pi.TfexService.Infrastructure.Services;

public class UserService(IUserApi userApi, IUserMigrationApi userMigrationApi, ILogger<UserService> logger) : IUserService
{
    public async Task<User> GetUserById(string userId)
    {
        try
        {
            var userInfoResponse = await userApi.InternalUserIdGetAsync(userId);

            if (userInfoResponse == null)
            {
                throw new InvalidDataException($"Not found user with userId: {userId}");
            }

            return new User(
                userInfoResponse.Data.Id,
                userInfoResponse.Data.CustCodes,
                userInfoResponse.Data.TradingAccounts,
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
            var userInfoResponse = await userApi.InternalUserIdGetAsync(customerCode, true);

            if (userInfoResponse == null)
            {
                throw new InvalidDataException($"Not found user with customerCode: {customerCode}");
            }

            return new User(
                userInfoResponse.Data.Id,
                userInfoResponse.Data.CustCodes,
                userInfoResponse.Data.TradingAccounts,
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
            throw HandleException(e, "GetUserByCustomerCode");
        }
    }

    public async Task<List<UserTradingAccountInfo>> GetTradingAccounts(string userId)
    {
        try
        {
            var userTradingAccountInfo = await userMigrationApi.InternalGetTradingAccountV2Async(Guid.Parse(userId));

            if (userTradingAccountInfo == null)
            {
                throw new InvalidDataException($"Not found user trading account info with userId: {userId}");
            }

            var userTradingAccountInfoList = userTradingAccountInfo.Data.Select(u =>
                new UserTradingAccountInfo(u.CustomerCode, u.TradingAccounts.Select(t =>
                    new TradingAccountDetails(t.Id, t.TradingAccountNo, t.AccountType, t.AccountTypeCode, t.ExchangeMarketId, t.Product.ToString() ?? string.Empty, t.ExternalAccounts.Select(e =>
                        new ExternalAccountDetails(e.Id, e.Account, e.ProviderId)))))).ToList();

            return userTradingAccountInfoList;
        }
        catch (Exception e)
        {
            throw HandleException(e, "GetTradingAccounts");
        }
    }

    private UserApiException HandleException(Exception e, string methodName)
    {
        logger.LogError("UserService {MethodName}: {Message}", methodName, e.Message);
        return new UserApiException($"UserService {methodName}: {e.Message}");
    }
}