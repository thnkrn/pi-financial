namespace Pi.PortfolioService.Services;

public interface ISetService
{
    Task<IEnumerable<PortfolioAccount>> GetPortfolioAccounts(Guid userId, CancellationToken ct = default);
    Task<IEnumerable<PortfolioAccount>> GetPortfolioAccounts(Guid userId, string sid, CancellationToken ct = default);
}
