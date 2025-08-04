using Pi.Common.SeedWork;

namespace Pi.User.Domain.AggregatesModel.KycAggregate;

public class Kyc(Guid id) : Entity<Guid>(id), IAggregateRoot
{
    public DateTime ReviewDate { get; set; }
    public DateTime ExpiredDate { get; set; }
    public Guid UserId { get; init; }
}