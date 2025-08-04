using MongoDB.Bson.Serialization.Attributes;
using Pi.FundMarketData.Constants;

namespace Pi.FundMarketData.DomainModels;

public class AmcProfile
{
    [BsonId]
    public string Code { get; init; }
    public string Name { get; init; }

    [BsonIgnore]
    public string Logo
    {
        get
        {
            // TODO remove later
            // https://pifinancial.atlassian.net/browse/BUGA-667
            // https://pifinancial.atlassian.net/browse/BUGA-671
            if (Code == "MFC")
                return Path.Combine(StaticUrl.AmcLogoCdnServerUrl, "FIX", Code + ".svg");

            return Path.Combine(StaticUrl.AmcLogoCdnServerUrl, Code + ".svg");
        }
    }
}
