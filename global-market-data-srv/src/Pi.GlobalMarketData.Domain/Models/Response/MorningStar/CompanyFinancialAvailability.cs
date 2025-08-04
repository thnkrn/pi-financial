using System.Text.Json;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Pi.GlobalMarketData.Domain.Models.Response;

public class CompanyFinancialAvailabilityResponse
{
    [BsonId]
    [BsonElement("_id")]
    public ObjectId Id { get; set; }

    public List<CompanyFinancialAvailability>? CompanyFinancialAvailabilityEntityList { get; set; }

    public static CompanyFinancialAvailabilityResponse? FromJson(string json) =>
        JsonSerializer.Deserialize<CompanyFinancialAvailabilityResponse>(json);
}

public class CompanyFinancialAvailability
{
    public string? CompanyName { get; set; }
    public string? ExchangeId { get; set; }
    public string? Symbol { get; set; }
    public string? CIK { get; set; }
    public int Start { get; set; }
    public int End { get; set; }
    public string? SectorId { get; set; }
    public string? SectorName { get; set; }
    public string? IndustryGroupId { get; set; }
    public string? IndustryGroupName { get; set; }
    public string? IndustryId { get; set; }
    public string? IndustryName { get; set; }
    public DateTime LatestQuarterlyReportDate { get; set; }
    public DateTime LatestAnnualReportDate { get; set; }
    public DateTime LatestPreliminaryReportDate { get; set; }
    public DateTime LatestSemiAnnualReportDate { get; set; }
    public string? TemplateCode { get; set; }
    public string? GlobalTemplateCode { get; set; }
}
