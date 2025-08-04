namespace Pi.GlobalMarketData.Domain.ConstantConfigurations;

public static class CacheKey
{
    public const string PriceInfo = "price-info-";
    public const string MarketStatus = "market-status-";
    public const string PublicTrade = "public-trade-";
    public const string OrderBook = "order-book-";
    public const string OrderBookMarket = "order-book-market-";
    public const string InstrumentDetail = "instrument-detail-";
    public const string StreamingBody = "global-streaming-body-";
    public const string GeStreamingBody = "{ge-str-streaming-body-}";
    public const string MarketDirectory = "market-directory-";
    public const string SymbolVenue = "symbol-venue-";
    public const string GeInstrument = "ge-instrument";
    public const string GeVenueMapping = "ge-venue-mapping-";
    public const string MorningStarStocks = "morning-star-stocks-";
    public const string MorningStarEtfs = "morning-star-etfs-";
    public const string WhiteList = "white-list-";
    public const string RankingItem = "ranking-item";
    public const string MarketSchedule = "market-schedule:";
    public const string MarketSessionStatus = "market-session-status:";
    public const string WhiteListProcessor = "whitelist-data-";
    public const string GeMarketSchedule = "ge-market-schedule-list-";
}
