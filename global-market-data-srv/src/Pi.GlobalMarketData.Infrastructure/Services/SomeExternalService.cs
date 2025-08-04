using Pi.GlobalMarketData.Application.Services.Models.Weather;
using Pi.GlobalMarketData.Application.Services.SomeExternal;

namespace Pi.GlobalMarketData.Infrastructure.Services;

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