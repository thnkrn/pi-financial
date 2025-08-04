using Pi.TfexService.Application.Models;
using Pi.TfexService.Application.Services.SetTrade;

namespace Pi.TfexService.Application.Queries.Account;

public record AccountInfoDto(
    decimal MarketValue,
    decimal GainsInPortfolio,
    decimal PercentGainsInPortfolio,
    decimal ExcessEquity,
    decimal CreditLine,
    decimal Equity,
    decimal TotalMR,
    decimal TotalMM,
    decimal TotalFM,
    decimal CallForceMargin,
    decimal CallForceMarginMM,
    string? CallForceFlag
    );

public record PortfolioDto(
    string Symbol,
    Side Side,
    int ActualPosition,
    int AvailablePosition,
    decimal MarketPrice,
    decimal MarketValue,
    decimal? MarginRequire,
    decimal AverageCost,
    decimal Cost,
    decimal Multiplier,
    Currency Currency,
    decimal GainLoss,
    decimal GainLossPercentage,
    decimal GainLossRealized,
    string Logo,
    string InstrumentCategory);

public record PortfolioSummaryDto(
    string CustCode,
    bool IsSuccess,
    decimal TotalMarketValue,
    decimal UpnlPercentage,
    string TradingAccountNo,
    decimal Upnl,
    decimal CashBalance,
    decimal TotalValue);

public record SeriesInfoDto(
    string Symbol,
    decimal ExcessEquity,
    int AvailableUnit,
    string InstrumentCategory,
    decimal? TickSize = null,
    decimal? LotSize = null,
    decimal? Multiplier = null,
    MultiplierType? MultiplierType = null,
    MultiplierUnit? MultiplierUnit = null);

public interface ISetTradeAccountQueries
{
    public Task<AccountInfoDto> GetAccountInfo(string userId, string accountCode, CancellationToken cancellationToken);
    public Task<List<PortfolioDto>> GetPortfolio(string accountCode, string? sid, CancellationToken cancellationToken);
    public Task<List<PortfolioSummaryDto>> GetPortfolioByUserId(string userId, CancellationToken cancellationToken);
    public Task<SeriesInfoDto> GetSeriesInfo(string accountCode, string sid, string series, CancellationToken cancellationToken);
}