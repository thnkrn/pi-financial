using System.Globalization;
using System.Text.RegularExpressions;

namespace Pi.FundMarketData.Utils;

public class UtilsMethod
{
    public static bool? StringToBool(string value) => string.IsNullOrEmpty(value) ? null : value == "Y";

    public static DateTime? StringToDateTime(string date)
    {
        if (!DateTime.TryParseExact(date, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None,
                out var datetime))
            return null;

        datetime = DateTime.SpecifyKind(datetime, DateTimeKind.Utc);
        return datetime;
    }

    public static decimal? StringToDecimal(string value) => string.IsNullOrEmpty(value) ? null : decimal.Parse(value);

    public static int? StringToInt(string value) => string.IsNullOrEmpty(value) ? null : int.Parse(value);
    public static string ReplaceEncodedSlash(string input)
    {
        var rgx = new Regex("%2F");
        return rgx.Replace(input, "/");
    }

}
