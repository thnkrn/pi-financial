using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Pi.OnePort.Db2.Converters;

public class StringTrimConverter : ValueConverter<string, string>
{
    public StringTrimConverter() : base(v => v, t => t.TrimEnd())
    {
    }
}
