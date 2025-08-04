using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Pi.OnePort.Db2.Converters;

public class StringEmptyTrimConverter : ValueConverter<string?, string?>
{
    public StringEmptyTrimConverter() : base(v => v, t => Trim(t))
    {
    }

    private static string? Trim(string? s)
    {
        if (s == null) return null;

        var trim = s.TrimEnd();
        return trim != "" ? trim : " ";
    }
}
