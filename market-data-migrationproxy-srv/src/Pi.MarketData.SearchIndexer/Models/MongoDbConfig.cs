namespace Pi.MarketData.SearchIndexer.Models;

public class MongoDbConfig
{
    public required string ConnectionString { get; set; }
    public required string Database { get; set; }
    public required string Collection { get; set; }
}

public class MongoDbSettings
{
    public required MongoDbConfig GE { get; set; }
    public required MongoDbConfig SET_TFEX { get; set; }
}