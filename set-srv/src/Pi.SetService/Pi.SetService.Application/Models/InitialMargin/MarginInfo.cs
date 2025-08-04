namespace Pi.SetService.Application.Models.InitialMargin;

public record MarginRateInfo
{
    public required string MarginCode { get; init; }
    public required decimal MarginRate { get; init; }
}

public record MarginInstrumentInfo
{
    public required string Symbol { get; init; }
    public required string MarginCode { get; init; }
    public required bool IsTurnoverList { get; set; }
}
