using Microsoft.Extensions.Logging;
using Pi.Client.OnboardService.Api;
using Pi.Client.UserService.Api;
using CustCode = Pi.Client.UserService.Model.PiUserApplicationModelsCustomerCode;
using PiOnboardApiException = Pi.Client.OnboardService.Client.ApiException;
using PiUserApiException = Pi.Client.UserService.Client.ApiException;

namespace Pi.StructureNotes.Infrastructure.Services;

public class AccountService : IAccountService
{
    private readonly IUserDataCache _cache;
    private readonly ICustomerInfoApi _customerApi;

    private readonly ILogger _logger;
    private readonly ITradingAccountApi _tradingApi;
    private readonly IUserApi _userApi;

    public AccountService(IUserApi userApi, ICustomerInfoApi customerApi, ITradingAccountApi tradingApi,
        IUserDataCache cache, ILogger<AccountService> logger)
    {
        _userApi = userApi;
        _customerApi = customerApi;
        _tradingApi = tradingApi;
        _cache = cache;
        _logger = logger;
    }

    public async Task<AccountInfo> GetSnAccountById(string userId, string accountId, CancellationToken ct = default)
    {
        var accounts = await GetSnAccounts(userId, ct);
        return accounts.FirstOrDefault(x => x.AccountId == accountId);
    }

    public async Task<AccountInfo> GetSnAccountByAccountNo(string accountNo, CancellationToken ct = default)
    {
        var custCode = accountNo.Substring(0, accountNo.IndexOf("-"));
        var accounts = await GetStructuredNotesAccounts(custCode, ct);
        var account = accounts.SingleOrDefault(x => x.AccountNo == accountNo);
        return account;
    }

    public async Task<IEnumerable<AccountInfo>> GetSnAccounts(string userId, CancellationToken ct = default)
    {
        if (_cache.TryGetUserAccounts(userId, out IEnumerable<AccountInfo> result))
        {
            return result;
        }

        var custCodes = await GetCustCodes(userId, ct);
        var tasks = new List<Task<IEnumerable<AccountInfo>>>();
        foreach (CustCode custCode in custCodes)
        {
            tasks.Add(GetStructuredNotesAccounts(custCode.Code, ct));
        }

        await Task.WhenAll(tasks);

        result = tasks.Where(x => x.IsCompletedSuccessfully).SelectMany(x => x.Result).Distinct();

        _cache.AddUserAccounts(userId, result);

        return result;
    }

    private async Task<IEnumerable<AccountInfo>> GetStructuredNotesAccounts(string custCode, CancellationToken ct)
    {
        try
        {
            var isSnCustomer = await IsStructureNoteCustCode(custCode, ct);
            if (!isSnCustomer)
            {
                return Enumerable.Empty<AccountInfo>();
            }

            var response = await _tradingApi.InternalGetTradingAccountListByCustomerCodeAsync(custCode, cancellationToken: ct);
            var accounts = response.Data.Where(x =>
                    x is { ExchangeMarketId: "1", AccountTypeCode: "CH" } or
                    { ExchangeMarketId: "1", AccountType: "CC" });

            var accountInfos = accounts.Select(x => new AccountInfo
            {
                AccountId = x.Id.ToString(),
                AccountNo = x.TradingAccountNo,
                CustCode = x.CustomerCode
            });
            return accountInfos;
        }
        catch (PiOnboardApiException ex)
        {
            if (ex.ErrorCode == 404)
            {
                _logger.LogWarning("Couldn't find trading account for custcode: {custCode}", custCode);
                return Enumerable.Empty<AccountInfo>();
            }

            _logger.LogError(ex, "Error when calling Trading Client API,  " +
                                 "Error Content: {ex.ErrorContent} ," +
                                 "Cust Code: {custCode} ," +
                                 "Error Code: {ex.ErrorCode}",
                ex.ErrorContent, custCode, ex.ErrorCode);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogWarning($"Could not find account info by custcode {custCode}, " +
                               $"Error: {ex.Message}", custCode);
            throw;
        }
    }

    private async Task<IEnumerable<CustCode>> GetCustCodes(string userId, CancellationToken ct)
    {
        try
        {
            var userResponse = await _userApi.GetUserByIdOrCustomerCodeV2Async(userId, null, ct);
            var user = userResponse.Data;
            if (user == null)
            {
                throw new Exception($"Could not find user by user id {userId}");
            }

            return user.CustomerCodes;
        }
        catch (PiUserApiException ex)
        {
            _logger.LogError(ex, "Error when calling User Client API,  " +
                                 "Error Content: {ex.ErrorContent} ," +
                                 "UserId: {userId} ," +
                                 "Error Code: {ex.ErrorCode}",
                ex.ErrorContent, userId, ex.ErrorCode);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogWarning($"Could not find account info by user Id {userId}, " +
                               $"Error: {ex.Message}", userId);
            throw;
        }
    }

    private async Task<bool> IsStructureNoteCustCode(string custCode, CancellationToken ct)
    {
        try
        {
            var customerResponse = await _customerApi.InternalGetCustomerInfoByCustomerCodeAsync(custCode, ct);
            var customer = customerResponse.Data;
            var subType = customer.CustomerSubType;
            var custType = customer.CustomerType;

            var isSnCustomer = (subType == "5" && (custType == "1" || custType == "2")) ||
                               (subType == "6" && custType == "4");

            return isSnCustomer;
        }
        catch (PiOnboardApiException ex)
        {
            if (ex.ErrorCode == 404)
            {
                _logger.LogWarning("Couldn't find customer info for custcode: {custCode}", custCode);
                return false;
            }

            _logger.LogError(ex, "Error when calling Customer Client API,  " +
                                 "Error Content: {ex.ErrorContent} ," +
                                 "Cust Code: {custCode} ," +
                                 "Error Code: {ex.ErrorCode}",
                ex.ErrorContent, custCode, ex.ErrorCode);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogWarning($"Could not find account info by custCode {custCode}, " +
                               $"Error: {ex.Message}", custCode);
            throw;
        }
    }
}
