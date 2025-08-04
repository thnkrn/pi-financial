using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Pi.Client.UserService.Api;
using Pi.Client.UserService.Client;
using Pi.WalletService.Application.Models;
using Pi.WalletService.Application.Services.UserService;
using Pi.WalletService.Application.Utilities;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly IUserApi _userApi;
    private readonly IUserTradingAccountApi _userTradingAccountApi;
    private readonly IUserBankAccountApi _userBankAccountApi;
    private readonly ILogger<UserService> _logger;
    private readonly bool _useMockValues;
    private readonly decimal _creditLimitMockValue;

    public UserService(IUserApi userApi, IUserTradingAccountApi userTradingAccountApi,
        IUserBankAccountApi userBankAccountApi, ILogger<UserService> logger, IConfiguration configuration)
    {
        _userApi = userApi;
        _userTradingAccountApi = userTradingAccountApi;
        _userBankAccountApi = userBankAccountApi;
        _logger = logger;
        _useMockValues = configuration.GetValue<bool>("User:UseMockValues");
        _creditLimitMockValue = decimal.Parse(configuration.GetValue<string>("User:CreditLimitValue") ?? "0");
    }

    public async Task<User> GetUserInfoById(string userId)
    {
        try
        {
            var userInfoResponse = await _userApi.InternalUserIdGetAsync(userId);

            if (userInfoResponse == null)
            {
                throw new InvalidDataException($"Not found user with UserId {userId}");
            }

            return new User(
                userInfoResponse.Data.Id,
                userInfoResponse.Data.CustCodes,
                userInfoResponse.Data.TradingAccounts,
                userInfoResponse.Data.FirstnameTh,
                userInfoResponse.Data.LastnameTh,
                userInfoResponse.Data.FirstnameEn,
                userInfoResponse.Data.LastnameEn,
                userInfoResponse.Data.GlobalAccount,
                userInfoResponse.Data.PhoneNumber,
                userInfoResponse.Data.Email
            );
        }
        catch (Exception e)
        {
            _logger.LogError(e, "UserService:GetUserInfoById: Unable to get user info with UserId {ID} ", userId);
            throw;
        }
    }

    public async Task<User> GetUserInfoByCustomerCode(string customerCode)
    {
        try
        {
            var userInfoResponse = await _userApi.InternalUserIdGetAsync(customerCode, true);

            if (userInfoResponse == null)
            {
                throw new InvalidDataException($"Not found user with CustomerId {customerCode}");
            }

            return new User(
                userInfoResponse.Data.Id,
                userInfoResponse.Data.CustCodes,
                userInfoResponse.Data.TradingAccounts,
                userInfoResponse.Data.FirstnameTh,
                userInfoResponse.Data.LastnameTh,
                userInfoResponse.Data.FirstnameEn,
                userInfoResponse.Data.LastnameEn,
                userInfoResponse.Data.GlobalAccount,
                userInfoResponse.Data.PhoneNumber,
                userInfoResponse.Data.Email
            );
        }
        catch (Exception e)
        {
            _logger.LogError(e, "UserService:GetUserInfoById: Unable to get user info with UserId {ID} ", customerCode);
            throw;
        }
    }

    public async Task<string> GetUserCitizenId(Guid userId, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _userApi.InternalUserIdCitizenIdGetAsync(userId.ToString(), cancellationToken);
            return response.Data.CitizenId;
        }
        catch (ApiException ex)
        {
            if (ex.ErrorCode == 400)
            {
                throw new InvalidUserIdException(userId, ex);
            }

            throw;
        }
    }

    public async Task<decimal> GetCreditLimit(string userId, string customerCode, Product product)
    {
        if (_useMockValues)
        {
            return await Task.FromResult(_creditLimitMockValue);
        }

        var response =
            await _userTradingAccountApi.GetUserTradingAccountInfoByUserIdAsync(new Guid(userId), customerCode);
        return (decimal)response.Data.TradingAccounts.FirstOrDefault(t =>
            TradingAccountUtils.TryFindProductFromTradingAccount(t.TradingAccountNo) == product)!.CreditLine;
    }

    public async Task<BankAccount?> GetBankAccount(string userId)
    {
        try
        {
            var bankAccount = await _userBankAccountApi.GetBankAccountByUserIdAsync(userId);

            if (string.IsNullOrEmpty(bankAccount.Data.AccountNo) ||
                string.IsNullOrEmpty(bankAccount.Data.BankCode))
            {
                _logger.LogInformation("Not bank account with userId {UserId}", userId);
                return null;
            }

            return new BankAccount(bankAccount.Data.BankCode, bankAccount.Data.AccountNo, "0000");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "UserService:GetBankAccount: Unable to get bank account with UserId {UserId} ", userId);
            throw;
        }
    }
}