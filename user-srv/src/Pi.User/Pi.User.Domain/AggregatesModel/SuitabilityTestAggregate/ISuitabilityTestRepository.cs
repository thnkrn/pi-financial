using Pi.Common.SeedWork;
using Pi.User.Domain.AggregatesModel.KycAggregate;

namespace Pi.User.Domain.AggregatesModel.SuitabilityTestAggregate;

public interface ISuitabilityTestRepository : IRepository<SuitabilityTest>
{
    Task AddAsync(Kyc kyc);
}