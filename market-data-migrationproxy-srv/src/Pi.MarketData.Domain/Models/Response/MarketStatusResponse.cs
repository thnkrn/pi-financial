using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Pi.MarketData.Domain.Models.Response;

public class MarketStatusResponse
{
    [JsonPropertyName("code")] 
    public string? Code { get; set; }

    [JsonPropertyName("message")] 
    public string? Message { get; set; }

    [JsonPropertyName("response")] 
    public StatusResponse? Response { get; set; }
    
    [JsonPropertyName("debugStack")]
    public string? DebugStack { get; set; }
}

public class StatusResponse
{
    [JsonPropertyName("marketStatus")]
    public string MarketStatus { get; set; }
}

