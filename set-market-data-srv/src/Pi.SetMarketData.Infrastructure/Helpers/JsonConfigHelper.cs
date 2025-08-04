using Newtonsoft.Json;
using Pi.SetMarketData.Infrastructure.Converters;

namespace Pi.SetMarketData.Infrastructure.Helpers;

public static class JsonConfigHelper
{
    public static void ConfigureGlobalJsonSettings()
    {
        JsonConvert.DefaultSettings = () => new JsonSerializerSettings
        {
            Converters = new List<JsonConverter>
            {
                new TimestampJsonConverter()
            },
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Populate,
            ObjectCreationHandling = ObjectCreationHandling.Replace
        };
    }
}