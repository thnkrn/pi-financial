using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Pi.GlobalEquities.Models;

namespace Pi.GlobalEquities.Repositories.Models;

public class ProviderInfoEntity
{
    [BsonRepresentation(BsonType.String)]
    public Provider ProviderName { get; init; }
    public string AccountId { get; init; }
    public string OrderId { get; init; }
    public string ModificationId { get; init; }
    public string Status { get; init; }
    public string StatusReason { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime ModifiedAt { get; init; }
}
