namespace Pi.MarketData.MigrationProxy.API.Options;

public class HttpForwarderOptions
{
    public const string Options = "HttpForwarder";

    public string[] ForwardHeaders { get; set; } = [];
}
