namespace Pi.PortfolioService.DomainServices;

public interface IPortfolioSummaryQueries
{
    Task<PortfolioSummary> GetPortfolioSummaryAsync(string sid, Guid deviceId, string userId, string valueUnit,
        CancellationToken ct = default);

    public Task<PortfolioSummary> GetPortfolioSummaryV2Async(
        string userId,
        string valueUnit,
        CancellationToken ct = default);
}
