using System.Text.Json.Serialization;

namespace Pi.GlobalMarketDataWSS.Domain.Models.Response;

// ReSharper disable PropertyCanBeMadeInitOnly.Global
public class StreamingBody
{
    [JsonPropertyName("symbol")] public string Symbol { get; set; } = string.Empty;
    [JsonPropertyName("venue")] public string Venue { get; set; } = string.Empty;
    [JsonPropertyName("price")] public string Price { get; set; } = string.Empty;
    [JsonPropertyName("auctionPrice")] public string AuctionPrice { get; set; } = "0.00";
    [JsonPropertyName("auctionVolume")] public string AuctionVolume { get; set; } = "0.0";
    [JsonPropertyName("isProjected")] public bool IsProjected { get; set; }
    [JsonPropertyName("lastPriceTime")] public long LastPriceTime { get; set; }
    [JsonPropertyName("open")] public string Open { get; set; } = string.Empty;
    [JsonPropertyName("high24H")] public string High24H { get; set; } = string.Empty;
    [JsonPropertyName("low24H")] public string Low24H { get; set; } = string.Empty;
    [JsonPropertyName("priceChanged")] public string PriceChanged { get; set; } = "0.00";
    [JsonPropertyName("priceChangedRate")] public string PriceChangedRate { get; set; } = "0.00";
    [JsonPropertyName("volume")] public string Volume { get; set; } = "0.0";
    [JsonPropertyName("amount")] public string Amount { get; set; } = "0.00";
    [JsonPropertyName("totalAmount")] public string TotalAmount { get; set; } = "0.00";
    [JsonPropertyName("totalAmountK")] public string TotalAmountK { get; set; } = "0.00";
    [JsonPropertyName("totalVolume")] public string TotalVolume { get; set; } = "0.0";
    [JsonPropertyName("totalVolumeK")] public string TotalVolumeK { get; set; } = "0.0";
    [JsonPropertyName("open1")] public string Open1 { get; set; } = string.Empty;
    [JsonPropertyName("open2")] public string Open2 { get; set; } = string.Empty;
    [JsonPropertyName("ceiling")] public string Ceiling { get; set; } = "0";
    [JsonPropertyName("floor")] public string Floor { get; set; } = "0";
    [JsonPropertyName("average")] public string Average { get; set; } = "0";
    [JsonPropertyName("averageBuy")] public string AverageBuy { get; set; } = "0";
    [JsonPropertyName("averageSell")] public string AverageSell { get; set; } = "0";
    [JsonPropertyName("aggressor")] public string Aggressor { get; set; } = string.Empty;
    [JsonPropertyName("preClose")] public string PreClose { get; set; } = "0.00";
    [JsonPropertyName("status")] public string Status { get; set; } = string.Empty;
    [JsonPropertyName("yield")] public string Yield { get; set; } = "0";
    [JsonPropertyName("publicTrades")] public List<List<object>> PublicTrades { get; set; } = [];
    [JsonPropertyName("orderBook")] public StreamingOrderBook OrderBook { get; set; } = new();
    [JsonPropertyName("securityType")] public string SecurityType { get; set; } = string.Empty;
    [JsonPropertyName("instrumentType")] public string InstrumentType { get; set; } = string.Empty;
    [JsonPropertyName("market")] public string Market { get; set; } = string.Empty;
    [JsonPropertyName("lastTrade")] public string LastTrade { get; set; } = string.Empty;
    [JsonPropertyName("toLastTrade")] public int ToLastTrade { get; set; }
    [JsonPropertyName("moneyness")] public string Moneyness { get; set; } = string.Empty;
    [JsonPropertyName("maturityDate")] public string MaturityDate { get; set; } = string.Empty;
    [JsonPropertyName("multiplier")] public string Multiplier { get; set; } = "0";
    [JsonPropertyName("exercisePrice")] public string ExercisePrice { get; set; } = "0.00";
    [JsonPropertyName("intrinsicValue")] public string IntrinsicValue { get; set; } = "0";
    [JsonPropertyName("pSettle")] public string PSettle { get; set; } = "0.00";
    [JsonPropertyName("poi")] public string Poi { get; set; } = "0";
    [JsonPropertyName("underlying")] public string Underlying { get; set; } = "0";
    [JsonPropertyName("open0")] public string Open0 { get; set; } = string.Empty;
    [JsonPropertyName("basis")] public string Basis { get; set; } = "0";
    [JsonPropertyName("settle")] public string Settle { get; set; } = "0";
    [JsonPropertyName("prior_close")] public string PriorClose { get; set; } = "0.00";
}

public class StreamingOrderBook
{
    [JsonPropertyName("bid")] public List<List<string>> Bid { get; set; } = [];
    [JsonPropertyName("offer")] public List<List<string>> Offer { get; set; } = [];
}

public class StreamingResponse
{
    [JsonPropertyName("data")] public List<StreamingBody> Data { get; set; } = [];
}

public class MarketStreamingResponse
{
    [JsonPropertyName("code")] public string Code { get; set; } = string.Empty;
    [JsonPropertyName("op")] public string Op { get; set; } = string.Empty;
    [JsonPropertyName("processing_time")] public string? ProcessingTime { get; set; } = string.Empty;
    [JsonPropertyName("response_time")] public string? ResponseTime { get; set; } = string.Empty;
    [JsonPropertyName("sending_time")] public string? SendingTime { get; set; } = string.Empty;
    [JsonPropertyName("creation_time")] public string? CreationTime { get; set; } = string.Empty;
    [JsonPropertyName("sequence_number")] public long? SequenceNumber { get; set; } = 0;
    [JsonPropertyName("sending_id")] public string? SendingId { get; set; } = string.Empty;
    [JsonPropertyName("md_entry_type")] public string? MdEntryType { get; set; } = string.Empty;
    [JsonPropertyName("message")] public string Message { get; set; } = string.Empty;
    [JsonPropertyName("response")] public StreamingResponse Response { get; set; } = new();
}