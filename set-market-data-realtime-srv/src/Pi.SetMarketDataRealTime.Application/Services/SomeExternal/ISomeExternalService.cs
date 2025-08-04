using Pi.SetMarketDataRealTime.Application.Services.Models.Weather;

namespace Pi.SetMarketDataRealTime.Application.Services.SomeExternal;

public interface ISomeExternalService
{
    /// <summary>
    ///     Get a weather forecast
    /// </summary>
    /// <returns></returns>
    Task<WeatherForecast[]> GetWeatherForecast();
}