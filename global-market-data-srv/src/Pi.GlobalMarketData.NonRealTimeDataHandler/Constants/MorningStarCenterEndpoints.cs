namespace Pi.GlobalMarketData.NonRealTimeDataHandler.constants;

public class Expiration
{
    private Expiration(string value)
    {
        Value = value;
    }

    public string Value { get; private set; }

    public static Expiration D90 => new("90D");

    public static Expiration D60 => new("60D");

    public static Expiration D30 => new("30D");
}

public static class MorningStarCenterEndpoints
{
    public const string apiCenter = "/service/mf";
    public const string apiCenterV2 = "/v2/service/mf";
    public const string commonQuery = "/{0}/{1}?format={2}&accesscode={3}";
    public const string accessCodeQuery = "{2}?format=json&account_code={0}&account_password={1}";
    public const string createAccessCode = "v2/service/account/CreateAccesscode/" + accessCodeQuery;

    public const string queryAccessCode =
        "v2/service/account/AccesscodeBasicInfo/" + accessCodeQuery;

    public const string deleteAccessCode = "v2/service/account/DeleteAccesscode/" + accessCodeQuery;
    public const string NetAssets = apiCenterV2 + "/NetAssets" + commonQuery;
    public const string CurrentPrice = apiCenterV2 + "/CurrentPrice" + commonQuery;
    public const string DailyPerformance = apiCenterV2 + "/DailyPerformance" + commonQuery;
    public const string FundShareClassBasicInfo =
        apiCenterV2 + "/FundShareClassBasicInfo" + commonQuery;
    public const string Yields = apiCenterV2 + "/Yields" + commonQuery;
    public const string ProspectusFees = apiCenterV2 + "/ProspectusFees" + commonQuery;
    public const string InvestmentCriteria = apiCenterV2 + "/InvestmentCriteria" + commonQuery;
}
