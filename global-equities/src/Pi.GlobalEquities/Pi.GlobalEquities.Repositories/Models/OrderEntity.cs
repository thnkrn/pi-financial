#nullable enable
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Pi.GlobalEquities.DomainModels;
using Pi.GlobalEquities.Models;

namespace Pi.GlobalEquities.Repositories.Models;

public class OrderEntity
{
    public int? Version { get; private init; } = 2;
    [BsonId]
    public required string Id { get; init; }
    public string? GroupId { get; init; }
    public required string UserId { get; init; }
    public required string AccountId { get; init; }
    public required string Venue { get; init; }
    public required string Symbol { get; init; }

    [BsonRepresentation(BsonType.String)]
    public required OrderType OrderType { get; init; }

    [BsonRepresentation(BsonType.String)]
    public required OrderSide Side { get; init; }

    [BsonRepresentation(BsonType.String)]
    public required OrderDuration Duration { get; init; }
    public decimal Quantity { get; init; }
    public decimal? LimitPrice { get; init; }
    public decimal? StopPrice { get; init; }
    public IEnumerable<OrderFill>? Fills { get; init; }
    public required string Status { get; init; }
    public required ProviderInfoEntity ProviderInfo { get; init; }
    [BsonRepresentation(BsonType.String)]
    public required Channel Channel { get; init; }
    public required DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}
