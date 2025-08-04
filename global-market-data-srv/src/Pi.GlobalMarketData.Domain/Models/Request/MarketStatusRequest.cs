using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Pi.GlobalMarketData.Domain.Models.Request;

public class MarketStatusRequest
{
    [JsonPropertyName("market")] 
    public string? Market { get; set; }
}
