namespace Pi.GlobalEquities.Worker.Configs;

public class FeatureApiConfig
{
    public const string Name = "GrowthBook";
    public string Host { get; init; }
    public string ApiKey { get; init; }
    public string ProjectId { get; init; }
}
