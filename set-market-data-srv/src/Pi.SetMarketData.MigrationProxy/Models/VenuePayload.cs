using Newtonsoft.Json;

namespace Pi.SetMarketData.MigrationProxy.Models;

public class VenuePayload : IPayload<CommonPayload>
{
    [JsonProperty("param")]
    public List<CommonPayload>? Param { get; set; }
}

public class SymbolVenuePayload : IPayload<CommonPayload>
{
    [JsonProperty("symbolVenueList")]
    public List<CommonPayload>? Param { get; set; }
}

public interface IPayload<T>
{
    List<T>? Param { get; set; }
}