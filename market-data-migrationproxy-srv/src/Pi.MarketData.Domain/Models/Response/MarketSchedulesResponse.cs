using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Pi.MarketData.Domain.Models.Response;

public class MarketSchedulesResponse
{
    [JsonPropertyName("code")] public string? Code { get; set; }

    [JsonPropertyName("message")] public string? Message { get; set; }

    [JsonPropertyName("response")] public SchedulesResponse? Response { get; set; }
}

public class SchedulesResponse
{
    [JsonPropertyName("serverTimestamp")]
    public long ServerTimestamp { get; set; }
    [JsonPropertyName("schedules")]
    public List<Schedules> Schedules { get; set; }
}

public class Schedules
{
    [JsonPropertyName("statusName")]
    public string StatusName { get; set; }
    [JsonPropertyName("date")]
    public string Date { get; set; }
    [JsonPropertyName("startTime")]
    public long StartTime { get; set; }
    [JsonPropertyName("endTime")]
    public long EndTime { get; set; }
}
