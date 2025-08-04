using Pi.MarketData.Search.Application.Constants;

namespace Pi.MarketData.Search.Application.Services;

public interface ILogoService
{
    string GetLogoUrl(string market, string symbol, string? category = null);
}

public class LogoService : ILogoService
{
    private readonly string _baseUrl;
    private const string ThaiMarket = "SET";

    public LogoService(string baseUrl)
    {
        _baseUrl = baseUrl;
    }

    public string GetLogoUrl(string market, string symbol, string? category = null)
    {
        if (category != null && market.Equals("Equity", StringComparison.OrdinalIgnoreCase) && category.Contains("ETF", StringComparison.OrdinalIgnoreCase))
        {
            return BuildLogoUrl(ThaiMarket, Logo.DefaultEtf);
        }

        if (market.Equals("Derivative", StringComparison.OrdinalIgnoreCase))
        {
            if (Tfex.GoldPrefixes.Any(q => symbol.StartsWith(q, StringComparison.OrdinalIgnoreCase)))
            {
                return BuildLogoUrl(ThaiMarket, Logo.DefaultTfexGold);
            }

            if (symbol.StartsWith(Tfex.SilverPrefix, StringComparison.OrdinalIgnoreCase))
            {
                return BuildLogoUrl(ThaiMarket, Logo.DefaultTfexSilver);
            }
        }

        // Normalize market name for SET markets
        market = NormalizeMarketName(market);

        if (!string.IsNullOrWhiteSpace(market) && !string.IsNullOrWhiteSpace(symbol))
            return BuildLogoUrl(market, symbol);

        return string.Empty;
    }

    private static string NormalizeMarketName(string market)
    {
        if (string.IsNullOrWhiteSpace(market))
            return string.Empty;

        return market.Equals("Equity", StringComparison.OrdinalIgnoreCase) ||
               market.Equals("Derivative", StringComparison.OrdinalIgnoreCase)
            ? ThaiMarket
            : market;
    }

    private string BuildLogoUrl(string market, string symbol)
    {
        return $"{_baseUrl.TrimEnd('/')}/{market}/{symbol}.png";
    }
}
