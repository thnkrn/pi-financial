using Newtonsoft.Json;

namespace Pi.GlobalMarketDataWSS.Infrastructure.Helpers;

public static class JsonConfigHelper
{
    public static void ConfigureGlobalJsonSettings()
    {
        JsonConvert.DefaultSettings = () => new JsonSerializerSettings
        {
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Populate,
            ObjectCreationHandling = ObjectCreationHandling.Replace
        };
    }
}