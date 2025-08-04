using System.Text.Json.Serialization;

namespace Pi.BackofficeService.Application.Models;

public record CuratedMemberItem
{
    [JsonConstructor]
    public CuratedMemberItem(
      string symbol,
      string friendlyName,
      string logo,
      string figi,
      string units,
      string exchange,
      string dataVendorCode,
      string dataVendorCode2)
    {
        Symbol = symbol;
        FriendlyName = friendlyName;
        Logo = logo ?? string.Empty;
        Figi = figi ?? string.Empty;
        Units = units ?? string.Empty;
        Exchange = exchange;
        DataVendorCode = dataVendorCode ?? string.Empty;
        DataVendorCode2 = dataVendorCode2 ?? string.Empty;
    }

    [JsonPropertyName("symbol")]
    public string Symbol { get; }

    [JsonPropertyName("friendlyName")]
    public string FriendlyName { get; }

    [JsonPropertyName("logo")]
    public string Logo { get; }

    [JsonPropertyName("figi")]
    public string Figi { get; }

    [JsonPropertyName("units")]
    public string Units { get; }

    [JsonPropertyName("exchange")]
    public string Exchange { get; }

    [JsonPropertyName("dataVendorCode")]
    public string DataVendorCode { get; }

    [JsonPropertyName("dataVendorCode2")]
    public string DataVendorCode2 { get; }
}

public record TransformedCuratedMemberItem : CuratedMemberItem
{
    [JsonConstructor]
    public TransformedCuratedMemberItem(
        string symbol,
        string friendlyName,
        string logo,
        string figi,
        string units,
        string exchange,
        string dataVendorCode,
        string dataVendorCode2,
        CuratedListSource? source,
        string? id = null)
        : base(symbol, friendlyName, logo, figi, units, exchange, dataVendorCode, dataVendorCode2)
    {
        Id = id ?? Guid.NewGuid().ToString();
        Source = source;
    }

    [JsonPropertyName("id")]
    public string Id { get; }

    [JsonPropertyName("source")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public CuratedListSource? Source { get; }
}