using Microsoft.Extensions.Logging;
using Pi.Common.CommonModels;
using Pi.Common.Features;
using Pi.GlobalEquities.Application.Exceptions;
using Pi.GlobalEquities.Application.Models.Dto;
using Pi.GlobalEquities.Application.Queries.Wallet;
using Pi.GlobalEquities.Application.Repositories;
using Pi.GlobalEquities.Application.Services.MarketData;
using Pi.GlobalEquities.Application.Services.User;
using Pi.GlobalEquities.Application.Services.Velexa;
using Pi.GlobalEquities.Application.Services.Wallet;
using Pi.GlobalEquities.DomainModels;
using Pi.GlobalEquities.Errors;
using Pi.GlobalEquities.Models;

namespace Pi.GlobalEquities.Application.Queries;

public class AccountQueries : IAccountQueries
{
    private readonly IWalletQueries _walletQueries;
    private readonly IAccountRepository _accountRepository;
    private readonly IUserService _userService;
    private readonly IVelexaReadService _velexaReadService;
    private readonly IMarketDataService _marketDataService;
    private readonly IFeatureService _featureService;
    private readonly ILogger<AccountQueries> _logger;

    private const string SsoPhase3 = "ge-user-v2-migration";

    public AccountQueries(IWalletQueries walletQueries, IAccountRepository accountRepository, IUserService userService,
        IVelexaReadService velexaReadService, IMarketDataService marketDataService, IFeatureService featureService,
        ILogger<AccountQueries> logger)
    {
        _walletQueries = walletQueries;
        _accountRepository = accountRepository;
        _userService = userService;
        _velexaReadService = velexaReadService;
        _marketDataService = marketDataService;
        _featureService = featureService;
        _logger = logger;
    }

    public async Task<IAccount?> GetAccountByAccountId(string userId, string accountId, CancellationToken ct)
    {
        if (_featureService.IsOff(SsoPhase3))
        {
            var account = await _accountRepository.GetAccount(userId, accountId, ct);
            if (account != null && !account.IsExpired())
                return account;

            var refreshedAccounts = await RefreshAccounts(userId, ct);
            account = refreshedAccounts.Length > 0
                ? refreshedAccounts.FirstOrDefault(x => x.Id == accountId)
                : account;

            return account;
        }

        var accounts = (await _userService.GetGeAccountsV2(userId, ct)).ToArray();

        await TryUpsertAccountsToDatabase(accounts, ct);

        var result = accounts.FirstOrDefault(x => x.Id == accountId);
        return result;
    }

    public async Task<IAccount?> GetAccountByProviderAccount(string userId, Provider provider, string providerAccountId,
        CancellationToken ct)
    {
        if (_featureService.IsOff(SsoPhase3))
        {
            var account = await _accountRepository.GetAccountByProviderAccount(userId, provider, providerAccountId, ct);
            if (account != null && !account.IsExpired())
                return account;

            var refreshedAccounts = await RefreshAccounts(userId, ct);
            account = refreshedAccounts.Length > 0
                ? refreshedAccounts.FirstOrDefault(x => x.GetProviderAccount(Provider.Velexa) == providerAccountId)
                : account;

            return account;
        }

        var accounts = (await _userService.GetGeAccountsV2(userId, ct)).ToArray();

        await TryUpsertAccountsToDatabase(accounts, ct);

        return accounts.FirstOrDefault(x => x.GetProviderAccount(Provider.Velexa) == providerAccountId);
    }

    public async Task<IAccountBalance?> GetAccountBalanceByAccountId(string userId, string accountId, Currency currency,
        CancellationToken ct)
    {
        var account = await GetAccountByAccountId(userId, accountId, ct);
        if (account == null)
            return null;

        var custCode = account.CustCode;
        var balance = await _walletQueries.GetLineAvailable(userId, custCode, currency, ct);

        var result = new AccountBalance
        {
            Id = account.Id,
            UserId = account.UserId,
            CustCode = account.CustCode,
            TradingAccountNo = account.TradingAccountNo,
            VelexaAccount = account.GetProviderAccount(Provider.Velexa),
            UpdatedAt = account.UpdatedAt,
            WithdrawableCash = balance,
            Currency = currency,
            EnableSell = account.EnableSell,
            EnableBuy = account.EnableBuy
        };
        return result;
    }

    public async Task<IAccountBalance?> GetAccountBalanceByProviderAccount(string userId, Provider provider, string providerAccount,
        Currency currency, CancellationToken ct)
    {
        var account = await GetAccountByProviderAccount(userId, provider, providerAccount, ct);
        if (account == null)
            return null;

        var custCode = account.CustCode;
        var balance = await _walletQueries.GetLineAvailable(userId, custCode, currency, ct);

        var result = new AccountBalance
        {
            Id = account.Id,
            UserId = account.UserId,
            CustCode = account.CustCode,
            TradingAccountNo = account.TradingAccountNo,
            VelexaAccount = account.GetProviderAccount(Provider.Velexa),
            UpdatedAt = account.UpdatedAt,
            WithdrawableCash = balance,
            Currency = currency,
            EnableSell = account.EnableSell,
            EnableBuy = account.EnableBuy
        };
        return result;
    }

