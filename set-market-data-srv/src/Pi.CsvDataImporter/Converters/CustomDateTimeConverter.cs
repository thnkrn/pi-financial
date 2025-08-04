using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace Pi.CsvDataImporter.Converters;

public class CustomDateTimeConverter : DefaultTypeConverter
{
    private static readonly string[] DateTimeFormats =
    {
        "yyyy-MM-dd HH:mm:ss",
        "yyyy-MM-dd HH:mm:ss.fff"
        // Add other formats if needed
    };

    private static readonly IFormatProvider FormatProvider = CultureInfo.InvariantCulture;

    public override object ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
    {
        // If we have valid text, try to parse it with specific formats
        if (!string.IsNullOrWhiteSpace(text) && DateTime.TryParseExact(text,
                DateTimeFormats,
                FormatProvider,
                DateTimeStyles.None,
                out var dateValue))
            return dateValue;

        // For UpdateTime specifically, use CreateTime as fallback
        if (memberMapData.Names.Contains("update_time"))
        {
            var createTimeValue = row.GetField("create_time");
            if (!string.IsNullOrWhiteSpace(createTimeValue) &&
                DateTime.TryParseExact(createTimeValue,
                    DateTimeFormats,
                    FormatProvider,
                    DateTimeStyles.None,
                    out var createTime))
                return createTime;
        }

        // If all else fails, use current UTC time
        return DateTime.UtcNow;
    }
}