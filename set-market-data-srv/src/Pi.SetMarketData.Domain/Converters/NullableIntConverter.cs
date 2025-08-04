using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace Pi.SetMarketData.Domain.Converters;

public class NullableIntConverter : DefaultTypeConverter
{
    public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
    {
        if (string.IsNullOrWhiteSpace(text) || text.Equals("N/A", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        if (int.TryParse(text, out int result))
        {
            return result;
        }

        return null;
    }
}