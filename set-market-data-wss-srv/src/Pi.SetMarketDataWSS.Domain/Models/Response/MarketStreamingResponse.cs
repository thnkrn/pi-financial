using System.Text.Json.Serialization;

namespace Pi.SetMarketDataWSS.Domain.Models.Response;

// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
public class StreamingBody
{
    [JsonPropertyName("symbol")] public string? Symbol { get; set; }

    [JsonPropertyName("venue")] public string? Venue { get; set; }

    [JsonPropertyName("price")] public string? Price { get; set; }

    [JsonPropertyName("auctionPrice")] public string? AuctionPrice { get; set; }

    [JsonPropertyName("auctionVolume")] public string? AuctionVolume { get; set; }

    [JsonPropertyName("isProjected")] public bool IsProjected { get; set; }

    [JsonPropertyName("lastPriceTime")] public long LastPriceTime { get; set; }

    [JsonPropertyName("open")] public string? Open { get; set; }

    [JsonPropertyName("high24H")] public string? High24H { get; set; }

    [JsonPropertyName("low24H")] public string? Low24H { get; set; }

    [JsonPropertyName("priceChanged")] public string? PriceChanged { get; set; }

    [JsonPropertyName("priceChangedRate")] public string? PriceChangedRate { get; set; }

    [JsonPropertyName("volume")] public string? Volume { get; set; }

    [JsonPropertyName("amount")] public string? Amount { get; set; }

    [JsonPropertyName("totalAmount")] public string? TotalAmount { get; set; }

    [JsonPropertyName("totalAmountK")] public string? TotalAmountK { get; set; }

    [JsonPropertyName("totalVolume")] public string? TotalVolume { get; set; }

    [JsonPropertyName("totalVolumeK")] public string? TotalVolumeK { get; set; }

    [JsonPropertyName("open1")] public string? Open1 { get; set; }

    [JsonPropertyName("open2")] public string? Open2 { get; set; }

    [JsonPropertyName("ceiling")] public string? Ceiling { get; set; }

    [JsonPropertyName("floor")] public string? Floor { get; set; }

    [JsonPropertyName("average")] public string? Average { get; set; }

    [JsonPropertyName("averageBuy")] public string? AverageBuy { get; set; }

    [JsonPropertyName("averageSell")] public string? AverageSell { get; set; }

    [JsonPropertyName("aggressor")] public string? Aggressor { get; set; }

    [JsonPropertyName("preClose")] public string? PreClose { get; set; }

    [JsonPropertyName("status")] public string? Status { get; set; }

    [JsonPropertyName("yield")] public string? Yield { get; set; }

    [JsonPropertyName("publicTrades")] public List<List<object>>? PublicTrades { get; set; }

    [JsonPropertyName("orderBook")] public StreamingOrderBook? OrderBook { get; set; }

    [JsonPropertyName("securityType")] public string? SecurityType { get; set; }

    [JsonPropertyName("instrumentType")] public string? InstrumentType { get; set; }

    [JsonPropertyName("market")] public string? Market { get; set; }

    [JsonPropertyName("lastTrade")] public string? LastTrade { get; set; }

    [JsonPropertyName("toLastTrade")] public int ToLastTrade { get; set; }

    [JsonPropertyName("moneyness")] public string? Moneyness { get; set; }

    [JsonPropertyName("maturityDate")] public string? MaturityDate { get; set; }

    [JsonPropertyName("multiplier")] public string? Multiplier { get; set; }

    [JsonPropertyName("exercisePrice")] public string? ExercisePrice { get; set; }

    [JsonPropertyName("intrinsicValue")] public string? IntrinsicValue { get; set; }

    [JsonPropertyName("pSettle")] public string? PSettle { get; set; }

    [JsonPropertyName("poi")] public string? Poi { get; set; }

    [JsonPropertyName("underlying")] public string? Underlying { get; set; }

    [JsonPropertyName("open0")] public string? Open0 { get; set; }

    [JsonPropertyName("basis")] public string? Basis { get; set; }

    [JsonPropertyName("settle")] public string? Settle { get; set; }
}

public class StreamingOrderBook
{
    [JsonPropertyName("bid")] public List<List<string>>? Bid { get; set; }

    [JsonPropertyName("offer")] public List<List<string>>? Offer { get; set; }
}

public class StreamingResponse
{
    [JsonPropertyName("data")] public List<StreamingBody>? Data { get; set; }
}

public class MarketStreamingResponse
{
    [JsonPropertyName("code")] public string? Code { get; set; }

    [JsonPropertyName("op")] public string? Op { get; set; }

    [JsonPropertyName("processing_time")] public string? ProcessingTime { get; set; }

    [JsonPropertyName("response_time")] public string? ResponseTime { get; set; }

    [JsonPropertyName("sending_time")] public string? SendingTime { get; set; }
    
    [JsonPropertyName("creation_time")] public string? CreationTime { get; set; }

    [JsonPropertyName("sequence_number")] public ulong? SequenceNumber { get; set; }

    [JsonPropertyName("sending_id")] public string? SendingId { get; set; }

    [JsonPropertyName("message")] public string? Message { get; set; }

    [JsonPropertyName("response")] public StreamingResponse Response { get; set; } = new();
}