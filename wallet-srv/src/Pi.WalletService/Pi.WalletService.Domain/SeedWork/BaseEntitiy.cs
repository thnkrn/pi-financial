using Pi.Common.SeedWork;
namespace Pi.WalletService.Domain.SeedWork;

public class BaseEntity : Entity, IAggregateRoot
{
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}