    public async Task<AccountSummaryDto> GetAccountSummary(string userId, string accountId, IEnumerable<Currency> currencies,
        CancellationToken ct)
    {
        var account = await GetAccountByAccountId(userId, accountId, ct);
        if (account == null)
            throw new GeException(AccountErrors.NotExist);

        currencies = currencies.ToArray();

        var extraCurrencies = currencies.Where(x => x != Currency.USD).ToArray();

        var tasks = new List<Task>();
        var velexaAccountId = account.GetProviderAccount(Provider.Velexa);
        var tAccountSummary = _velexaReadService.GetAccountSummary(velexaAccountId, Currency.USD, ct);
        tasks.Add(tAccountSummary);

        var tActiveOrder = _velexaReadService.GetActiveOrders(velexaAccountId, null, ct);
        tasks.Add(tActiveOrder);

        var tExRateThUs = _walletQueries.GetExchangeRate(Currency.USD, Currency.THB, ct);
        tasks.Add(tExRateThUs);

        var tVelexaExRateHkUs = _velexaReadService.GetExchangeRate(Currency.HKD, Currency.USD, ct);
        tasks.Add(tVelexaExRateHkUs);

        var tExchangeRates = new Dictionary<Currency, Task<decimal>>();
        foreach (var currency in extraCurrencies)
        {
            tExchangeRates[currency] = _velexaReadService.GetExchangeRate(Currency.USD, currency, ct);
            tasks.Add(tExchangeRates[currency]);
        }

        await Task.WhenAll(tasks);

        var summary = await tAccountSummary;
        var unGroupedActiveOrder = await tActiveOrder;
        var groupedActiveOrder = OrderGroupMapper.MapOrdersByGroupIds(unGroupedActiveOrder).ToArray();
        var exRateThUs = await tExRateThUs;
        var velexaExRateHkUs = await tVelexaExRateHkUs;
        var exchangeRates = tExchangeRates.ToDictionary(x => x.Key, x => x.Value.Result);

        var accountBalance = _walletQueries.GetLineAvailableUsd(velexaAccountId, summary, groupedActiveOrder, velexaExRateHkUs);

        var symbols = summary.Positions.Select(x => x.SymbolId);
        var marketPrices = (await _marketDataService.GetTicker(symbols, ct)).ToArray();

        var accountSummary = CalculateSummary(summary, groupedActiveOrder, marketPrices, velexaExRateHkUs,
            exchangeRates, accountBalance);

        return new AccountSummaryDto
        {
            TradingAccountNo = account.TradingAccountNo,
            TradingAccountNoDisplay = account.TradingAccountNo.Replace("-", ""),
            UpnlPercentage = accountSummary.TotalUpnlPercentage,
            ExchangeRate = exRateThUs,
            Values = currencies.Select(cur => new AccountSummaryValueDto
            {
                Currency = cur,
                NetAssetValue = cur == Currency.USD ? accountSummary.NetAssetValue : accountSummary.NetAssetValue * exchangeRates[cur],
                MarketValue = cur == Currency.USD ? accountSummary.TotalMarketValue : accountSummary.TotalMarketValue * exchangeRates[cur],
                Cost = cur == Currency.USD ? accountSummary.TotalCost : accountSummary.TotalCost * exchangeRates[cur],
                Upnl = cur == Currency.USD ? accountSummary.TotalUpnl : accountSummary.TotalUpnl * exchangeRates[cur],
                UnusedCash = cur == Currency.USD ? accountSummary.WithdrawableCash : accountSummary.WithdrawableCash * exchangeRates[cur],
                AccountLimit = 100000m,
                LineAvailable = cur == Currency.USD ? accountSummary.LineAvailable : accountSummary.LineAvailable * exchangeRates[cur],
            }),
            Positions = accountSummary.Positions.Select(x => new PositionDto
            {
                Symbol = x.Symbol,
                Venue = x.Venue,
                Currency = x.Currency,
                EntireQuantity = x.EntireQuantity,
                AvailableQuantity = x.AvailableQuantity,
                LastPrice = x.LastPrice,
                MarketValue = x.MarketValue,
                AveragePrice = x.AveragePrice,
                Upnl = x.Upnl,
                Cost = x.Cost,
                UpnlPercentage = x.UpnlPercentage,
                Logo = x.Logo
            })
        };
    }

