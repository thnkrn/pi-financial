using Pi.GlobalMarketData.Domain.Entities;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Mongo;
using Pi.GlobalMarketData.NonRealTimeDataHandler.interfaces;

namespace Pi.GlobalMarketData.NonRealTimeDataHandler.Repository;

public class GeDataRepository(
    IMongoService<GeVenueMapping> geVenueMapping,
    IMongoService<GeInstrument> geInstrument,
    IMongoService<WhiteList> whitelist,
    IMongoService<MorningStarEtfs> morningStarEtfs,
    IMongoService<MorningStarStocks> morningStarStocks
) : IGeDataRepository
{
    public IMongoService<GeVenueMapping> GeVenueMapping => geVenueMapping;
    public IMongoService<GeInstrument> GeInstrument => geInstrument;
    public IMongoService<WhiteList> Whitelist => whitelist;
    public IMongoService<MorningStarEtfs> MorningStarEtfs => morningStarEtfs;
    public IMongoService<MorningStarStocks> MorningStarStocks => morningStarStocks;
}