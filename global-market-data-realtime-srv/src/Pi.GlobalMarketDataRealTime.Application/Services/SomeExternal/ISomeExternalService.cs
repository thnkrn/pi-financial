using Pi.GlobalMarketDataRealTime.Application.Services.Models.Weather;

namespace Pi.GlobalMarketDataRealTime.Application.Services.SomeExternal;

public interface ISomeExternalService
{
    /// <summary>
    ///     Get a weather forecast
    /// </summary>
    /// <returns></returns>
    Task<WeatherForecast[]> GetWeatherForecast();
}