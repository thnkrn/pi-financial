using System.Text.Json.Serialization;

namespace Pi.SetMarketDataWSS.Domain.Models.Response;

public class TickerBody
{
    [JsonPropertyName("symbol")] public string? Symbol { get; set; }

    [JsonPropertyName("venue")] public string? Venue { get; set; }

    [JsonPropertyName("price")] public string? Price { get; set; }

    [JsonPropertyName("currency")] public string? Currency { get; set; }

    [JsonPropertyName("auctionPrice")] public string? AuctionPrice { get; set; }

    [JsonPropertyName("auctionVolume")] public string? AuctionVolume { get; set; }

    [JsonPropertyName("open")] public string? Open { get; set; }

    [JsonPropertyName("high24H")] public string? High24H { get; set; }

    [JsonPropertyName("low24H")] public string? Low24H { get; set; }

    [JsonPropertyName("high52W")] public string? High52W { get; set; }

    [JsonPropertyName("low52W")] public string? Low52W { get; set; }

    [JsonPropertyName("priceChanged")] public string? PriceChanged { get; set; }

    [JsonPropertyName("priceChangedRate")] public string? PriceChangedRate { get; set; }

    [JsonPropertyName("volume")] public string? Volume { get; set; }

    [JsonPropertyName("amount")] public string? Amount { get; set; }

    [JsonPropertyName("changeAmount")] public string? ChangeAmount { get; set; }

    [JsonPropertyName("changeVolume")] public string? ChangeVolume { get; set; }

    [JsonPropertyName("turnoverRate")] public string? TurnoverRate { get; set; }

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

    [JsonPropertyName("pe")] public string? Pe { get; set; }

    [JsonPropertyName("pb")] public string? Pb { get; set; }

    [JsonPropertyName("totalAmount")] public string? TotalAmount { get; set; }

    [JsonPropertyName("totalAmountK")] public string? TotalAmountK { get; set; }

    [JsonPropertyName("totalVolume")] public string? TotalVolume { get; set; }

    [JsonPropertyName("totalVolumeK")] public string? TotalVolumeK { get; set; }

    [JsonPropertyName("tradableEquity")] public string? TradableEquity { get; set; }

    [JsonPropertyName("tradableAmount")] public string? TradableAmount { get; set; }

    [JsonPropertyName("eps")] public string? Eps { get; set; }

    [JsonPropertyName("publicTrades")] public List<object>? PublicTrades { get; set; }

    [JsonPropertyName("orderBook")] public TickerOrderBook? OrderBook { get; set; }

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

    [JsonPropertyName("open1")] public string? Open1 { get; set; }

    [JsonPropertyName("basis")] public string? Basis { get; set; }

    [JsonPropertyName("settle")] public string? Settle { get; set; }

    [JsonPropertyName("instrumentCategory")]
    public string? InstrumentCategory { get; set; }

    [JsonPropertyName("friendlyName")] public string? FriendlyName { get; set; }

    [JsonPropertyName("logo")] public string? Logo { get; set; }

    [JsonPropertyName("exchangeTimezone")] public string? ExchangeTimezone { get; set; }

    [JsonPropertyName("country")] public string? Country { get; set; }

    [JsonPropertyName("offsetSeconds")] public int OffsetSeconds { get; set; }

    [JsonPropertyName("isProjected")] public bool IsProjected { get; set; }

    [JsonPropertyName("lastPriceTime")] public int LastPriceTime { get; set; }
}

public class TickerOrderBook
{
    [JsonPropertyName("bid")] public List<object>? Bid { get; set; }

    [JsonPropertyName("offer")] public List<object>? Offer { get; set; }
}

public class TickerResponse
{
    [JsonPropertyName("data")] public List<TickerBody>? Data { get; set; }
}

public class MarketTickerResponse
{
    [JsonPropertyName("code")] public string? Code { get; set; }

    [JsonPropertyName("message")] public string? Message { get; set; }

    [JsonPropertyName("response")] public TickerResponse? Response { get; set; }
}