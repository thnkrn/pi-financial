using Pi.Common.SeedWork;

namespace Pi.TfexService.Domain.SeedWork;

public class BaseEntity : Entity, IAggregateRoot
{
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}