using MassTransit.Initializers;
using Microsoft.Extensions.Logging;
using Pi.Client.OnboardService.Api;
using Pi.Common.Domain.AggregatesModel.ProductAggregate;
using Pi.User.Application.Models;
using Pi.User.Application.Services.Onboard;
using Pi.User.Application.Services.SSO;
using Pi.User.Domain.AggregatesModel.TradingAccountAggregate;
using Pi.User.Domain.AggregatesModel.UserInfoAggregate;

namespace Pi.User.Application.Queries;

public class UserTradingAccountQueries : IUserTradingAccountQueries
{
    private readonly ILogger<UserTradingAccountQueries> _logger;
    private readonly IProductRepository _productRepository;
    private readonly ITradingAccountRepository _tradingAccountRepository;
    private readonly IUserInfoRepository _userInfoRepository;
    private readonly IOnboardTradingAccountService _onboardTradingAccountService;
    private readonly IUserQueries _userQueries;
    private readonly ISsoService _ssoService;

    public UserTradingAccountQueries(
        IUserInfoRepository userInfoRepository,
        ITradingAccountRepository tradingAccountRepository,
        IProductRepository productRepository,
        IOnboardTradingAccountService onboardTradingAccountService,
        ILogger<UserTradingAccountQueries> logger,
        IUserQueries userQueries,
        ISsoService ssoService
    )
    {
        _userInfoRepository = userInfoRepository;
        _tradingAccountRepository = tradingAccountRepository;
        _productRepository = productRepository;
        _onboardTradingAccountService = onboardTradingAccountService;
        _logger = logger;
        _userQueries = userQueries;
        _ssoService = ssoService;
    }

    public async Task<UserTradingAccountInfo?> GetUserTradingAccountInfoAsync(
        Guid userId,
        string customerCode,
        CancellationToken cancellationToken = default)
    {
        var tradingAccounts = await _tradingAccountRepository.GetTradingAccountsAsync(customerCode, cancellationToken);
        if (!tradingAccounts.Any())
        {
            _logger.LogWarning("There isn't any trading account of customer code {CustomerCode}", customerCode);
            return new UserTradingAccountInfo(userId, customerCode, Enumerable.Empty<TradingAccountWithProductInfo>());
        }

        var products = await _productRepository.GetProducts();
        var tradingAccountWithProductInfos = from ta in tradingAccounts
                                             join p in products
                                                 on new { ta.AccountTypeCode, ta.ExchangeMarketId }
                                                 equals new { p.AccountTypeCode, p.ExchangeMarketId }
                                             select new TradingAccountWithProductInfo
                                             {
                                                 TradingAccountNo = ta.TradingAccountNo,
                                                 Product = p,
                                                 CreditLine = ta.CreditLine,
                                                 CreditLineCurrency = ta.CreditLineCurrency,
                                                 CreditLineEffectiveDate = ta.CreditLineEffectiveDate.HasValue
                                                     ? DateOnly.FromDateTime(ta.CreditLineEffectiveDate.Value)
                                                     : null,
                                                 CreditLineEndDate = ta.CreditLineEndDate.HasValue
                                                     ? DateOnly.FromDateTime(ta.CreditLineEndDate.Value)
                                                     : null,
                                                 MarketingId = ta.MarketingId,
                                                 AccountOpeningDate = ta.AccountOpeningDate.HasValue
                                                     ? DateOnly.FromDateTime(ta.AccountOpeningDate.Value)
                                                     : null,
                                                 AccountStatus = ta.AccountStatus
                                             };

        return new UserTradingAccountInfo(userId, customerCode, tradingAccountWithProductInfos);
    }

    [Obsolete("This method is deprecated. Please use GetUserTradingAccountInfoAsync instead.")]
    public async Task<IEnumerable<string>> GetTradingAccountNoListAsync(string customerCode,
        CancellationToken cancellationToken = default)
    {
        var res = await _userInfoRepository.GetTradingAccountNoListByCustomerCodeAsync(customerCode, cancellationToken);

        if (res == null)
            throw new InvalidDataException($"Trading accounts not found for customer code: {customerCode}");

        return res.Select(t => t.TradingAccountNo).ToList();
    }

    /// <summary>
    /// Returns list of trading accounts, including external account details, for all customer codes belonging to
    /// <paramref name="userId"/> grouped by customer code.
    /// </summary>
    /// <param name="userId">User id to query trading accounts for each customer code.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of trading accounts grouped by customer code.</returns>
    /// <exception cref="InvalidDataException">Can't find citizen id for user <paramref name="userId"/>.</exception>
    public async Task<List<UserTradingAccountInfoWithExternalAccounts>>
        GetUserTradingAccountsWithExternalAccountsByUserId(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userQueries.GetUser(userId);
        var identificationNo = user.CitizenId;
        if (identificationNo == null)
        {
            throw new InvalidDataException($"Citizen id not found. UserId: {userId}");
        }

        var tradingAccountsGroupedByCustCode =
            await _onboardTradingAccountService.GetTradingAccountListGroupedByCustomerCodeByIdentificationNumberAsync(
                identificationNo, cancellationToken);
        List<UserTradingAccountInfoWithExternalAccounts> userTradingAccountInfoList = new();

        foreach (var tradingAccountsByCustCode in tradingAccountsGroupedByCustCode)
        {
            var customerCode = tradingAccountsByCustCode.CustCode;
            var tradingAccounts = tradingAccountsByCustCode.TradingAccounts;
            List<TradingAccountDetailsWithExternalAccounts> customerTradingAccounts = [];
            foreach (var tradingAccount in tradingAccounts)
            {
                var accountTypeCode = tradingAccount.AccountTypeCode;
                var productName = await _productRepository.GetProductNameFromAccountTypeCode(accountTypeCode);
                var tradingAccountDetails = new TradingAccountDetailsWithExternalAccounts(
                    tradingAccount.TradingAccountId,
                    tradingAccount.TradingAccountNo,
                    tradingAccount.AccountType,
                    tradingAccount.AccountTypeCode,
                    tradingAccount.ExchangeMarketId,
                    productName,
                    tradingAccount.ExternalAccounts.Select(externalAccountResp =>
                            new ExternalAccountDetails(
                                externalAccountResp.Id,
                                externalAccountResp.Account,
                                externalAccountResp.ProviderId))
                        .ToList());
                customerTradingAccounts.Add(tradingAccountDetails);
            }

            var userTradingAccountInfo =
                new UserTradingAccountInfoWithExternalAccounts(customerCode, customerTradingAccounts);
            userTradingAccountInfoList.Add(userTradingAccountInfo);
        }

        return userTradingAccountInfoList;
    }

    public async Task<List<CustomerCodeHasPin>> CheckHasPin(Guid userId, CancellationToken cancellationToken)
    {
        var user = await _userQueries.GetUser(userId);
        var identificationNo = user.CitizenId;
        if (identificationNo == null)
        {
            throw new InvalidDataException($"Citizen id not found. UserId: {userId}");
        }
        var tradingAccountsGroupedByCustCode =
            await _onboardTradingAccountService.GetTradingAccountListGroupedByCustomerCodeByIdentificationNumberAsync(
                identificationNo, cancellationToken);

        List<CustomerCodeHasPin> customerCodeList = [];
        foreach (var customerCode in tradingAccountsGroupedByCustCode.Select(tradingAccountsByCustCode => tradingAccountsByCustCode.CustCode))
        {
            var hasPin = await _ssoService.CheckSyncedPin(customerCode);
            customerCodeList.Add(new CustomerCodeHasPin(customerCode, hasPin));
        }

        return customerCodeList;
    }
}