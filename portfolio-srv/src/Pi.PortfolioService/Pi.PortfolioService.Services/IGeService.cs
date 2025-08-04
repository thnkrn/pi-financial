namespace Pi.PortfolioService.Services;

public interface IGeService
{
    Task<IEnumerable<PortfolioAccount>> GetAccounts(string userId, string currency,
        CancellationToken ct);
}