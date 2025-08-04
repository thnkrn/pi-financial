using Pi.Common.SeedWork;

namespace Pi.User.Domain.AggregatesModel.SuitabilityTestAggregate;

public class SuitabilityTest(Guid id) : Entity<Guid>(id), IAggregateRoot
{
    public required string Grade { get; set; }
    public int Score { get; set; }
    public required string Version { get; set; }
    public DateTime ReviewDate { get; set; }
    public DateTime ExpiredDate { get; set; }
    public Guid UserId { get; init; }
}