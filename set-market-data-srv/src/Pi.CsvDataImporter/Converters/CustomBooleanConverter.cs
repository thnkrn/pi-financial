using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace Pi.CsvDataImporter.Converters;

public class CustomBooleanConverter : DefaultTypeConverter
{
    private const double Epsilon = 1e-10;

    public override object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
    {
        if (string.IsNullOrWhiteSpace(text) ||
            text.Equals("N/A", StringComparison.OrdinalIgnoreCase)) return false; // Default value when N/A

        // Try parsing as number first (0/1)
        if (double.TryParse(text, out var numericValue))
            return Math.Abs(numericValue) > Epsilon; // Return true if value is not close to 0

        // If not numeric, try parsing as boolean string
        return bool.TryParse(text, out var boolValue) && boolValue;
        // If all parsing fails, return default value
    }
}