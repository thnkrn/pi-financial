namespace Pi.MarketData.Search.Indexer.Models;

public class OpenSearchSettings
{
    public string Host { get; set; } = "";
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? DefaultIndex { get; set; }
}