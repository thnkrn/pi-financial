using Pi.SetMarketDataRealTime.DataHandler.Exceptions;
using Pi.SetMarketDataRealTime.DataHandler.Interfaces.SoupBinTcp;
using Pi.SetMarketDataRealTime.Infrastructure.Interfaces.SoupBinTcp;

namespace Pi.SetMarketDataRealTime.DataHandler.Services.SoupBinTcp;

public class ClientFactory(
    IServiceProvider serviceProvider,
    ILogger<ClientFactory> logger) : IClientFactory
{
    public IClient CreateClient()
    {
        try
        {
            logger.LogDebug("Created new IClient instance");
            return serviceProvider.GetRequiredService<IClient>();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create new IClient instance");
            throw new SubscriptionServiceException("Failed to create new IClient instance", ex);
        }
    }
}