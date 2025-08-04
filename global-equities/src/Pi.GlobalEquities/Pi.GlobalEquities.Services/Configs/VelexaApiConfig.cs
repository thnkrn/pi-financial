namespace Pi.GlobalEquities.Services.Configs;

public class VelexaApiConfig
{
    public string Url { get; init; }
    public string Token { get; init; }
    public TimeSpan Timeout { get; init; }

    public const int OrderQueryLimit = 1000;
    public const int TransactionQueryLimit = 2000;
}
