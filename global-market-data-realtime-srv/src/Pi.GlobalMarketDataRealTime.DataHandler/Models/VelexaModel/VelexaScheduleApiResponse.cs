using Newtonsoft.Json;

namespace Pi.GlobalMarketDataRealTime.DataHandler.Models.VelexaModel;

public class VelexaScheduleApiResponse
{
    [JsonProperty("intervals")] public List<ResponseInterval>? Intervals { get; set; }
}

public class ResponseInterval
{
    [JsonProperty("name")] public string? Name { get; set; }

    [JsonProperty("orderTypes")] public string? OrderTypes { get; set; }

    [JsonProperty("period")] public ResponsePeriod? Period { get; set; }
}

public class ResponsePeriod
{
    [JsonProperty("start")] public long? Start { get; set; }

    [JsonProperty("end")] public long? End { get; set; }
}