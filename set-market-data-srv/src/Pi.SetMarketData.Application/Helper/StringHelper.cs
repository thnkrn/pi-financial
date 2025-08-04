using System.Text.RegularExpressions;
using Pi.SetMarketData.Application.Utils;

namespace Pi.SetMarketData.Application.Helper;

public static class StringHelper
{
    private static readonly string DwPattern = @"^(?<symbol>\w+)(?<broker_code>\d{2})(?<series>C|P)(?<year>\d{2})(?<month_num>\d{2})(?<series_code>[A-Z])$";
    private static readonly TimeSpan timeout = TimeSpan.FromSeconds(5);
    public static (string?, string?) DwFormat(string name, IDictionary<string, string> brokerMap)
    {
        var regex = new Regex(DwPattern, RegexOptions.None, timeout);
        var match = regex.Match(name);
        if (match.Success)
        {
            var symbol = match.Groups["symbol"].Value;
            var brokerCode = match.Groups["broker_code"].Value;
            var series = match.Groups["series"].Value;
            var year = match.Groups["year"].Value;
            var month = match.Groups["month_num"].Value;
            var seriesCode = match.Groups["series_code"].Value;

            var monthName = "";
            if (int.TryParse(month, out int monthNumber))
            {
                monthName = DataManipulation.GetMonthNameAbbreviation(monthNumber);
            }

            var direction = "";
            if (series == "C")
            {
                direction = "Call";
            }
            else if (series == "P")
            {
                direction = "Put";
            }

            var brokerId = "[BROKER_ID]";
            if (brokerMap.ContainsKey(brokerCode))
            {
                brokerId = brokerMap[brokerCode];
            }

            return ($"{symbol} {monthName} {year} DW {direction} Series {seriesCode} by {brokerId}", direction);
        }
        return (null, null);
    }

    public static string? WarrantsFormat(string symbol)
    {
        var parts = symbol.Split("-");
        if( parts.Length != 2) return null;
        return $"{parts[0]} Warrant Series {parts[1]}";
    }

    public static string? GetIssuerSeries(string symbol, IDictionary<string, string> brokerMap)
    {
        var regex = new Regex(DwPattern, RegexOptions.None, timeout);
        var match = regex.Match(symbol);
        if (match.Success)
        {
            var brokerCode = match.Groups["broker_code"].Value;
            var series = match.Groups["series_code"].Value;
            if (brokerMap.TryGetValue(brokerCode, out var brokerId))
            {
                return $"{brokerId} / {series}";
            }
        }
        return null;
    }
}