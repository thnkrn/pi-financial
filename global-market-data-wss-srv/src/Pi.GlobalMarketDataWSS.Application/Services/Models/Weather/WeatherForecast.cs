namespace Pi.GlobalMarketDataWSS.Application.Services.Models.Weather;

public record WeatherForecast
{
    public DateTime Date { get; set; }

    public int TemperatureC { get; init; }

    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}