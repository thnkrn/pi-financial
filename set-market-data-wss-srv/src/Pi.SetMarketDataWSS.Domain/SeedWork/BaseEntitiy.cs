namespace Pi.SetMarketDataWSS.Domain.SeedWork;

public class BaseEntity : IAggregateRoot
{
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}