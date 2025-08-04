namespace Pi.GlobalMarketDataRealTime.Domain.SeedWork;

public interface IRepository
{
    IUnitOfWork UnitOfWork { get; }
}