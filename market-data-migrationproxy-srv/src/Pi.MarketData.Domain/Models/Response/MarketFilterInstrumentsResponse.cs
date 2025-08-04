using System.Text.Json.Serialization;

namespace Pi.MarketData.Domain.Models.Response;

    public class InstrumentCategoryList
    {
        [JsonPropertyName("order")]
        public int Order { get; set; }

        [JsonPropertyName("instrumentType")]
        public string? InstrumentType { get; set; }

        [JsonPropertyName("instrumentCategory")]
        public string? InstrumentCategory { get; set; }

        [JsonPropertyName("instrumentList")]
        public List<FilterInstrumentList>? InstrumentList { get; set; }
    }

    public class FilterInstrumentList
    {
        [JsonPropertyName("venue")]
        public string? Venue { get; set; }

        [JsonPropertyName("symbol")]
        public string? Symbol { get; set; }

        [JsonPropertyName("friendlyName")]
        public string? FriendlyName { get; set; }

        [JsonPropertyName("logo")]
        public string? Logo { get; set; }

        [JsonPropertyName("price")]
        public string? Price { get; set; }

        [JsonPropertyName("priceChange")]
        public string? PriceChange { get; set; }

        [JsonPropertyName("priceChangeRatio")]
        public string? PriceChangeRatio { get; set; }

        [JsonPropertyName("isFavorite")]
        public bool IsFavorite { get; set; }

        [JsonPropertyName("unit")]
        public string? Unit { get; set; }

        [JsonPropertyName("latestNavTimestamp")]
        public int LatestNavTimestamp { get; set; }

        [JsonPropertyName("isMainSession")]
        public bool IsMainSession { get; set; }

        [JsonPropertyName("totalValue")]
        public string? TotalValue { get; set; }

        [JsonPropertyName("totalVolume")]
        public string? TotalVolume { get; set; }
        
        [JsonPropertyName("nav")]
        public string? Nav { get; set; }
        
        [JsonPropertyName("navChange")]
        public string? NavChange { get; set; }
        
        [JsonPropertyName("navChangePercentage")]
        public string? NavChangePercentage { get; set; }
    }

    public class FilterInstrumentsResponse
    {
        [JsonPropertyName("instrumentCategoryList")]
        public List<InstrumentCategoryList>? InstrumentCategoryList { get; set; }
    }

    public class MarketFilterInstrumentsResponse
    {
        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("response")]
        public FilterInstrumentsResponse? Response { get; set; }
    }