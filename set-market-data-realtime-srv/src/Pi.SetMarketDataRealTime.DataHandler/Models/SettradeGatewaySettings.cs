// ReSharper disable InconsistentNaming

namespace Pi.SetMarketDataRealTime.DataHandler.Models;

public class SettradeGatewaySettings
{
    public List<ServerConfig?>? SERVERS { get; set; }
    public int RECONNECT_DELAY_MS { get; set; }
    public int FAIL_OVER_DELAY_MS { get; set; }
}