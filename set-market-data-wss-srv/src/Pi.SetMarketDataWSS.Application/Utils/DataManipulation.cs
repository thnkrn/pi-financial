using Pi.SetMarketDataWSS.Application.Services.Models.ItchOrderBookMapper;

namespace Pi.SetMarketDataWSS.Application.Utils;

public static class DataManipulation
{
    private const string DefaultValue = "0.00";

    public static string FormatDecimals(this string? value, int decimals, bool handleUnavailableValue = false,
        int fixedDecimal = 0)
    {
        // Handle the case where value is null, empty or "0" then return "0.00"
        if (string.IsNullOrEmpty(value) || value == "0") return DefaultValue;

        if (handleUnavailableValue)
            value = HandleUnavailableValue(value);

        // Handle the case where decimals is 0
        if (decimals == 0 && value.Contains('.'))
            // If value contains a decimal point, return it unchanged
            return value;

        // Remove any existing decimal point
        value = value.Replace(".", "");

        // Calculate decimal point 
        _ = double.TryParse(value, out var decimalsInInteger);
        var formattedValue = decimalsInInteger / Math.Pow(10, decimals);
        var result = formattedValue.ToString("0.00######");

        // Formatting result
        if (fixedDecimal > 0) result = formattedValue.ToString($"F{fixedDecimal}");

        return result;
    }

    public static string CalculatePriceChanged(this PublicTrade? publicTrade, string? preClose)
    {
        if (string.IsNullOrEmpty(preClose)) return DefaultValue;

        if (int.TryParse(preClose, out var lastPrice))
            return (publicTrade?.Price.Value - lastPrice).ToString() ?? DefaultValue;

        return DefaultValue;
    }

    private static string HandleUnavailableValue(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return DefaultValue;

        var result = value.Equals("-2147483648") ? DefaultValue : value;
        return result;
    }
}