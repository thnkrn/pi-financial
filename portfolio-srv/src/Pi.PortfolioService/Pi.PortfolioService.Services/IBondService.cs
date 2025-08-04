namespace Pi.PortfolioService.Services;

public interface IBondService
{
    Task<IEnumerable<PortfolioAccount>> GetAccountsOverview(string userId,
        CancellationToken ct);
}