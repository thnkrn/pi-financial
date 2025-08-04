using Newtonsoft.Json;

namespace Pi.MarketData.Domain.Models;

public class VenuePayload : IPayload<CommonPayload>
{
    [JsonProperty("param")] public List<CommonPayload>? Param { get; set; }
}

public class SymbolVenuePayload : IPayload<CommonPayload>
{
    [JsonIgnore]
    public List<CommonPayload>? Param { get; set; }

    [JsonProperty("symbolVenueList")]
    public List<CommonPayload>? SymbolVenueList
    {
        get => Param;
        set => Param = value;
    }
}

public interface IPayload<T>
{
    List<T>? Param { get; set; }
}