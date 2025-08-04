namespace Pi.SetMarketData.SmartIntegration.Configurations;

public class BatchUpdateOptions
{
    public int BatchSize { get; set; } = 10000;
    public int Delay { get; set; } = 2000;
}