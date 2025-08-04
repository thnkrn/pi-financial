namespace Pi.GlobalMarketDataRealTime.Infrastructure.Interfaces.Redis;

public interface IDistributedCache
{
    string? GetString(string? key);
    void SetString(string key, string value);
}