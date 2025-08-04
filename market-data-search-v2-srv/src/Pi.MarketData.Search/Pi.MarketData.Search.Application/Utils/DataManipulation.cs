using System.Globalization;

namespace Pi.MarketData.Search.Application.Utils;

public static class DataManipulation
{
    private static readonly string[] Formats =
    {
        "dd/MM/yyyy HH:mm:ss",
        "dd/MM/yyyy",
    };

    public static bool TryParseDateTime(string? value, out DateTime result)
    {
        if (string.IsNullOrEmpty(value))
        {
            result = default;
            return false;
        }

        // First try explicit formats
        if (DateTime.TryParseExact(
            value,
            Formats,
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out result))
        {
            if (result == DateTime.MinValue) return false;
            return true;
        }

        // Then try general parsing
        bool success = DateTime.TryParse(
            value,
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out result
        );

        if (result == DateTime.MinValue) return false;
        return success;
    }
}