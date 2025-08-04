using MongoDB.Bson.Serialization.Conventions;

namespace Pi.FundMarketData.Repositories;

public class MongoDbConfig
{
    public string ConnectionString { get; init; }
    public string Database { get; init; }

    public const string FundWhitelistsColName = "FundWhitelists";
    public const string FundProfilesColName = "FundProfiles";
    public const string FundTradeDataColName = "FundTradeData";
    public const string HistoricalNavsColName = "HistoricalNavs";
    public const string ExtFundCategoriesColName = "ExtFundCategories";
    public const string FundCategoriesColName = "FundCategories";
    public const string AmcProfilesColName = "AmcProfiles";
    public const string BusinessHolidaysColName = "BusinessHolidays";

    public const string SymbolIndexName = "Symbol_1";
    public const string FundSearchIndexName = "Search_Symbol_Name";

    public static void IgnoreExtraElements()
    {
        var pack = new ConventionPack { new IgnoreExtraElementsConvention(true) };
        ConventionRegistry.Register("IgnoreExtraElements", pack, t => true);
    }
}
