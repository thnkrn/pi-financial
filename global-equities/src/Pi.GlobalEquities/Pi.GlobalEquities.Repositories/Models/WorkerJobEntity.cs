using MongoDB.Bson.Serialization.Attributes;

namespace Pi.GlobalEquities.Repositories.Models;
public class WorkerJobEntity<T>
{
    [BsonId]
    public string Name { get; init; }
    public T Data { get; init; }
}
