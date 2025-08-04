namespace Pi.SetService.Application.Options;


public record SetTradingOptions
{
    public const string Options = "SetTradingOptions";
    public required string EnterId { get; init; }
    public string NormalStartTime { get; init; } = "07:35:00";
    public string NormalEndTime { get; init; } = "16:40:00";
    public string MaintenanceStartTime { get; init; } = "06:00:00";
    public string MaintenanceEndTime { get; init; } = "08:35:00";
}
