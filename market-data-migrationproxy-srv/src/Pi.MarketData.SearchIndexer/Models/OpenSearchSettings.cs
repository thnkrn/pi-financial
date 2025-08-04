namespace Pi.MarketData.SearchIndexer.Models;

public class OpenSearchSettings
{
    public required string Host { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string DefaultIndex { get; set; }
}