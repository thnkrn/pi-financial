using Microsoft.Extensions.Logging;
using Pi.Client.UserService.Api;
using Pi.Client.UserSrvV2.Api;
using Pi.Client.UserSrvV2.Model;
using Pi.Common.Features;
using Pi.GlobalEquities.Repositories;
using Pi.GlobalEquities.Services.Velexa;
using Pi.GlobalEquities.Services.Wallet;
using UserAPIResponse =
    Pi.Client.UserService.Model.PiUserApplicationModelsUserTradingAccountInfoWithExternalAccountsListApiResponse;

namespace Pi.GlobalEquities.Services;

public class AccountService : IAccountService
{
    private const string SsoPhase3 = "ge-user-v2-migration";

    readonly IWalletService _walletService;
    readonly IAccountRepository _accountRepository;
    private readonly IUserMigrationApi _userApi;
    private ILogger<AccountService> _logger;
    private readonly ITradingAccountApi _tradingAccountV2Api;
    private readonly IFeatureService _featureService;

    public AccountService(IWalletService walletService, IAccountRepository accountRepository, IUserMigrationApi userApi,
        ILogger<AccountService> logger, ITradingAccountApi tradingAccountV2Api, IFeatureService featureService)
    {
        _walletService = walletService;
        _accountRepository = accountRepository;
        _userApi = userApi;
        _logger = logger;
        _featureService = featureService;
        _tradingAccountV2Api = tradingAccountV2Api;
    }

    public async Task<IAccount> GetAccount(string userId, string accountId, CancellationToken ct)
    {
        var account = await _accountRepository.GetAccount(userId, accountId, ct);
        if (account != null && !account.IsExpired())
            return account;

        var refreshedAccounts = await RefreshAccounts(userId, ct);
        account = refreshedAccounts.Any()
            ? refreshedAccounts.FirstOrDefault(x => x.Id == accountId)
            : account;

        return account;
    }

    public async Task<IAccount> GetAccount(string userId, Provider provider, string providerAccountId,
        CancellationToken ct)
    {
        var account = await _accountRepository.GetAccountByProviderAccount(userId, provider, providerAccountId, ct);
        if (account != null && !account.IsExpired())
            return account;

        var refreshedAccounts = await RefreshAccounts(userId ?? account?.UserId, ct);
        account = refreshedAccounts.Any()
            ? refreshedAccounts.FirstOrDefault(x => x.GetProviderAccount(Provider.Velexa) == providerAccountId)
            : account;

        return account;
    }

    public async Task<IEnumerable<IAccount>> GetAccounts(string userId, CancellationToken ct)
    {
        var accounts = (await _accountRepository.GetAccounts(userId, ct)).ToArray();
        var isExpired = accounts.Any(x => x.IsExpired());

        if (accounts.Any() && !isExpired)
            return accounts;

        var refreshedAccounts = await RefreshAccounts(userId, ct);
        accounts = refreshedAccounts.Any() ? refreshedAccounts : accounts;

        return accounts;
    }

    public async Task<IAccountBalance> GetAccountBalance(string userId, string accountId, Currency currency,
        CancellationToken ct)
    {
        var account = await GetAccount(userId, accountId, ct);
        if (account == null)
            return null;

        var custCode = account.CustCode;
        var balance = await _walletService.SecureWalletProductLineAvailabilityGetAsync(userId, custCode, currency, ct);

        var result = new AccountBalance
        {
            Id = account.Id,
            UserId = account.UserId,
            CustCode = account.CustCode,
            TradingAccountNo = account.TradingAccountNo,
            VelexaAccount = account.GetProviderAccount(Provider.Velexa),
            UpdatedAt = account.UpdatedAt,
            WithdrawableCash = balance,
            Currency = currency
        };
        return result;
    }

    public async Task<IAccountBalance> GetAccountBalance(string userId, string accountId, Currency currency,
        VelexaModel.PositionResponse accountSummary,
        IEnumerable<IOrder> activeOrders,
        CancellationToken ct)
    {
        var account = await GetAccount(userId, accountId, ct);
        if (account == null)
            return null;

        var providerAccount = account.GetProviderAccount(Provider.Velexa);
        var balance = await _walletService.CustomWalletProductLineAvailabilityGetAsync(
            providerAccount, accountSummary, activeOrders, currency, ct);

        var result = new AccountBalance
        {
            Id = account.Id,
            UserId = account.UserId,
            CustCode = account.CustCode,
            TradingAccountNo = account.TradingAccountNo,
            VelexaAccount = account.GetProviderAccount(Provider.Velexa),
            UpdatedAt = account.UpdatedAt,
            WithdrawableCash = balance,
            Currency = currency
        };
        return result;
    }

