namespace Pi.BackofficeService.Domain.SeedWork;

public interface IRepository
{
    IUnitOfWork UnitOfWork { get; }
}
