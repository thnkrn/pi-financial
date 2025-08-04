using System.Text.Json.Serialization;

namespace Pi.GlobalMarketData.Domain.Models.Response;

public class AverageShareCount
{
    [JsonPropertyName("yy")] public string? Yy { get; set; }

    [JsonPropertyName("list")] public List<string>? List { get; set; }

    [JsonPropertyName("statementType")] public string? StatementType { get; set; }
}

public class CashflowPerShare
{
    [JsonPropertyName("yy")] public string? Yy { get; set; }

    [JsonPropertyName("list")] public List<string>? List { get; set; }

    [JsonPropertyName("statementType")] public string? StatementType { get; set; }
}

public class DividendPerShare
{
    [JsonPropertyName("yy")] public string? Yy { get; set; }

    [JsonPropertyName("list")] public List<string>? List { get; set; }

    [JsonPropertyName("statementType")] public string? StatementType { get; set; }
}

public class EarningsPerShare
{
    [JsonPropertyName("yy")] public string? Yy { get; set; }

    [JsonPropertyName("list")] public List<string>? List { get; set; }

    [JsonPropertyName("statementType")] public string? StatementType { get; set; }
}

public class LiabilitiesToAssets
{
    [JsonPropertyName("yy")] public string? Yy { get; set; }

    [JsonPropertyName("list")] public List<string>? List { get; set; }

    [JsonPropertyName("statementType")] public string? StatementType { get; set; }
}

public class NetIncome
{
    [JsonPropertyName("yy")] public string? Yy { get; set; }

    [JsonPropertyName("list")] public List<string>? List { get; set; }

    [JsonPropertyName("statementType")] public string? StatementType { get; set; }
}

public class OperatingIncome
{
    [JsonPropertyName("yy")] public string? Yy { get; set; }

    [JsonPropertyName("list")] public List<string>? List { get; set; }

    [JsonPropertyName("statementType")] public string? StatementType { get; set; }
}

public class OperatingMargin
{
    [JsonPropertyName("yy")] public string? Yy { get; set; }

    [JsonPropertyName("list")] public List<string>? List { get; set; }

    [JsonPropertyName("statementType")] public string? StatementType { get; set; }
}

public class ProfileFinancialsResponse
{
    [JsonPropertyName("sales")] public Sales? Sales { get; set; }

    [JsonPropertyName("operatingIncome")] public OperatingIncome? OperatingIncome { get; set; }

    [JsonPropertyName("netIncome")] public NetIncome? NetIncome { get; set; }

    [JsonPropertyName("earningsPerShare")] public EarningsPerShare? EarningsPerShare { get; set; }

    [JsonPropertyName("dividendPerShare")] public DividendPerShare? DividendPerShare { get; set; }

    [JsonPropertyName("cashflowPerShare")] public CashflowPerShare? CashflowPerShare { get; set; }

    [JsonPropertyName("totalAssets")] public TotalAssets? TotalAssets { get; set; }

    [JsonPropertyName("totalLiabilities")] public TotalLiabilities? TotalLiabilities { get; set; }

    [JsonPropertyName("operatingMargin")] public OperatingMargin? OperatingMargin { get; set; }

    [JsonPropertyName("liabilitiesToAssets")]
    public LiabilitiesToAssets? LiabilitiesToAssets { get; set; }

    [JsonPropertyName("averageShareCount")]
    public AverageShareCount? AverageShareCount { get; set; }

    [JsonPropertyName("units")] public string? Units { get; set; }

    [JsonPropertyName("latestFinancials")] public string? LatestFinancials { get; set; }

    [JsonPropertyName("source")] public string? Source { get; set; }
}

public class MarketProfileFinancialsResponse
{
    [JsonPropertyName("code")] public string? Code { get; set; }

    [JsonPropertyName("message")] public string? Message { get; set; }

    [JsonPropertyName("response")] public ProfileFinancialsResponse? Response { get; set; }
}

public class Sales
{
    [JsonPropertyName("yy")] public string? Yy { get; set; }

    [JsonPropertyName("list")] public List<string>? List { get; set; }

    [JsonPropertyName("statementType")] public string? StatementType { get; set; }
}

public class TotalAssets
{
    [JsonPropertyName("yy")] public string? Yy { get; set; }

    [JsonPropertyName("list")] public List<string>? List { get; set; }

    [JsonPropertyName("statementType")] public string? StatementType { get; set; }
}

public class TotalLiabilities
{
    [JsonPropertyName("yy")] public string? Yy { get; set; }

    [JsonPropertyName("list")] public List<string>? List { get; set; }

    [JsonPropertyName("statementType")] public string? StatementType { get; set; }
}