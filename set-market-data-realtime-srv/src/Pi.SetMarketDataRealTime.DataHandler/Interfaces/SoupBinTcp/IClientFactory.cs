using Pi.SetMarketDataRealTime.Infrastructure.Interfaces.SoupBinTcp;

namespace Pi.SetMarketDataRealTime.DataHandler.Interfaces.SoupBinTcp;

public interface IClientFactory
{
    IClient CreateClient();
}