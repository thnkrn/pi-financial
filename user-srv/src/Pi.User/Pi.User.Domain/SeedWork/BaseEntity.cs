using Pi.Common.SeedWork;

namespace Pi.User.Domain.SeedWork;

public class BaseEntity : Entity, IAggregateRoot
{
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}