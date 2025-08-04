using Pi.GlobalMarketDataRealTime.Application.Services.Models.Weather;
using Pi.GlobalMarketDataRealTime.Application.Services.SomeExternal;

namespace Pi.GlobalMarketDataRealTime.Infrastructure.Services;

public class SomeExternalService : ISomeExternalService
{
    public Task<WeatherForecast[]> GetWeatherForecast()
    {
        return Task.FromResult(Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55)
            })
            .ToArray());
    }
}