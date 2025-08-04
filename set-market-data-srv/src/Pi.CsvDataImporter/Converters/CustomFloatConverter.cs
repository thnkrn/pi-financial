using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace Pi.CsvDataImporter.Converters;

public class CustomFloatConverter : DefaultTypeConverter
{
    private const float DefaultValue = 0f;

    public override object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
    {
        if (string.IsNullOrWhiteSpace(text)) return DefaultValue;

        return float.TryParse(text, out var floatValue) ? floatValue : DefaultValue;
    }
}