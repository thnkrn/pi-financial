// ReSharper disable InconsistentNaming
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Pi.SetMarketDataRealTime.DataHandler.Models;

public class ServerConfig
{
    public string? NAME { get; set; }

    public Gateway? GLIMPSE_GATEWAY { get; set; }
    public Gateway? ITCH_GATEWAY { get; set; }
}