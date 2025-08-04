using Microsoft.Extensions.Configuration;

namespace Pi.MarketData.SearchAPI.Services;

public interface ILogoService
{
    string GetLogoUrl(string venue, string symbol);
}

public class LogoService : ILogoService
{
    private readonly string _baseUrl;

    public LogoService(string baseUrl)
    {
        _baseUrl = baseUrl;
    }

    public string GetLogoUrl(string venue, string symbol)
    {
        // Example: https://d34vubfbkpay0j.cloudfront.net/NASDAQ/AAPL.svg
        return $"{_baseUrl.TrimEnd('/')}/{venue.Trim()}/{symbol.Trim()}.svg";
    }
}
