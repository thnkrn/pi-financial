using StackExchange.Redis;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Pi.MarketData.Search.Application.Services
{
    public class StreamingDataResponse
    {
        public string Price { get; set; } = "0.00";
        public string PriceChanged { get; set; } = "0.00";
        public string PriceChangedRate { get; set; } = "0.00";
    }


    public interface IStreamingDataCacheService
    {
        Task<StreamingDataResponse> GetStreamingDataAsync(string findingKey);
    }

    public class StreamingDataCacheService : IStreamingDataCacheService
    {
        private readonly IDatabase _db;
        private readonly ILogger<StreamingDataCacheService> _logger;
        private readonly string _cacheKeyPrefix;

        public StreamingDataCacheService(IDatabase redisDb, ILogger<StreamingDataCacheService> logger, string cacheKeyPrefix)
        {
            _db = redisDb ?? throw new ArgumentNullException(nameof(redisDb));
            _logger = logger;
            _cacheKeyPrefix = cacheKeyPrefix;
        }

        public async Task<StreamingDataResponse> GetStreamingDataAsync(string findingKey)
        {
            string cacheKey = $"{_cacheKeyPrefix}{findingKey}";
            try
            {


                _logger.LogDebug("Getting streaming data for key: {CacheKey}", cacheKey);

                var cachedData = await _db.HashGetAsync(cacheKey, "data");

                if (cachedData.IsNullOrEmpty)
                {
                    _logger.LogDebug("Streaming data not found in cache for key: {CacheKey}", cacheKey);
                    return new StreamingDataResponse();
                }

                try
                {
                    var dataString = cachedData.ToString();
                    _logger.LogDebug("Received data: {Data}", dataString);

                    if (dataString.Contains("\"Code\\\""))
                    {
                        // Convert JSON encoded as a JSON string to a JSON object
                        var cleanData = JToken.Parse(dataString).ToString();
                        _logger.LogDebug("Cleaned data: {Data}", cleanData);

                        var setResponse = JsonConvert.DeserializeObject<SetStreamingData>(cleanData);
                        if (setResponse?.Response?.Data?.FirstOrDefault() != null)
                        {
                            var data = setResponse.Response.Data[0];
                            return new StreamingDataResponse
                            {
                                Price = data.Price,
                                PriceChanged = data.PriceChanged,
                                PriceChangedRate = data.PriceChangedRate,
                            };
                        }
                    }
                    else
                    {
                        var geData = JsonConvert.DeserializeObject<GeStreamingData>(dataString);
                        if (geData != null)
                        {
                            return new StreamingDataResponse
                            {
                                Price = geData.Price,
                                PriceChanged = geData.PriceChanged,
                                PriceChangedRate = geData.PriceChangedRate,
                            };
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error parsing streaming data for key: {CacheKey}", cacheKey);
                }

                return new StreamingDataResponse();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting streaming data for key: {CacheKey}", cacheKey);
                return new StreamingDataResponse();
            }
        }

        // Only use for parsing the data from the cache
        private sealed class GeStreamingData
        {
            public string Price { get; set; } = "0.00";
            public string PriceChanged { get; set; } = "0.00";
            public string PriceChangedRate { get; set; } = "0.00";
        }

        private sealed class SetTfexStreamingData
        {
            public string Price { get; set; } = "0.00";
            public string PriceChanged { get; set; } = "0.00";
            public string PriceChangedRate { get; set; } = "0.00";

        }

        private sealed class SetStreamingResponse
        {
            public List<SetTfexStreamingData> Data { get; set; } = new();
        }

        private sealed class SetStreamingData
        {
            public string Code { get; set; } = "";
            public string Op { get; set; } = "";
            public string Message { get; set; } = "";
            public SetStreamingResponse Response { get; set; } = new();
        }
    }
}