namespace Pi.GlobalMarketData.Infrastructure.Interfaces.Helpers;
public interface IVelexaApiHelper
{
    public Task<string> getMinimumOrderSize(string symbol, string venue);
}