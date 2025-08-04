using Pi.SetMarketData.Application.Services.Models.Weather;

namespace Pi.SetMarketData.Application.Services.SomeExternal
{
    public interface ISomeExternalService
    {
        /// <summary>
        /// Get a weather forecast
        /// </summary>
        /// <returns></returns>
        Task<WeatherForecast[]> GetWeatherForecast();
    }
}
