using Pi.Common.SeedWork;

namespace Pi.User.Domain.AggregatesModel.TradeAccountAggregate;

public interface ITradeAccountRepository : IRepository<TradeAccount>
{
    Task AddAsync(TradeAccount tradeAccount);
}