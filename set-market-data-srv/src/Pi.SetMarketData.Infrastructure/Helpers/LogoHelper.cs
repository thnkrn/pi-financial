using Microsoft.Extensions.Configuration;
using Pi.SetMarketData.Application.Constants;
using Pi.SetMarketData.Domain.ConstantConfigurations;

namespace Pi.SetMarketData.Infrastructure.Helpers;

public static class LogoHelper
{
    private static readonly IConfiguration Configuration = ConfigurationHelper.GetConfiguration();

    public static string GetLogoUrl(string market, string securityType, string symbol)
    {
        var logoBaseUrl = Configuration[ConfigurationKeys.LogoBaseUrl] ??
                          throw new InvalidOperationException("LogoBaseUrl configuration is missing");
        
        ArgumentException.ThrowIfNullOrEmpty(logoBaseUrl);

        return securityType.Equals(InstrumentConstants.ETF, StringComparison.OrdinalIgnoreCase)
            ? BuildLogoUrl(logoBaseUrl, "SET", Logo.DefaultEtf)
            : GetLogoUrl(market, symbol);
    }

    public static string GetLogoUrl(string market, string symbol)
    {
        var logoBaseUrl = Configuration[ConfigurationKeys.LogoBaseUrl] 
                          ?? throw new InvalidOperationException("LogoBaseUrl configuration is missing");
        
        ArgumentException.ThrowIfNullOrEmpty(logoBaseUrl);

        if (market.Equals("Derivative", StringComparison.OrdinalIgnoreCase) )
        {
            if (Tfex.GoldPrefixes.Any(q => symbol.StartsWith(q, StringComparison.OrdinalIgnoreCase)))
            {
                return BuildLogoUrl(logoBaseUrl, "SET", Logo.DefaultTfexGold);
            }

            if (symbol.StartsWith(Tfex.SilverPrefix, StringComparison.OrdinalIgnoreCase))
            {
                return BuildLogoUrl(logoBaseUrl, "SET", Logo.DefaultTfexSilver);
            }
        }
        
        // Normalize market name for SET markets
        market = NormalizeMarketName(market);

        if(!string.IsNullOrEmpty(market) && !string.IsNullOrEmpty(symbol)) 
            return BuildLogoUrl(logoBaseUrl, market, symbol);

        return string.Empty;
    }

    private static string NormalizeMarketName(string market) => 
        market.Equals("Equity", StringComparison.OrdinalIgnoreCase) || 
        market.Equals("Derivative", StringComparison.OrdinalIgnoreCase) 
            ? "SET" 
            : market;

    private static string BuildLogoUrl(string baseUrl, string market, string symbol) => 
        $"{baseUrl.TrimEnd('/')}/{market}/{symbol}.png";
}
