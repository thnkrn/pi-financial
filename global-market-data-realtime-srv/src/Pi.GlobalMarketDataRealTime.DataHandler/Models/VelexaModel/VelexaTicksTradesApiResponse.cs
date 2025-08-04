using Newtonsoft.Json;

namespace Pi.GlobalMarketDataRealTime.DataHandler.Models.VelexaModel;

public class VelexaTicksQuotesApi2Response
{
    [JsonProperty("ask")] public List<BaseParameter2>? Ask { get; set; }

    [JsonProperty("bid")] public List<BaseParameter2>? Bid { get; set; }

    [JsonProperty("symbolId")] public string? SymbolId { get; set; }

    [JsonProperty("timestamp")] public long Timestamp { get; set; }
}

public class VelexaTicksQuotesApi3Response
{
    [JsonProperty("ask")] public List<BaseParameter3>? Ask { get; set; }

    [JsonProperty("bid")] public List<BaseParameter3>? Bid { get; set; }

    [JsonProperty("symbolId")] public string? SymbolId { get; set; }

    [JsonProperty("timestamp")] public long Timestamp { get; set; }
}

public class BaseParameter2
{
    [JsonProperty("size")] public string? Size { get; set; }

    [JsonProperty("value")] public string? Value { get; set; }
}

public class BaseParameter3
{
    [JsonProperty("price")] public string? Price { get; set; }

    [JsonProperty("size")] public string? Size { get; set; }
}

public class VelexaTicksTradesApi2Response
{
    [JsonProperty("size")] public string? Size { get; set; }

    [JsonProperty("value")] public string? Value { get; set; }

    [JsonProperty("symbolId")] public string? SymbolId { get; set; }

    [JsonProperty("timestamp")] public long Timestamp { get; set; }
}

public class VelexaTicksTradesApi3Response
{
    [JsonProperty("price")] public string? Price { get; set; }

    [JsonProperty("size")] public string? Size { get; set; }

    [JsonProperty("symbolId")] public string? SymbolId { get; set; }

    [JsonProperty("timestamp")] public long Timestamp { get; set; }
}