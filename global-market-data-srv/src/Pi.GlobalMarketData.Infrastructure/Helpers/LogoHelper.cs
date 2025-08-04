using Microsoft.Extensions.Configuration;
using Pi.GlobalMarketData.Domain.ConstantConfigurations;

namespace Pi.GlobalMarketData.Infrastructure.Helpers;

public static class LogoHelper
{
    private static readonly IConfiguration Configuration = ConfigurationHelper.GetConfiguration();

    public static string GetLogoUrl(string market, string symbol)
    {
        var logoBaseUrl = Configuration[ConfigurationKeys.LogoBaseUrl]
                          ?? throw new InvalidOperationException("LogoBaseUrl configuration is missing");

        ArgumentException.ThrowIfNullOrEmpty(logoBaseUrl);

        // Normalize market name for SET markets
        market = NormalizeMarketName(market);

        if (!string.IsNullOrEmpty(market) && !string.IsNullOrEmpty(symbol))
            return BuildLogoUrl(logoBaseUrl, market, symbol);

        return string.Empty;
    }

    private static string NormalizeMarketName(string market)
    {
        return market.Equals("Equity", StringComparison.OrdinalIgnoreCase) ||
               market.Equals("Derivative", StringComparison.OrdinalIgnoreCase)
            ? "SET"
            : market;
    }

    private static string BuildLogoUrl(string baseUrl, string market, string symbol)
    {
        return $"{baseUrl.TrimEnd('/')}/{market}/{symbol}.png";
    }
}
