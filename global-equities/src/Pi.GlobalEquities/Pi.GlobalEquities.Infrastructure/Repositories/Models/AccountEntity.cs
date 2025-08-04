using MongoDB.Bson.Serialization.Attributes;

namespace Pi.GlobalEquities.Infrastructure.Repositories.Models;

public class AccountEntity
{
    public int? Version { get; private init; } = 1;

    /// <summary>
    /// Pi account id, Maps to user account in user service
    /// </summary>
    [BsonId]
    public string Id { get; init; }
    public string UserId { get; init; }
    public string CustCode { get; init; }
    public string TradingAccountNo { get; init; }  //```custCode-2```
    public string VelexaAccount { get; init; }
    public DateTime UpdatedAt { get; init; }
}
