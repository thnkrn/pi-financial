using Serilog.Events;

namespace Pi.SetMarketDataWSS.Infrastructure.Services.Logging.Extensions;

public class LoggingOptions
{
    public string ServiceType { get; set; } = string.Empty;
    public string Component { get; set; } = string.Empty;
    public Dictionary<string, LogEventLevel>? AdditionalOverrides { get; set; }
    public Dictionary<string, string>? AdditionalProperties { get; set; }
}