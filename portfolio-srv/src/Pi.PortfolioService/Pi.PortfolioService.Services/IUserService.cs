namespace Pi.PortfolioService.Services;

public interface IUserService
{
    Task<IEnumerable<TradingAccount>> GetTradingAccountsAsync(Guid userId, CancellationToken ct = default);
}
