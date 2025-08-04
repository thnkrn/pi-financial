using Pi.Common.SeedWork;

namespace Pi.User.Domain.AggregatesModel.KycAggregate;

public interface IKycRepository : IRepository<Kyc>
{
    Task AddAsync(Kyc kyc);
}