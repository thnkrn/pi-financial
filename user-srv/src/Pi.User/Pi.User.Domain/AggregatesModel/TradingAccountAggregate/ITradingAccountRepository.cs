namespace Pi.User.Domain.AggregatesModel.TradingAccountAggregate;

public interface ITradingAccountRepository
{
    Task<IList<TradingAccount>> GetTradingAccountsAsync(
        string customerCode,
        CancellationToken cancellationToken = default);
    Task<IList<TradingAccount>> GetTradingAccountsAsync(IEnumerable<string> customerCodes, CancellationToken cancellationToken = default);
}
