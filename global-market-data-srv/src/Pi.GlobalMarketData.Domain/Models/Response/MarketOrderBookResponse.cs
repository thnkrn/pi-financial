using System.Text.Json.Serialization;

namespace Pi.GlobalMarketData.Domain.Models.Response;

public class BidOfferList
{
    [JsonPropertyName("symbol")]
    public string? Symbol { get; set; }

    [JsonPropertyName("venue")]
    public string? Venue { get; set; }

    [JsonPropertyName("bid")]
    public string? Bid { get; set; }

    [JsonPropertyName("offer")]
    public string? Offer { get; set; }
}

public class OrderBookResponse
{
    [JsonPropertyName("bidOfferList")]
    public List<BidOfferList>? BidOfferList { get; set; }
}

public class MarketOrderBookResponse
{
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("response")]
    public OrderBookResponse? Response { get; set; }
}