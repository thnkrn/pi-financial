using Pi.Common.SeedWork;
using Pi.User.Domain.AggregatesModel.TradeAccountAggregate;

namespace Pi.User.Domain.AggregatesModel.UserAccountAggregate;

public class UserAccount(string id) : Entity<string>(id), IAggregateRoot, IAuditableEntity
{
    public UserAccountType UserAccountType { get; set; }
    public Guid UserId { get; init; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public ICollection<TradeAccount> TradeAccounts { get; } = new List<TradeAccount>();
}