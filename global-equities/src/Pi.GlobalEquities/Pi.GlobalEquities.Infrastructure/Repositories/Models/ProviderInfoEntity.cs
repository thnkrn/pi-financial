using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Pi.GlobalEquities.Models;

namespace Pi.GlobalEquities.Infrastructure.Repositories.Models;

public class ProviderInfoEntity
{
    [BsonRepresentation(BsonType.String)]
    public Provider ProviderName { get; init; }
    public required string AccountId { get; init; }
    public required string OrderId { get; init; }
    public required string ModificationId { get; init; }
    public required string Status { get; init; }
    public string? StatusReason { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime ModifiedAt { get; init; }
}
