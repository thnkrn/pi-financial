using System.Text.Json.Serialization;

namespace Pi.GlobalEquities.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum OperationType
{
    Unknown = -1,
    Trade = 1,
    Commission,
    AutoConversion,
    StockSplit,
    Dividend,
    DividendTax,
    Tax,
    CorporateAction
}
