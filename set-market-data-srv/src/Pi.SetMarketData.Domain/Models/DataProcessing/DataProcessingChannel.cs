namespace Pi.SetMarketData.Domain.Models.DataProcessing;

public enum DataProcessingChannel
{
    MongoDb,
    TimescaleDb,
    Redis,
    Both
}