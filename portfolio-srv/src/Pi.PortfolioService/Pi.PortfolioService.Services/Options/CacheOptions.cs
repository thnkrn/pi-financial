namespace Pi.PortfolioService.Services.Options;

public record CacheOptions
{
    public static readonly string Options = "Cache";
    public bool Enabled { get; set; } = false;
}
