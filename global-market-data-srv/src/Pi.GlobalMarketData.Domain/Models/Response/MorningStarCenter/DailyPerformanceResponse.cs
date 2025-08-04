using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Pi.GlobalMarketData.Domain.Models.Response.MorningStarCenter;

public class DailyPerformanceResponse
{
    [BsonElement("data")]
    public List<Data<DailyPerformance>>? Data { get; set; }

    public static DailyPerformanceResponse? FromJson(string json) =>
        JsonConvert.DeserializeObject<DailyPerformanceResponse>(json);
}

public class DailyPerformance
{
    [JsonProperty("_name")]
    public string Name { get; set; } = string.Empty;
    public string MStarID { get; set; } = string.Empty;
    public string FundName { get; set; } = string.Empty;
    public string ExchangeID { get; set; } = string.Empty;
    public string Ticker { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
    public string CategoryCode { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string DayEndDate { get; set; } = string.Empty;
    public string DayEndNAV { get; set; } = string.Empty;
    public string NAVChange { get; set; } = string.Empty;
    public string NAVChangePercentage { get; set; } = string.Empty;
    public string DividendDate { get; set; } = string.Empty;
    public string Dividend { get; set; } = string.Empty;
    public string PremiumDiscountDate { get; set; } = string.Empty;
    public string PremiumDiscount { get; set; } = string.Empty;
    public string ReturnDate { get; set; } = string.Empty;
    public string Return1Day { get; set; } = string.Empty;
    public string Return1Week { get; set; } = string.Empty;
    public string Return1Mth { get; set; } = string.Empty;
    public string Return2Mth { get; set; } = string.Empty;
    public string Return3Mth { get; set; } = string.Empty;
    public string Return6Mth { get; set; } = string.Empty;
    public string Return1Yr { get; set; } = string.Empty;
    public string Return2Yr { get; set; } = string.Empty;
    public string Return3Yr { get; set; } = string.Empty;
    public string Return4Yr { get; set; } = string.Empty;
    public string Return5Yr { get; set; } = string.Empty;
    public string Return6Yr { get; set; } = string.Empty;
    public string Return7Yr { get; set; } = string.Empty;
    public string Return8Yr { get; set; } = string.Empty;
    public string Return9Yr { get; set; } = string.Empty;
    public string Return10Yr { get; set; } = string.Empty;
    public string Return15Yr { get; set; } = string.Empty;
    public string ReturnMTD { get; set; } = string.Empty;
    public string ReturnQTD { get; set; } = string.Empty;
    public string ReturnYTD { get; set; } = string.Empty;
    public string ReturnSinceInception { get; set; } = string.Empty;
    public string Rank1Day { get; set; } = string.Empty;
    public string Rank1Week { get; set; } = string.Empty;
    public string Rank1Mth { get; set; } = string.Empty;
    public string Rank2Mth { get; set; } = string.Empty;
    public string Rank3Mth { get; set; } = string.Empty;
    public string Rank6Mth { get; set; } = string.Empty;
    public string Rank1Yr { get; set; } = string.Empty;
    public string Rank3Yr { get; set; } = string.Empty;
    public string Rank5Yr { get; set; } = string.Empty;
    public string Rank10Yr { get; set; } = string.Empty;
    public string Rank15Yr { get; set; } = string.Empty;
    public string RankMTD { get; set; } = string.Empty;
    public string RankQTD { get; set; } = string.Empty;
    public string RankYTD { get; set; } = string.Empty;
    public string CategoryEndDate { get; set; } = string.Empty;
    public string CategoryReturn1Day { get; set; } = string.Empty;
    public string CategoryReturn1Week { get; set; } = string.Empty;
    public string CategoryReturn1Mth { get; set; } = string.Empty;
    public string CategoryReturn2Mth { get; set; } = string.Empty;
    public string CategoryReturn3Mth { get; set; } = string.Empty;
    public string CategoryReturn6Mth { get; set; } = string.Empty;
    public string CategoryReturn1Yr { get; set; } = string.Empty;
    public string CategoryReturn3Yr { get; set; } = string.Empty;
    public string CategoryReturn5Yr { get; set; } = string.Empty;
    public string CategoryReturn10Yr { get; set; } = string.Empty;
    public string CategoryReturn15Yr { get; set; } = string.Empty;
    public string CategoryReturn20Yr { get; set; } = string.Empty;
    public string CategoryReturnMTD { get; set; } = string.Empty;
    public string CategoryReturnQTD { get; set; } = string.Empty;
    public string CategoryReturnYTD { get; set; } = string.Empty;
    public string CategorySize1Day { get; set; } = string.Empty;
    public string CategorySize1Week { get; set; } = string.Empty;
    public string CategorySize1Mth { get; set; } = string.Empty;
    public string CategorySize2Mth { get; set; } = string.Empty;
    public string CategorySize3Mth { get; set; } = string.Empty;
    public string PricingFrequency { get; set; } = string.Empty;
    public string CumulativeReturn3Yr { get; set; } = string.Empty;
    public string CumulativeReturn5Yr { get; set; } = string.Empty;
    public string CumulativeReturn10Yr { get; set; } = string.Empty;
    public string CumulativeReturn15Yr { get; set; } = string.Empty;
    public string CumulativeReturnSinceInception { get; set; } = string.Empty;
    public string PotentialCapitalGain { get; set; } = string.Empty;
    public string RecordDate { get; set; } = string.Empty;
    public string AverageSpreadDate { get; set; } = string.Empty;
    public string AverageSpread { get; set; } = string.Empty;
}