    public async Task<IAccountBalance> GetAccountBalance(string userId, Provider provider, string providerAccount,
        Currency currency,
        CancellationToken ct)
    {
        var account = await GetAccount(userId, provider, providerAccount, ct);
        if (account == null)
            return null;

        var custCode = account.CustCode;
        var balance = await _walletService.SecureWalletProductLineAvailabilityGetAsync(userId, custCode, currency, ct);

        var result = new AccountBalance
        {
            Id = account.Id,
            UserId = account.UserId,
            CustCode = account.CustCode,
            TradingAccountNo = account.TradingAccountNo,
            VelexaAccount = account.GetProviderAccount(Provider.Velexa),
            UpdatedAt = account.UpdatedAt,
            WithdrawableCash = balance,
            Currency = currency
        };
        return result;
    }

    private async Task<IEnumerable<IAccount>> FetchAccount(string userId, CancellationToken ct)
    {
        var guid = new Guid(userId);
        if (_featureService.IsOff(SsoPhase3))
        {
            var v1Response = await _userApi.InternalGetTradingAccountV2Async(guid, ct);
            return GetSupportedAccounts(v1Response, userId);
        }
        var v2Response = await _tradingAccountV2Api.InternalV1TradingAccountsGetAsync(userId, "N", cancellationToken: ct);
        return GetSupportedAccounts(v2Response, userId);
    }

    private IEnumerable<IAccount> GetSupportedAccounts(
        UserAPIResponse tradingAccount,
        string userId)
    {
        foreach (var accountItem in tradingAccount.Data)
        {
            foreach (var acc in accountItem.TradingAccounts)
            {
                var account = acc.ExternalAccounts.FirstOrDefault(x => x.ProviderId == (int)Provider.Velexa);
                if (account == null)
                    continue;

                var accountId = account.Id.ToString();
                yield return new Account
                {
                    Id = accountId,
                    UserId = userId,
                    CustCode = accountItem.CustomerCode,
                    TradingAccountNo = acc.TradingAccountNo,
                    VelexaAccount = account.Account,
                    UpdatedAt = DateTime.UtcNow
                };
            }
        }
    }

    private IEnumerable<IAccount> GetSupportedAccounts(
        InternalV1TradingAccountsGet200Response tradingAccount,
        string userId)
    {
        var supportedAccounts = new List<IAccount>();

        foreach (var accountItem in tradingAccount.Data)
        {
            supportedAccounts.AddRange(
                from acc in accountItem.TradingAccounts
                let externalAccount = acc.ExternalAccounts
                    .FirstOrDefault(x => x.ProviderId == (int)Provider.Velexa)
                where externalAccount != null
                select new Account
                {
                    Id = externalAccount.Id,
                    UserId = userId,
                    CustCode = accountItem.CustomerCode,
                    TradingAccountNo = acc.TradingAccountNo,
                    VelexaAccount = externalAccount.Account,
                    UpdatedAt = DateTime.UtcNow
                });
        }

        return supportedAccounts;
    }

    private async void TryUpsertAccountsToDatabase(
        IEnumerable<IAccount> accounts,
        CancellationToken ct)
    {
        try
        {
            await _accountRepository.UpsertAccounts(accounts, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Something went wrong when try to replace data in database");
        }
    }

    private async Task<IAccount[]> RefreshAccounts(string userId, CancellationToken ct)
    {
        try
        {
            var result = await FetchAccount(userId, ct);
            var accounts = result.ToArray();
            TryUpsertAccountsToDatabase(accounts, ct);

            return accounts;
        }
        catch (Pi.Client.UserService.Client.ApiException ex)
        {
            if (ex.ErrorCode >= 500)
                _logger.LogWarning(ex, "Cannot refresh user accounts");
            else
                throw;
        }

        return Array.Empty<IAccount>();
    }
}
