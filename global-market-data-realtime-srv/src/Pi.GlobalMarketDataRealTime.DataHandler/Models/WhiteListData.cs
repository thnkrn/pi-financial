namespace Pi.GlobalMarketDataRealTime.DataHandler.Models;

// ReSharper disable PropertyCanBeMadeInitOnly.Global
public class WhiteListData
{
    public string? Symbol { get; set; } = string.Empty;
    public string? SecurityId { get; set; } = string.Empty;
    public string? Exchange { get; set; } = string.Empty;
    public string? SecurityIdSource { get; set; } = string.Empty;
}