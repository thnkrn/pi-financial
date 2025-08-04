using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Pi.SetMarketData.Domain.Entities;

public class MorningStarStocks
{
    [BsonId]
    [BsonElement("_id")]
    public ObjectId Id { get; set; }

    [BsonElement("symbol")]
    public string Symbol { get; set; } = string.Empty;

    [BsonElement("exchange_id")]
    public string ExchangeId { get; set; } = string.Empty;

    [BsonElement("sales")]
    public MorningStarStockItem Sales { get; set; } = new MorningStarStockItem();

    [BsonElement("operating_incomes")]
    public MorningStarStockItem OperatingIncomes { get; set; } = new MorningStarStockItem();

    [BsonElement("net_incomes")]
    public MorningStarStockItem NetIncomes { get; set; } = new MorningStarStockItem();

    [BsonElement("earnings_per_shares")]
    public MorningStarStockItem EarningsPerShares { get; set; } = new MorningStarStockItem();

    [BsonElement("dividend_per_shares")]
    public MorningStarStockItem DividendPerShares { get; set; } = new MorningStarStockItem();

    [BsonElement("cashflow_per_share")]
    public double CashflowPerShare { get; set; }

    [BsonElement("total_assets")]
    public MorningStarStockItem TotalAssets { get; set; } = new MorningStarStockItem();

    [BsonElement("total_liabilities")]
    public MorningStarStockItem TotalLiabilities { get; set; } = new MorningStarStockItem();

    [BsonElement("operating_margin")]
    public MorningStarStockItem OperatingMargin { get; set; } = new MorningStarStockItem();

    [BsonElement("liabilities_to_assets")]
    public MorningStarStockItem LiabilitiesToAssets { get; set; } = new MorningStarStockItem();

    [BsonElement("average_share_count")]
    public MorningStarStockItem AverageShareCount { get; set; } = new MorningStarStockItem();

    [BsonElement("units")]
    public string Units { get; set; } = string.Empty;

    [BsonElement("latest_financials")]
    public DateTime LatestFinancials { get; set; }

    [BsonElement("market_capitalization")]
    public double MarketCapitalization { get; set; }

    [BsonElement("share_free_float")]
    public double ShareFreeFloat { get; set; }

    [BsonElement("industry")]
    public string Industry { get; set; } = string.Empty;

    [BsonElement("sector")]
    public string Sector { get; set; } = string.Empty;

    [BsonElement("country")]
    public string Country { get; set; } = string.Empty;

    [BsonElement("price_to_earnings_ratio")]
    public double PriceToEarningsRatio { get; set; }

    [BsonElement("price_to_book_ratio")]
    public double PriceToBookRatio { get; set; }

    [BsonElement("price_to_sales_ratio")]
    public double PriceToSalesRatio { get; set; }

    [BsonElement("dividend_yield")]
    public double DividendYield { get; set; }

    [BsonElement("ex_dividend_date")]
    public DateTime ExDividendDate { get; set; }

    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;

    [BsonElement("website")]
    public string Website { get; set; } = string.Empty;
}

public class MorningStarStockItem
{
    [BsonElement("values")]
    public List<double> Values { get; set; } = [];

    [BsonElement("statement_type")]
    public string StatementType { get; set; } = string.Empty;
}
