namespace Pi.SetMarketData.Infrastructure.Interfaces.Helpers;
public interface IInstrumentHelper 
{
    Task<string> GetFriendlyName(string symbol, string instrumentCategory);
}