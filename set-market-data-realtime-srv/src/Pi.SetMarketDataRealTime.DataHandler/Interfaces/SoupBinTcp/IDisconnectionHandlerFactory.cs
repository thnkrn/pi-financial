namespace Pi.SetMarketDataRealTime.DataHandler.Interfaces.SoupBinTcp;

public interface IDisconnectionHandlerFactory
{
    IDisconnectionHandler CreateHandler();
}