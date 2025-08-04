using System.Text.Json;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Pi.GlobalMarketData.Domain.Models.Response;

public class FullStockExchangeSecurityListResponse
{
    public List<FullStockExchangeSecurityEntity>? FullStockExchangeSecurityEntityList { get; set; }

    public static FullStockExchangeSecurityListResponse? FromJson(string json) =>
        JsonSerializer.Deserialize<FullStockExchangeSecurityListResponse>(json);
}

public class FullStockExchangeSecurityEntity
{
    [BsonId]
    [BsonElement("_id")]
    public ObjectId Id { get; set; }

    [BsonElement("share_class_id")]
    public string? ShareClassId { get; set; }

    [BsonElement("company_id")]
    public string? CompanyId { get; set; }

    [BsonElement("investment_id")]
    public string? InvestmentId { get; set; }

    [BsonElement("country_id")]
    public string? CountryId { get; set; }

    [BsonElement("company_name")]
    public string? CompanyName { get; set; }

    [BsonElement("exchange_id")]
    public string? ExchangeId { get; set; }

    [BsonElement("symbol")]
    public string? Symbol { get; set; }

    [BsonElement("cik")]
    public string? CIK { get; set; }

    [BsonElement("isin")]
    public string? Isin { get; set; }

    [BsonElement("company_lei")]
    public string? CompanyLEI { get; set; }

    [BsonElement("investment_type_id")]
    public string? InvestmentTypeId { get; set; }

    [BsonElement("stock_status")]
    public string? StockStatus { get; set; }

    [BsonElement("suspended_flag")]
    public string? SuspendedFlag { get; set; }

    [BsonElement("market_data_id")]
    public string? MarketDataId { get; set; }
}
