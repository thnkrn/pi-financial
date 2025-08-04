using MongoDB.Bson.Serialization.Attributes;

namespace Pi.GlobalEquities.DomainModels;

public class WorkerJob<T>
{
    [BsonId]
    public string Name { get; init; }
    public T Data { get; init; }
}
