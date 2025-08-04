namespace Pi.MarketData.Search.Domain.ConstantConfigurations;

public class MongoDbConfig
{
    public string? ConnectionString { get; set; }
    public string? Database { get; set; }
    public string? Collection { get; set; }
}

public class MongoDbSettings
{
    public MongoDbConfig? GE { get; set; }
    public MongoDbConfig? SET_TFEX { get; set; }
}