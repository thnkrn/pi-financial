using Newtonsoft.Json;

namespace Pi.GlobalMarketDataRealTime.DataHandler.Models.Velexa;

public class VelexaScheduleApiResponse
{
    [JsonProperty("intervals")] public List<ResponseInterval>? Intervals { get; set; }
    public override int GetHashCode()
    {
        var hash = new HashCode();
    
        foreach(var interval in Intervals){
            if(interval != null)
                hash.Add(interval.Period?.GetHashCode());
        }   
    return hash.ToHashCode();
    }
    public override bool Equals(object? obj)
    {
        if(obj is VelexaScheduleApiResponse response)
        {
            return GetHashCode() == response.GetHashCode();
        }
        return false;
    }
}

public class ResponseInterval
{
    [JsonProperty("name")] public string? Name { get; set; }

    [JsonProperty("orderTypes")] public string? OrderTypes { get; set; }

    [JsonProperty("period")] public ResponsePeriod? Period { get; set; }
    public override int GetHashCode()
    {
        return Period?.GetHashCode() ?? 0;
    }
public override bool Equals(object? obj)
    {
        if(obj is ResponseInterval interval)
        {
            return Period?.GetHashCode() == interval.Period?.GetHashCode();
        }
        return false;
    }
}

public class ResponsePeriod
{
    [JsonProperty("start")] public long? Start { get; set; }

    [JsonProperty("end")] public long? End { get; set; }
    public override int GetHashCode()
    {
        return Start.GetHashCode();
    }
    public override bool Equals(object? obj)
    {
        if(obj is ResponsePeriod period)
        {
            return Start == period.Start;
        }
        return false;
    }
}