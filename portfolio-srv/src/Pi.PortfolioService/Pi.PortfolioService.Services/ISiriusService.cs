namespace Pi.PortfolioService.Services;

public interface ISiriusService
{
    public Task<PortfolioSummary> GetByToken(string sid, Guid deviceId, string valueUnit,
        CancellationToken ct = default);
}
