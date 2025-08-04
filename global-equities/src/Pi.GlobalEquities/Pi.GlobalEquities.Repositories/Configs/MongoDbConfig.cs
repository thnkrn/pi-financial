using MongoDB.Bson.Serialization.Conventions;

namespace Pi.GlobalEquities.Repositories.Configs;

public class MongoDbConfig
{
    public const string Name = "GlobalEquityDatabase";
    public string ConnectionString { get; init; }
    public TimeSpan Timeout { get; init; }
    public string Database { get; init; }

    public static void IgnoreExtraElements()
    {
        var pack = new ConventionPack { new IgnoreExtraElementsConvention(true) };
        ConventionRegistry.Register("IgnoreExtraElements", pack, t => true);
    }

    public static class OrderCollection
    {
        public static string Name = "Order";
        public static class Indexes
        {
            public const string OrderStatus = "OrderStatus";
            public const string OrderLastUpdate = "OrderLastUpdate";
        }
    }

    public static class AccountCollection
    {
        public static string Name = "TradingAccount";
        public static class Indexes
        {
            public const string UserId = "UserId";
            public const string VelexaAccount = "VelexaAccount";
        }
    }

    public static class WorkerJobCollection
    {
        public static string Name = "WorkerJob";
    }
}
