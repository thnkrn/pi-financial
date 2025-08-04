namespace Pi.PortfolioService.Services;

public interface IFundService
{
    public Task<IEnumerable<PortfolioAccount>> GetPortfolioAccounts(Guid userId, CancellationToken ct = default);
    public Task<IEnumerable<PortfolioAccount>> GetPortfolioAccountsOld(Guid userId, CancellationToken ct = default);
}
