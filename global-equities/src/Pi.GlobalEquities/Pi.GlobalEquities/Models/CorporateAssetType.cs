using System.Text.Json.Serialization;

namespace Pi.GlobalEquities.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CorporateAssetType
{
    Unknown = -1,
    Instrument = 1,
    Cash
}
