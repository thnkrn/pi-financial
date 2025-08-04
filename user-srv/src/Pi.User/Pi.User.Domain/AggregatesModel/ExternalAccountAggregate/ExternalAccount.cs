using Pi.Common.SeedWork;

namespace Pi.User.Domain.AggregatesModel.ExternalAccountAggregate;

public class ExternalAccount(Guid id) : Entity<Guid>(id), IAuditableEntity, IAggregateRoot
{
    public required string Value { get; set; }
    public int ProviderId { get; set; }
    public Guid TradeAccountId { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}