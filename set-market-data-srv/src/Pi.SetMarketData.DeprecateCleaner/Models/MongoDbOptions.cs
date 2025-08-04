namespace Pi.SetMarketData.DeprecateCleaner.Models;

public class MongoDbOptions
{
    public const string Options = "MongoDB";

    public required string ConnectionString { get; set; }
    public required string Database { get; set; }
    public required string Collection { get; set; }
}
