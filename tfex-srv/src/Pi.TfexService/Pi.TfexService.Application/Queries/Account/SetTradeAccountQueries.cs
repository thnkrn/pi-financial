using Microsoft.Extensions.Logging;
using Pi.Client.Sirius.Model;
using Pi.Common.Features;
using Pi.TfexService.Application.Models;
using Pi.TfexService.Application.Providers;
using Pi.TfexService.Application.Queries.Market;
using Pi.TfexService.Application.Services.It;
using Pi.TfexService.Application.Services.MarketData;
using Pi.TfexService.Application.Services.SetTrade;
using Pi.TfexService.Application.Services.UserService;
using Pi.TfexService.Application.Services.Wallet;
using Pi.TfexService.Application.Utils;
using Pi.TfexService.Domain.Exceptions;
using Pi.TfexService.Domain.Models.InitialMargin;

namespace Pi.TfexService.Application.Queries.Account;

public class SetTradeAccountQueries(
    ISetTradeService setTradeService,
    IMarketDataService marketDataService,
    IUserService userService,
    IUserV2Service userV2Service,
    IItService itService,
    IFeatureService featureService,
    IDateTimeProvider dateTimeProvider,
    IInitialMarginRepository initialMarginRepository,
    IMarketDataQueries marketDataQueries,
    IWalletService walletService,
    ILogger<SetTradeAccountQueries> logger)
    : ISetTradeAccountQueries
{
    private static readonly HashSet<char> ValidMonthCodes = ['F', 'G', 'H', 'J', 'K', 'M', 'N', 'Q', 'U', 'V', 'X', 'Z'];

    public async Task<AccountInfoDto> GetAccountInfo(string userId, string accountCode, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(accountCode))
            throw new ArgumentException("Account code cannot be null or empty.", nameof(accountCode));

        AccountInfo accountInfo;
        PortfolioResponse portfolio;
        try
        {
            if (featureService.IsOn(Features.GetTfexDataFromIt)) throw new NotSupportedException("Forced get data from IT");
            (accountInfo, portfolio) = await ExecuteServiceCalls(accountCode, cancellationToken);
        }
        catch (Exception e)
        {
            if (e.InnerException is SetTradeNotFoundException)
            {
                // Fallback to get data from Wallet if Account Not Found
                try
                {
                    var excessEquity = await walletService.GetWalletBalance(userId, accountCode, cancellationToken);
                    return new AccountInfoDto(0, 0, 0, excessEquity, 0, 0, 0, 0, 0, 0, 0, "");
                }
                catch
                {
                    // Rethrow original exception if fallback fails
                    throw e;
                }
            }

            // Fallback to get data from IT Service if SetTrade is not available
            CheckInvalidDateForItFallback(e);
            accountInfo = await GetAccountInfoFromItService(accountCode, cancellationToken);
            portfolio = await GetPortfolioFromItService(accountCode, cancellationToken);
        }

        return MapToAccountInfoDto(accountInfo, portfolio);
    }

    public async Task<List<PortfolioDto>> GetPortfolio(string accountCode, string? sid, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(accountCode))
            throw new ArgumentException("Account code cannot be null or empty.", nameof(accountCode));

        PortfolioResponse portfolio;
        try
        {
            if (featureService.IsOn(Features.GetTfexDataFromIt)) throw new NotSupportedException("Forced get data from IT");
            portfolio = await setTradeService.GetPortfolio(accountCode, cancellationToken);
        }
        catch (Exception exception)
        {
            // If Account Not Found Return Empty List
            if (exception is SetTradeNotFoundException)
            {
                return [];
            }
            CheckInvalidDateForItFallback(exception);
            portfolio = await GetPortfolioFromItService(accountCode, cancellationToken);
        }
        var symbols = portfolio.PortfolioList.Select(p => p.Underlying == "SET50" ? "S50" : p.Underlying).Distinct().ToList();
        var initialMargins = await initialMarginRepository.GetInitialMarginList(symbols, cancellationToken);

        var marketData = new List<Ticker>();
        var series = portfolio.PortfolioList.Select(p => p.Symbol).Distinct().ToList();
        try
        {
            var data = await marketDataService.GetTicker(sid, series);
            marketData.AddRange(data);
        }
        catch (Exception ex)
        {
            logger.LogError("SetTradeAccountQueries: Cannot Get Market Data on {Series} for Portfolio: {Exception}", series, ex);
        }

        return portfolio.PortfolioList.Select(p => MapToPortfolioDto(p, initialMargins, marketData)).ToList();
    }

    public async Task<List<PortfolioSummaryDto>> GetPortfolioByUserId(string userId, CancellationToken cancellationToken)
    {
        var userTradingAccountInfo = featureService.IsOn(Features.MigrateUserV2)
            ? await userV2Service.GetTradingAccounts(userId)
            : await userService.GetTradingAccounts(userId);
        var accountCodeList = userTradingAccountInfo.SelectMany(u => u.TradingAccounts
            .Where(t => t.TradingAccountNo.EndsWith($"0")))
            .Select(t => t.TradingAccountNo.Replace("-", "")).ToList();
        List<PortfolioSummaryDto> list = [];

        foreach (var accountCode in accountCodeList)
        {
            try
            {
                var (accountInfo, portfolio) = await ExecuteServiceCalls(accountCode, cancellationToken);

                list.Add(new PortfolioSummaryDto(
                    accountCode[..^1],
                    true,
                    portfolio.TotalPortfolio.MarketValue,
                    portfolio.TotalPortfolio.PercentUnrealizePl,
                    accountCode,
                    portfolio.TotalPortfolio.UnrealizePl,
                    accountInfo.CashBalance,
                    accountInfo.Equity
                ));
            }
            catch (Exception)
            {
                list.Add(new PortfolioSummaryDto(
                    accountCode[..^1],
                    false,
                    0,
                    0,
                    accountCode,
                    0,
                    0,
                    0));
            }
        }

        return list;
    }

    public async Task<SeriesInfoDto> GetSeriesInfo(string accountCode, string sid, string series, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(accountCode))
        {
            throw new ArgumentException("Invalid Account Code");
        }

        if (string.IsNullOrWhiteSpace(series))
        {
            throw new ArgumentException("Invalid Symbol");
        }

        if (!ValidMonthCodes.Contains(GetMonthCode(series)))
        {
            throw new ArgumentException("Invalid Expiration month");
        }

        var (accountInfo, portfolio) = await ExecuteServiceCalls(accountCode, cancellationToken);
        if (accountInfo == null || portfolio == null)
        {
            throw new SetTradeNotFoundException("Account or Portfolio Not Found");
        }

        var positionInfo = portfolio.PortfolioList.Find(p => string.Equals(p.Symbol, series, StringComparison.CurrentCultureIgnoreCase));

        var availableUnit = 0;
        if (positionInfo != null)
        {
            var side = GetSide(positionInfo);
            availableUnit = side == Side.Long ? positionInfo.AvailableLongPosition : positionInfo.AvailableShortPosition;
        }

        var marketData = await marketDataQueries.GetMarketData(sid, series, cancellationToken);
        if (marketData == null)
        {
            throw new NotSupportedException("Symbol Not Found In Market Data");
        }

        return new SeriesInfoDto(
            series,
            accountInfo.ExcessEquity,
            availableUnit,
            marketData.InstrumentCategory,
            marketData.TickSize,
            marketData.LotSize,
            marketData.Multiplier,
            marketData.MultiplierType,
            marketData.MultiplierUnit
        );
    }

    private async Task<(AccountInfo accountInfo, PortfolioResponse portfolio)> ExecuteServiceCalls(string accountCode, CancellationToken cancellationToken)
    {
        var accountInfoTask = setTradeService.GetAccountInfo(accountCode, cancellationToken);
        var portfolioTask = setTradeService.GetPortfolio(accountCode, cancellationToken);

        await Task.WhenAll(accountInfoTask, portfolioTask).WithAggregatedExceptions();

        var accountInfo = await accountInfoTask;
        var portfolio = await portfolioTask;

        return (accountInfo, portfolio);
    }

    private static AccountInfoDto MapToAccountInfoDto(AccountInfo accountInfo, PortfolioResponse portfolio)
    {
        return new AccountInfoDto(
            portfolio.TotalPortfolio.MarketValue,
            portfolio.TotalPortfolio.UnrealizePl,
            portfolio.TotalPortfolio.PercentUnrealizePl,
            accountInfo.ExcessEquity,
            accountInfo.CreditLine,
            accountInfo.Equity,
            accountInfo.TotalMarginRequire,
            accountInfo.TotalMaintenanceMargin,
            accountInfo.TotalForceMargin,
            accountInfo.CallForceMargin,
            accountInfo.CallForceMarginMM,
            accountInfo.CallForceFlag
        );
    }

    private PortfolioDto MapToPortfolioDto(Portfolio portfolio, List<InitialMargin> initialMargins, List<Ticker> marketData)
    {
        var side = GetSide(portfolio);
        var isLong = side == Side.Long;

        var symbolData = marketData.Find(t => t.Symbol == portfolio.Symbol);
        var logo = symbolData?.Logo ?? string.Empty;
        var instrumentCategory = string.IsNullOrEmpty(symbolData?.InstrumentCategory) ? InstrumentCategory.Others.ToString() : symbolData.InstrumentCategory;
        var im = initialMargins.Find(a =>
        {
            var underlying = portfolio.Underlying == "SET50" ? "S50" : portfolio.Underlying;
            return a.Symbol == underlying;
        });

        var (gainLoss, gainLossPercentage) = GetGainLoss(portfolio, isLong);

        return new PortfolioDto(
            portfolio.Symbol,
            side,
            isLong ? portfolio.ActualLongPosition : portfolio.ActualShortPosition,
            isLong ? portfolio.AvailableLongPosition : portfolio.AvailableShortPosition,
            portfolio.MarketPrice,
            isLong ? portfolio.LongMarketValue : portfolio.ShortMarketValue,
            im != null ? im.ImOutright * (isLong ? portfolio.ActualLongPosition : portfolio.ActualShortPosition) : null,
            isLong ? portfolio.LongAvgCost : portfolio.ShortAvgCost,
            isLong ? portfolio.LongAmountByCost : portfolio.ShortAmountByCost,
            portfolio.Multiplier,
            portfolio.Currency,
            gainLoss,
            gainLossPercentage,
            portfolio.RealizedPl,
            logo,
            instrumentCategory
        );
    }

    private (decimal gainLoss, decimal gainLossPercentage) GetGainLoss(Portfolio portfolio, bool isLong)
    {
        if (featureService.IsOn(Features.UseTfexPlByCost))
        {
            return isLong
                ? (portfolio.LongUnrealizePlByCost, portfolio.LongPercentUnrealizePlByCost)
                : (portfolio.ShortUnrealizePlByCost, portfolio.ShortPercentUnrealizePlByCost);
        }

        return isLong
            ? (portfolio.LongUnrealizePl, portfolio.LongPercentUnrealizePl)
            : (portfolio.ShortUnrealizePl, portfolio.ShortPercentUnrealizePl);
    }

    private static Side GetSide(Portfolio portfolio)
    {
        return (portfolio.HasLongPosition, portfolio.HasShortPosition) switch
        {
            (true, false) => Side.Long,
            (false, true) => Side.Short,
            (true, true) => portfolio.ActualLongPosition >= portfolio.ActualShortPosition ? Side.Long : Side.Short,
            _ => throw new InvalidOperationException("Short and Long position cannot be in the same Portfolio")
        };
    }

    private static char GetMonthCode(string series)
    {
        // case: SET50 Index Options e.g. S50U24C700
        if (series.StartsWith("S50") && series.Length > 6)
        {
            return series[..6][^3..][0];
        }

        return series[^3..][0];
    }

    private async Task<AccountInfo> GetAccountInfoFromItService(string accountCode, CancellationToken cancellationToken)
    {
        var tfexData = await itService.GetTfexData(accountCode[..^1], cancellationToken);

        return new AccountInfo(
            0,
            (decimal)tfexData.Data.ExcessEquity!,
            0,
            (decimal)tfexData.Data.Equity!,
            0,
            0,
            0,
            null,
            0,
            0,
            0,
            0,
            0,
            null
        );
    }

    private async Task<PortfolioResponse> GetPortfolioFromItService(string accountCode,
        CancellationToken cancellationToken)
    {
        var tfexData = await itService.GetTfexData(accountCode[..^1], cancellationToken);
        var portfolioList = (tfexData.Data.PositionList ?? []).Select(CreatePortfolio).ToList();
        var percentUnrealizePl = tfexData.Data.MktVal is null or 0 ? 0 : (decimal)tfexData.Data.UnrealizePl! / (decimal)tfexData.Data.MktVal! * 100;

        return new PortfolioResponse(
            portfolioList,
            new TotalPortfolio(
                0,
                (decimal)tfexData.Data.MktVal!,
                0,
                (decimal)tfexData.Data.UnrealizePl!,
                0,
                0,
                0,
                percentUnrealizePl,
                0,
                0)
            );
    }

    private static Portfolio CreatePortfolio(PositionItem x)
    {
        var hasLongPosition = x.LongUnit != 0;
        var hasShortPosition = x.ShortUnit != 0;
        var longAvgCost = hasLongPosition ? x.SettlementPrice : 0;
        var shortAvgCost = hasShortPosition ? x.SettlementPrice : 0;
        var marketPrice = x.SettlementPrice;
        var longAmountByCost = hasLongPosition ? x.AvgCostThbLong * x.Multiplier : 0;
        var shortAmountByCost = hasShortPosition ? x.AvgCostThbShort * x.Multiplier : 0;
        var longMarketValue = hasLongPosition ? x.SettlementPrice * x.Multiplier : 0;
        var shortMarketValue = hasShortPosition ? x.SettlementPrice * x.Multiplier : 0;
        var longUnrealizePlByCost = hasLongPosition ? x.AccMtm : 0;
        var shortUnrealizePlByCost = hasShortPosition ? -x.AccMtm : 0;
        var longPercentUnrealizePlByCost = hasLongPosition ? x.AccMtm / (x.SettlementPrice * x.Multiplier) * 100 : 0;
        var shortPercentUnrealizePlByCost = hasShortPosition ? -x.AccMtm / (x.SettlementPrice * x.Multiplier) * 100 : 0;

        return new Portfolio(
            "",
            "",
            x.ShareCode,
            "",
            SecurityType.Futures,
            DateOnly.FromDateTime(DateTime.UtcNow),
            x.Multiplier,
            GetCurrency(x.FxCode),
            0,
            "",
            hasLongPosition,
            0,
            x.LongUnit,
            x.LongUnit,
            0,
            0,
            0,
            longAvgCost,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            hasShortPosition,
            0,
            x.ShortUnit,
            x.ShortUnit,
            0,
            0,
            0,
            shortAvgCost,
            0,
            0,
            0,
            0,
            0,
            0,
            marketPrice,
            0,
            0,
            0,
            0,
            0,
            0,
            shortAmountByCost,
            longAmountByCost,
            0,
            0,
            longUnrealizePlByCost,
            longUnrealizePlByCost,
            longPercentUnrealizePlByCost,
            longPercentUnrealizePlByCost,
            0,
            longMarketValue,
            shortUnrealizePlByCost,
            shortPercentUnrealizePlByCost,
            shortUnrealizePlByCost,
            shortPercentUnrealizePlByCost,
            0,
            shortMarketValue,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0
        );
    }

    private static Currency GetCurrency(string fxCode)
    {
        return fxCode switch
        {
            "THB" => Currency.THB,
            "USD" => Currency.USD,
            _ => throw new NotSupportedException("Currency not supported")
        };
    }

    private void CheckInvalidDateForItFallback(Exception e)
    {
        var now = dateTimeProvider.GetThDateTimeNow();
        if (now.DayOfWeek is not (DayOfWeek.Saturday or DayOfWeek.Sunday) && !featureService.IsOn(Features.GetTfexDataFromIt))
        {
            logger.LogError(e, "Unable to connect to SetTrade with Exception: {Message}", e.Message);
            throw new NotSupportedException("SetTrade internal server error");
        }
    }
}