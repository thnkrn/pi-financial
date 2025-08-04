namespace Pi.SetMarketDataRealTime.Domain.Models.Holiday;

// ReSharper disable UnusedAutoPropertyAccessor.Global
public class HolidayApiResponse<T>
{
    public T? Data { get; set; }
    public int Status { get; set; }
    public string? Title { get; set; }
    public string? Detail { get; set; }
}