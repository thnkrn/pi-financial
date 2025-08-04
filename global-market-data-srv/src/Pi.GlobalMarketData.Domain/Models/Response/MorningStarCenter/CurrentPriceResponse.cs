using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Pi.GlobalMarketData.Domain.Models.Response.MorningStarCenter;

public class CurrentPriceResponse
{
    [BsonElement("data")]
    public List<Data<CurrentPrice>>? Data { get; set; }

    public static CurrentPriceResponse? FromJson(string json)
    {
        return JsonConvert.DeserializeObject<CurrentPriceResponse>(json);
    }
}

public class CurrentPrice
{
    [JsonProperty("_name")]
    [BsonElement("name")]
    public string? Name { get; set; }

    [BsonElement("currency_iso3")]
    public string? CurrencyISO3 { get; set; }

    [BsonElement("nav_52wk_high")]
    public string? NAV52wkHigh { get; set; }

    [BsonElement("nav_52wk_high_date")]
    public string? NAV52wkHighDate { get; set; }

    [BsonElement("nav_52wk_low")]
    public string? NAV52wkLow { get; set; }

    [BsonElement("nav_52wk_low_date")]
    public string? NAV52wkLowDate { get; set; }

    [BsonElement("market_price_52wk_high")]
    public string? MarketPrice52wkHigh { get; set; }

    [BsonElement("market_price_52wk_high_date")]
    public string? MarketPrice52wkHighDate { get; set; }

    [BsonElement("market_price_52wk_low")]
    public string? MarketPrice52wkLow { get; set; }

    [BsonElement("market_price_52wk_low_date")]
    public string? MarketPrice52wkLowDate { get; set; }

    [BsonElement("day_end_bid_offer_prices_date")]
    public string? DayEndBidOfferPricesDate { get; set; }

    [BsonElement("month_end_bid_offer_prices_date")]
    public string? MonthEndBidOfferPricesDate { get; set; }

    [BsonElement("unsplit_nav")]
    public string? UnsplitNAV { get; set; }

    [BsonElement("day_end_nav_date")]
    public string? DayEndNAVDate { get; set; }

    [BsonElement("day_end_nav")]
    public string? DayEndNAV { get; set; }

    [BsonElement("month_end_nav_date")]
    public string? MonthEndNAVDate { get; set; }

    [BsonElement("month_end_nav")]
    public string? MonthEndNAV { get; set; }

    [BsonElement("day_end_market_price_date")]
    public string? DayEndMarketPriceDate { get; set; }

    [BsonElement("day_end_market_price")]
    public string? DayEndMarketPrice { get; set; }

    [BsonElement("month_end_market_price_date")]
    public string? MonthEndMarketPriceDate { get; set; }

    [BsonElement("month_end_market_price")]
    public string? MonthEndMarketPrice { get; set; }

    [BsonElement("day_end_trading_volume_date")]
    public string? DayEndTradingVolumeDate { get; set; }

    [BsonElement("day_end_trading_volume")]
    public string? DayEndTradingVolume { get; set; }

    [BsonElement("month_end_trading_volume_date")]
    public string? MonthEndTradingVolumeDate { get; set; }

    [BsonElement("month_end_trading_volume")]
    public string? MonthEndTradingVolume { get; set; }

    [BsonElement("pre_tax_mid_date")]
    public string? PreTaxMidDate { get; set; }

    [BsonElement("pre_tax_mid")]
    public string? PreTaxMid { get; set; }

    [BsonElement("month_end_pre_tax_mid_date")]
    public string? MonthEndPreTaxMidDate { get; set; }

    [BsonElement("month_end_pre_tax_mid")]
    public string? MonthEndPreTaxMid { get; set; }

    [BsonElement("performance_return_source")]
    public string? PerformanceReturnSource { get; set; }

    [BsonElement("market_price_open")]
    public string? MarketPriceOpen { get; set; }

    [BsonElement("market_price_day_high")]
    public string? MarketPriceDayHigh { get; set; }

    [BsonElement("market_price_day_low")]
    public string? MarketPriceDayLow { get; set; }
}
