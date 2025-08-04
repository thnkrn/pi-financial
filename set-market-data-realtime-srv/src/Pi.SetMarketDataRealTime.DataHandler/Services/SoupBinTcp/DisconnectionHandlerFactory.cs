using Pi.SetMarketDataRealTime.DataHandler.Exceptions;
using Pi.SetMarketDataRealTime.DataHandler.Interfaces.SoupBinTcp;

namespace Pi.SetMarketDataRealTime.DataHandler.Services.SoupBinTcp;

public class DisconnectionHandlerFactory(
    IServiceProvider serviceProvider,
    ILogger<ClientFactory> logger)
    : IDisconnectionHandlerFactory
{
    public IDisconnectionHandler CreateHandler()
    {
        try
        {
            logger.LogDebug("Created new IDisconnectionHandler instance");
            return serviceProvider.GetRequiredService<IDisconnectionHandler>();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create new IDisconnectionHandler instance");
            throw new SubscriptionServiceException("Failed to create new IDisconnectionHandler instance", ex);
        }
    }
}