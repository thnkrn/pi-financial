namespace Pi.GlobalEquities.Models;

public class OrderTagInfo
{
    public string UserId { get; init; }
    public string AccountId { get; init; }
    public string ProviderAccountId { get; init; }
    public OrderType? OrderType { get; init; }
}
