using System.Numerics;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace Pi.StructureNotes.Infrastructure.Helpers;

public class NumberConverter<TNumber> : DefaultTypeConverter where TNumber : INumber<TNumber>
{
    public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return null;
        }

        return TNumber.Parse(text.Replace(",", ""), null);
    }
}
