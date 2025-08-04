namespace Pi.SetMarketDataRealTime.Domain.SeedWork;

public interface IRepository
{
    IUnitOfWork UnitOfWork { get; }
}