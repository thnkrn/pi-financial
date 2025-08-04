using Pi.GlobalMarketData.Domain.Entities;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.GlobalMarketData.NonRealTimeDataHandler.interfaces;

public interface IGeDataRepository
{
    public IMongoService<GeVenueMapping> GeVenueMapping { get; }
    public IMongoService<GeInstrument> GeInstrument { get; }
    public IMongoService<WhiteList> Whitelist { get; }
    public IMongoService<MorningStarEtfs> MorningStarEtfs { get; }
    public IMongoService<MorningStarStocks> MorningStarStocks { get; }
}