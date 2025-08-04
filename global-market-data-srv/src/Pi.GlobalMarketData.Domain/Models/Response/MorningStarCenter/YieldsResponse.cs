using Newtonsoft.Json;

namespace Pi.GlobalMarketData.Domain.Models.Response.MorningStarCenter;

public class YieldsResponse
{
    public List<Data<Yields>>? Data { get; set; }

    public static YieldsResponse? FromJson(string json)
    {
        return JsonConvert.DeserializeObject<YieldsResponse>(json);
    }
}

public class Yields
{
    public string? Name { get; set; }
    public string? Yield1YrDate { get; set; }
    public string? Yield1Yr { get; set; }
}
