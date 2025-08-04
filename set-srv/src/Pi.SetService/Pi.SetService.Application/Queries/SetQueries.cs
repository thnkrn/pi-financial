using Microsoft.Extensions.Logging;
using Pi.SetService.Application.Exceptions;
using Pi.SetService.Application.Factories;
using Pi.SetService.Application.Models;
using Pi.SetService.Application.Models.AccountInfo;
using Pi.SetService.Application.Models.AccountSummaries;
using Pi.SetService.Application.Services.MarketService;
using Pi.SetService.Application.Services.OnboardService;
using Pi.SetService.Application.Services.OneportService;
using Pi.SetService.Application.Services.PiInternalService;
using Pi.SetService.Application.Services.UserService;
using Pi.SetService.Application.Utils;
using Pi.SetService.Domain.AggregatesModel.AccountAggregate;
using Pi.SetService.Domain.AggregatesModel.ErrorAggregate;
using Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;
using Pi.SetService.Domain.AggregatesModel.InstrumentAggregate;
using Pi.SetService.Domain.AggregatesModel.TradingAggregate;
using SblOrder = Pi.SetService.Domain.AggregatesModel.TradingAggregate.SblOrder;

namespace Pi.SetService.Application.Queries;

public class SetQueries(
    IOnboardService onboardService,
    IUserService userService,
    IOnePortService onePortService,
    IMarketService marketService,
    IPiInternalService piInternalService,
    IInstrumentRepository instrumentRepository,
    ISblOrderRepository sblOrderRepository,
    ILogger<SetQueries> logger)
    : ISetQueries
{
    private const string EquityConstantThb = "THB";

    private static readonly TradingAccountType[] CashAccountTypes =
        [TradingAccountType.Cash, TradingAccountType.CashBalance];

    private static readonly TradingAccountType[] CreditAccountTypes = [TradingAccountType.CreditBalance];

    public async Task<CashAccountInfo> GetCashAccountInfoByTradingAccountNoAsync(Guid userId,
        string tradingAccountNo,
        CancellationToken cancellationToken = default)
    {
        var tradingAccount = await GetTradingAccount(userId, tradingAccountNo, cancellationToken);
        ValidateTradingAccountType(tradingAccount, CashAccountTypes);

        var availableCashBalances =
            await onePortService.GetAvailableCashBalances(tradingAccountNo, cancellationToken);
        var availableCashBalance = availableCashBalances.FirstOrDefault();
        if (availableCashBalance == null) throw new SetException(SetErrorCode.SE103);

        var backofficeBalance =
            await piInternalService.GetBackofficeAvailableBalance(tradingAccount.TradingAccountNo, cancellationToken);

        return ApplicationFactory.NewCashAccountInfo(tradingAccount, availableCashBalance,
            backofficeBalance);
    }

    public async Task<CashAccountSummary> GetCashAccountSummaryByTradingAccountNoAsync(Guid userId,
        string tradingAccountNo,
        CancellationToken cancellationToken = default)
    {
        var tradingAccount = await GetTradingAccount(userId, tradingAccountNo, cancellationToken);
        ValidateTradingAccountType(tradingAccount, CashAccountTypes);

        return await GetCashAccountSummary(tradingAccount, cancellationToken);
    }

    public async Task<CreditBalanceAccountSummary> GetCreditBalanceSummaryByTradingAccountNoAsync(Guid userId,
        string tradingAccountNo, CancellationToken cancellationToken = default)
    {
        var tradingAccount = await GetTradingAccount(userId, tradingAccountNo, cancellationToken);
        ValidateTradingAccountType(tradingAccount, CreditAccountTypes);

        return await GetCreditBalanceAccountSummary(tradingAccount, cancellationToken);
    }

    public async Task<IEnumerable<AccountSummary>> GetAccountSummariesByUserIdAsync(Guid userId,
        CancellationToken cancellationToken = default)
    {
        var custCodes = await userService.GetCustomerCodesByUserId(userId);

        var tasks = custCodes.Select(custCode =>
                onboardService.GetSetTradingAccountsByUserIdAsync(userId, custCode, cancellationToken))
            .ToArray();
        var tradingAccounts = (await Task.WhenAll(tasks)).SelectMany(response => response)
            .ToArray();

        if (tradingAccounts == null || tradingAccounts.Length == 0)
            throw new SetException(SetErrorCode.SE102);

        var func = new Func<TradingAccount, CancellationToken, Task<AccountSummary?>>(async (tradingAccount, ct) =>
        {
            try
            {
                if (tradingAccount.TradingAccountType == TradingAccountType.CreditBalance)
                    return await GetCreditBalanceAccountSummary(tradingAccount, ct);

                return await GetCashAccountSummary(tradingAccount, ct);
            }
            catch (SetException e)
            {
                logger.LogError(e, "Can't get account balance of {TradingAccountNo}", tradingAccount.AccountNo);
                return null;
            }
        });

        var resultTasks = tradingAccounts.Select(tradingAccount => func(tradingAccount, cancellationToken));

        return (await Task.WhenAll(resultTasks)).Where(q => q != null).Select(q => q!);
    }

    public async Task<List<EquityAsset>> GetCashBalancePositionsByTradingAccountNoAsync(Guid userId,
        string tradingAccountNo, CancellationToken cancellationToken = default)
    {
        var tradingAccount = await GetTradingAccount(userId, tradingAccountNo, cancellationToken);
        ValidateTradingAccountType(tradingAccount, CashAccountTypes);

        return await GetCashAssets(tradingAccountNo, cancellationToken);
    }

    public async Task<List<CreditBalanceEquityAsset>> GetCreditBalancePositionsByTradingAccountNoAsync(Guid userId,
        string tradingAccountNo, CancellationToken cancellationToken = default)
    {
        var tradingAccount = await GetTradingAccount(userId, tradingAccountNo, cancellationToken);
        ValidateTradingAccountType(tradingAccount, CreditAccountTypes);


        return await GetCreditBalanceAssets(tradingAccountNo, cancellationToken);
    }

    public async Task<List<OrderInfo>> GetOpenOrdersByTradingAccountNoAsync(Guid userId,
        string tradingAccountNo,
        CancellationToken cancellationToken = default)
    {
        var setOrderFilters = new SetOrderFilters
        {
            OpenOrder = true
        };

        return await GetOrdersAsync(userId, tradingAccountNo, setOrderFilters, cancellationToken);
    }

    public async Task<List<OrderInfo>> GetOrderHistoriesByTradingAccountNoAsync(Guid userId, string tradingAccountNo,
        SetOrderHistoriesFilters filters,
        CancellationToken cancellationToken = default)
    {
        ValidateDates(filters.EffectiveDateFrom, filters.EffectiveDateTo);

        var setOrderFilters = new SetOrderFilters
        {
            EffectiveDateFrom = filters.EffectiveDateFrom,
            EffectiveDateTo = filters.EffectiveDateTo,
            HistoryOrder = true
        };

        var result = await GetOrdersAsync(userId, tradingAccountNo, setOrderFilters, cancellationToken);

        result = result.OrderBy(order => order.OrderDateTime).ToList();
        result = ListHelper.ApplyPagination(result, filters.OffSet, filters.Limit);

        return result;
    }

    public async Task<List<Trade>> GetTradeHistoriesByTradingAccountNoAsync(Guid userId, string tradingAccountNo,
        SetTradeHistoriesFilters filters,
        CancellationToken cancellationToken = default)
    {
        ValidateDates(filters.EffectiveDateFrom, filters.EffectiveDateTo, 90);

        await GetTradingAccount(userId, tradingAccountNo, cancellationToken);

        var trades = await piInternalService.GetTradeHistories(tradingAccountNo, filters.EffectiveDateFrom,
            filters.EffectiveDateTo
            , cancellationToken);

        if (trades.Count == 0) return trades;

        trades = trades.OrderBy(trade => trade.DealDateTime).ToList();
        trades = ListHelper.ApplyPagination(trades, filters.OffSet, filters.Limit);

        return trades;
    }

    public async Task<EquityMarginInfo> GetMarginRateBySymbol(string symbol,
        CancellationToken cancellationToken = default)
    {
        var result = await instrumentRepository.GetEquityMarginInfo(symbol, cancellationToken);
        if (result == null) throw new SetException(SetErrorCode.SE110);

        return result;
    }

    public async Task<AccountInstrumentBalance> GetAccountInstrumentBalanceAsync(Guid userId,
        string tradingAccountNo, string symbol, CancellationToken ct = default)
    {
        var tradingAccount = await GetTradingAccount(userId, tradingAccountNo, ct);
        if (tradingAccount.TradingAccountType == TradingAccountType.CreditBalance)
        {
            var accountBalances =
                await onePortService.GetAvailableCreditBalances(tradingAccount.TradingAccountNo, ct);
            var accountBalance = accountBalances.Find(q => q.TradingAccountNo == tradingAccount.TradingAccountNo);

            if (accountBalance == null) throw new SetException(SetErrorCode.SE103);

            var accountPositions = await onePortService.GetPositionsCreditBalance(tradingAccountNo, ct);
            return new AccountInstrumentBalance(symbol, accountBalance,
                accountPositions.Where(q => q.SecSymbol.Equals(symbol, StringComparison.CurrentCultureIgnoreCase)));
        }
        else
        {
            var accountBalances =
                await onePortService.GetAvailableCashBalances(tradingAccount.TradingAccountNo, ct);
            var accountBalance = accountBalances.Find(q => q.TradingAccountNo == tradingAccount.TradingAccountNo);

            if (accountBalance == null) throw new SetException(SetErrorCode.SE103);

            var accountPositions = await onePortService.GetPositions(tradingAccountNo, ct);
            return new AccountInstrumentBalance(symbol, accountBalance,
                accountPositions.Where(q => q.SecSymbol.Equals(symbol, StringComparison.CurrentCultureIgnoreCase)));
        }
    }

    public async Task<AccountSblInstrumentBalance> GetAccountSblInstrumentBalanceAsync(Guid userId, string tradingAccountNo, string symbol,
        CancellationToken ct = default)
    {
        var tradingAccount = await GetTradingAccount(userId, tradingAccountNo, ct);
        if (!tradingAccount.SblEnabled) throw new SetException(SetErrorCode.SE115);

        ValidateTradingAccountType(tradingAccount, CreditAccountTypes);

        var balancesTask = onePortService.GetAvailableCreditBalances(tradingAccountNo, ct);
        var positionsTask = onePortService.GetPositionsCreditBalance(tradingAccountNo, ct);
        var tradingDetailTask = marketService.GetTradingDetail(symbol, ct);
        var corporateActionsTask = marketService.GetCorporateActions(symbol, ct);

        await Task.WhenAll(balancesTask, tradingDetailTask, corporateActionsTask, positionsTask);
        var creditBalance = (await balancesTask).Find(q => q.TradingAccountNo == tradingAccount.TradingAccountNo);
        if (creditBalance == null) throw new SetException(SetErrorCode.SE103);

        var tradingDetail = await tradingDetailTask;
        if (tradingDetail == null) throw new SetException(SetErrorCode.SE104);

        var positions =
            (await positionsTask).Where(q => q.SecSymbol.Equals(symbol, StringComparison.CurrentCultureIgnoreCase));
        var sblInstrument = await instrumentRepository.GetSblInstrument(symbol, ct);
        var marginInfo = await instrumentRepository.GetEquityMarginInfo(symbol, ct);
        SblInstrumentInfo? sblInstrumentInfo = null;
        if (marginInfo != null && sblInstrument != null)
            sblInstrumentInfo = new SblInstrumentInfo
            {
                SblInstrument = sblInstrument,
                CorporateActions = await corporateActionsTask,
                MarginInfo = marginInfo
            };

        return new AccountSblInstrumentBalance(symbol, creditBalance, positions, tradingDetail, sblInstrumentInfo);
    }

    public async Task<CreditAccountInfo> GetCreditAccountInfoByTradingAccountNoAsync(Guid userId,
        string tradingAccountNo,
        CancellationToken cancellationToken = default)
    {
        var tradingAccount = await GetTradingAccount(userId, tradingAccountNo, cancellationToken);
        ValidateTradingAccountType(tradingAccount, CreditAccountTypes);

        var availableCreditBalances =
            await onePortService.GetAvailableCreditBalances(tradingAccountNo, cancellationToken);
        var availableCreditBalance = availableCreditBalances.FirstOrDefault();
        if (availableCreditBalance == null) throw new SetException(SetErrorCode.SE103);

        return ApplicationFactory.NewCreditAccountInfo(tradingAccount, availableCreditBalance);
    }

    private async Task<CreditBalanceAccountSummary> GetCreditBalanceAccountSummary(TradingAccount tradingAccount,
        CancellationToken cancellationToken)
    {
        var creditBalances =
            await onePortService.GetAvailableCreditBalances(tradingAccount.TradingAccountNo, cancellationToken);
        var creditBalance = creditBalances.Find(q => q.TradingAccountNo == tradingAccount.TradingAccountNo);

        if (creditBalance == null) throw new SetException(SetErrorCode.SE103);

        var assets = await GetCreditBalanceAssets(tradingAccount.TradingAccountNo, cancellationToken);
        return ApplicationFactory.NewCreditBalanceAccountSummary(tradingAccount, creditBalance, assets);
    }

    private async Task<CashAccountSummary> GetCashAccountSummary(TradingAccount tradingAccount,
        CancellationToken cancellationToken)
    {
        var availableBalances =
            await onePortService.GetAvailableCashBalances(tradingAccount.TradingAccountNo, cancellationToken);
        var accountBalance = availableBalances.Find(q => q.TradingAccountNo == tradingAccount.TradingAccountNo);

        if (accountBalance == null) throw new SetException(SetErrorCode.SE103);

        var backofficeBalanceTask =
            piInternalService.GetBackofficeAvailableBalance(tradingAccount.TradingAccountNo, cancellationToken);
        var assetsTask = GetCashAssets(tradingAccount.TradingAccountNo, cancellationToken);

        await Task.WhenAll(backofficeBalanceTask, assetsTask);

        return ApplicationFactory.NewCashAccountSummary(tradingAccount, accountBalance, await assetsTask,
            await backofficeBalanceTask);
    }

    private async Task<List<EquityAsset>> GetCashAssets(string tradingAccountNo,
        CancellationToken cancellationToken)
    {
        var accountPositions = await onePortService.GetPositions(tradingAccountNo, cancellationToken);
        return await BuildEquityAssets(accountPositions, ApplicationFactory.NewEquityAsset, cancellationToken);
    }

    private async Task<List<CreditBalanceEquityAsset>> GetCreditBalanceAssets(string tradingAccountNo,
        CancellationToken cancellationToken)
    {
        var accountPositions = await onePortService.GetPositionsCreditBalance(tradingAccountNo, cancellationToken);
        return await BuildEquityAssets(accountPositions, ApplicationFactory.NewCreditBalanceEquityAsset,
            cancellationToken);
    }

    private async Task<List<TResult>> BuildEquityAssets<T, TResult>(List<T> accountPositions,
        Func<T, EquityInstrument?, TResult> func,
        CancellationToken cancellationToken)
        where T : AccountPosition
        where TResult : EquityAsset
    {
        if (accountPositions.Count == 0) return [];

        var symbols = accountPositions.Select(accountPosition => accountPosition.SecSymbol).Distinct().ToArray();

        var equityInstruments = await marketService.GetEquityInstruments(symbols, cancellationToken);
        var equityInstrumentsDict = equityInstruments.ToDictionary(i => i.Symbol, i => i);

        var assets = new List<TResult>();

        foreach (var accountPosition in accountPositions.Where(accountPosition =>
                     accountPosition is not { ActualVolume: 0, TodayRealize: 0 }))
        {
            if (string.Equals(accountPosition.SecSymbol, EquityConstantThb,
                    StringComparison.InvariantCultureIgnoreCase)) continue;

            equityInstrumentsDict.TryGetValue(accountPosition.SecSymbol, out var equityInstrument);

            assets.Add(func(accountPosition, equityInstrument));
        }

        assets.Sort(SortHelper.SortAccountPositions);
        return assets;
    }

    private static void ProcessOrders<T>(IEnumerable<T> orders, List<OrderInfo> result, string custCode,
        Dictionary<long, List<Deal>> deals, SetOrderFilters filters) where T : BaseOrder
    {
        foreach (var order in orders)
        {
            if (!filters.Execute(order))
                continue;

            deals.TryGetValue(order.OrderNo, out var dealsForOrderNo);
            if (dealsForOrderNo != null) order.SetDeals(dealsForOrderNo);
            var openOrder = ApplicationFactory.NewSetOder(order, custCode);
            result.Add(openOrder);
        }
    }

    private async Task<List<OrderInfo>> GetOrdersAsync(Guid userId, string tradingAccountNo, SetOrderFilters filters,
        CancellationToken cancellationToken = default)
    {
        var tradingAccount = await GetTradingAccount(userId, tradingAccountNo, cancellationToken);

        var ordersTask = onePortService.GetOrdersByAccountNo(tradingAccount.AccountNo, cancellationToken);
        var dealsTask = GetDeals(tradingAccount.AccountNo, cancellationToken);
        var offlineOrdersTask = onePortService.GetOfflineOrdersByAccountNo(tradingAccount.AccountNo, cancellationToken);

        var sblOrdersTask = Task.FromResult(new List<SblOrder>());
        if (tradingAccount.SblEnabled)
            sblOrdersTask = sblOrderRepository.GetSblOrdersAsync(tradingAccountNo,
                FilterFactory.NewSblOrderFilters(filters), cancellationToken);

        await Task.WhenAll(ordersTask, dealsTask, offlineOrdersTask, sblOrdersTask);
        var onlineOrders = await ordersTask;
        var deals = await dealsTask;
        var offlineOrders = await offlineOrdersTask;
        var sblOrders = await sblOrdersTask;

        var result = new List<OrderInfo>();
        ProcessOrders(onlineOrders, result, tradingAccount.CustomerCode, deals, filters);
        ProcessOrders(offlineOrders, result, tradingAccount.CustomerCode, deals, filters);
        result.AddRange(sblOrders.Select(ApplicationFactory.NewSblOrder));

        if (result.Count == 0) return result;

        var symbols = result.Select(q => q.Symbol).Distinct().ToArray();

        var instrumentProfilesTask = TaskWrapper(async () =>
        {
            var instrumentsProfile = await marketService.GetInstrumentsProfile(symbols, cancellationToken);
            return instrumentsProfile.ToDictionary(i => i.Symbol, i => i);
        });
        var sblInstrumentsTask = TaskWrapper(async () =>
        {
            if (!tradingAccount.SblEnabled) return null;

            var sblInstruments = await instrumentRepository.GetSblInstruments(symbols, cancellationToken);
            return sblInstruments.ToDictionary(i => i.Symbol, i => i);
        });

        await Task.WhenAll(instrumentProfilesTask, sblInstrumentsTask);

        var instrumentProfiles = await instrumentProfilesTask;
        var sblInstruments = await sblInstrumentsTask;

        foreach (var order in result)
        {
            if (instrumentProfiles.TryGetValue(order.Symbol, out var profile))
                order.SetInstrumentProfile(profile);
            if (sblInstruments != null && sblInstruments.TryGetValue(order.Symbol, out var sblInstrument))
                order.SetInterestRate(sblInstrument.InterestRate);
        }

        return result;
    }

    private static async Task<T> TaskWrapper<T>(Func<Task<T>> func)
    {
        return await func();
    }

    private async Task<Dictionary<long, List<Deal>>> GetDeals(string accountNo, CancellationToken cancellationToken)
    {
        var deals = await onePortService.GetDealsByAccountNo(accountNo, cancellationToken);

        Dictionary<long, List<Deal>> result = new();

        foreach (var deal in deals)
        {
            if (!result.ContainsKey(deal.OrderNo)) result[deal.OrderNo] = new List<Deal>();
            result[deal.OrderNo].Add(deal);
        }

        return result;
    }

    private static void ValidateDates(DateOnly effectiveDateFrom, DateOnly effectiveDateTo, int? maxRange = null)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        if (effectiveDateFrom <= today &&
            effectiveDateTo <= today &&
            effectiveDateFrom <= effectiveDateTo &&
            (maxRange == null || effectiveDateTo.DayNumber - effectiveDateFrom.DayNumber <= maxRange))
            return;

        if (maxRange != null && effectiveDateTo.DayNumber - effectiveDateFrom.DayNumber > maxRange)
            throw new SetException(SetErrorCode.SE008);

        throw new SetException(SetErrorCode.SE009);
    }

    private async Task<TradingAccount> GetTradingAccount(Guid userId, string tradingAccountNo,
        CancellationToken cancellationToken = default)
    {
        var custCode = TradingAccountHelper.GetCustCodeBySetTradingAccountNo(tradingAccountNo);

        if (custCode == null) throw new SetException(SetErrorCode.SE001);

        var custCodes = await userService.GetCustomerCodesByUserId(userId);

        if (!custCodes.Contains(custCode)) throw new SetException(SetErrorCode.SE101);

        var tradingAccounts = await onboardService.GetSetTradingAccountsByUserIdAsync(userId, custCode, cancellationToken);
        var tradingAccount = tradingAccounts.Find(q => q.TradingAccountNo == tradingAccountNo);

        if (tradingAccount == null) throw new SetException(SetErrorCode.SE102);

        return tradingAccount;
    }

    private static void ValidateTradingAccountType(TradingAccount tradingAccount, TradingAccountType[] supportedTypes)
    {
        if (!supportedTypes.Contains(tradingAccount.TradingAccountType)) throw new SetException(SetErrorCode.SE001);
    }
}
