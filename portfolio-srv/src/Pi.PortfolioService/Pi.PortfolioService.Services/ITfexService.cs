namespace Pi.PortfolioService.Services;

public record TfexPortfolioSummary(
    string CustCode,
    bool IsSuccess,
    decimal TotalMarketValue,
    decimal UpnlPercentage,
    string TradingAccountNo,
    decimal Upnl,
    decimal CashBalance,
    decimal TotalValue);

public interface ITfexService
{
    public Task<List<TfexPortfolioSummary>> GetPortfolioSummary(string userId, CancellationToken ct);
    public Task<IEnumerable<PortfolioAccount>> GetPortfolioAccount(string userId, CancellationToken ct);
}