    private static AccountSummary CalculateSummary(
    AccountSummaryPosition summary,
    IList<IOrder> groupedActiveOrder,
    IList<SymbolPrice> marketPrices,
    decimal velexaExRateHkUs,
    Dictionary<Currency, decimal> exchangeRates,
    decimal accountBalance)
    {
        IEnumerable<PositionSummary> sumPositions = summary.Positions ?? Enumerable.Empty<PositionSummary>();
        var fPositions = new List<Position>();
        decimal totalCost = 0, totalUpnl = 0, totalMarketValue = 0;

        foreach (var pos in sumPositions)
        {
            if (pos.SymbolType != "STOCK" || pos.EntireQuantity == 0) continue;

            var activeSellQuantity = groupedActiveOrder?.Where(x => x.Symbol == pos.Symbol && x.Venue == pos.Venue).Sum(x => x.Quantity) ?? 0;
            var lastPrice = GetMarketPrice(marketPrices, pos.Symbol, pos.Venue, pos.LastPrice);
            var marketValue = lastPrice * pos.EntireQuantity;
            var usdMarketValue = ConvertToUsdMarketValue(pos.Currency, marketValue, velexaExRateHkUs);

            var usdCost = ConvertToUsdCost(pos.Currency, pos.Cost, velexaExRateHkUs);
            var usdUpnl = usdMarketValue - usdCost;

            fPositions.Add(new Position
            {
                Symbol = pos.Symbol,
                Venue = pos.Venue,
                Currency = pos.Currency,
                EntireQuantity = pos.EntireQuantity,
                AvailableQuantity = pos.EntireQuantity - activeSellQuantity,
                SymbolType = pos.SymbolType,
                LastPrice = lastPrice,
                MarketValue = marketValue,
                ConvertedMarketValue = usdMarketValue,
                AveragePrice = pos.AveragePrice,
                Upnl = marketValue - pos.Cost,
                ConvertedUpnl = usdUpnl,
                Cost = pos.Cost,
                ConvertedCost = usdCost,
                UpnlPercentage = CalculateUpnlPercentage(usdCost, usdUpnl)
            });

            totalCost += usdCost;
            totalUpnl += usdUpnl;
            totalMarketValue += ConvertMarketValue(pos.Currency, summary.Currency, marketValue, exchangeRates);
        }

        var exchangeRate = GetExchangeRate(summary.Currency, exchangeRates);
        return CreateAccountSummary(summary, totalMarketValue, totalCost, totalUpnl, accountBalance, exchangeRate, fPositions);
    }

    private static decimal GetMarketPrice(IEnumerable<SymbolPrice> marketPrices, string symbol, string venue, decimal vLastPrice)
    {
        var marketPrice = marketPrices.FirstOrDefault(x => x.Symbol == symbol && x.Venue == venue);
        return (marketPrice != null && marketPrice.Price != 0m) ? marketPrice.Price : vLastPrice;
    }

    private static decimal ConvertToUsdMarketValue(Currency currency, decimal marketValue, decimal velexaExRateHkUs)
    {
        return currency == Currency.HKD ? marketValue * velexaExRateHkUs : marketValue;
    }

    private static decimal ConvertToUsdCost(Currency currency, decimal cost, decimal velexaExRateHkUs)
    {
        return currency == Currency.HKD ? cost * velexaExRateHkUs : cost;
    }

    private static decimal CalculateUpnlPercentage(decimal usdCost, decimal usdUpnl)
    {
        return usdCost != 0 ? usdUpnl * 100 / usdCost : 0;
    }

    private static decimal ConvertMarketValue(Currency posCurrency, Currency summaryCurrency, decimal marketValue, Dictionary<Currency, decimal> exchangeRates)
    {
        if (posCurrency == summaryCurrency) return marketValue;
        return marketValue * (exchangeRates.GetValueOrDefault(summaryCurrency, 1));
    }

    private static decimal GetExchangeRate(Currency currency, Dictionary<Currency, decimal> exchangeRates)
    {
        return exchangeRates.GetValueOrDefault(currency, 1);
    }

    private static AccountSummary CreateAccountSummary(AccountSummaryPosition summary, decimal totalMarketValue, decimal totalCost, decimal totalUpnl, decimal accountBalance, decimal exchangeRate, List<Position> fPositions)
    {
        return new AccountSummary
        {
            Currency = summary.Currency,
            NetAssetValue = summary.NetAssetValue,
            TotalMarketValue = totalMarketValue,
            TotalCost = totalCost,
            TotalUpnl = totalUpnl,
            LineAvailable = accountBalance * exchangeRate,
            WithdrawableCash = accountBalance * exchangeRate,
            Positions = fPositions
        };
    }

    private async Task TryUpsertAccountsToDatabase(
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
            var result = await _userService.GetGeAccounts(userId, ct);
            var accounts = result.ToArray();
            await TryUpsertAccountsToDatabase(accounts, ct);

            return accounts;
        }
        catch (Pi.Client.UserService.Client.ApiException ex)
        {
            if (ex.ErrorCode >= 500)
                _logger.LogWarning(ex, "Cannot refresh user accounts, ErrorCode: {ErrorCode}", ex.ErrorCode);
            else
            {
                _logger.LogWarning(ex, "Cannot refresh user accounts");
                throw;
            }
        }

        return Array.Empty<IAccount>();
    }
}
