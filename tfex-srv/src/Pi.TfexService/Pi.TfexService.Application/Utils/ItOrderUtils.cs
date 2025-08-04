using System.Reflection;
using Pi.Client.ItBackOffice.Model;
using Pi.TfexService.Application.Models;

namespace Pi.TfexService.Application.Utils;

public static class ItOrderUtils
{
    public static Side MappingSide(string refType)
    {
        return refType switch
        {
            "BU" or "BH" => Side.Long,
            "SE" or "SH" => Side.Short,
            _ => Side.Unknown
        };
    }

    public static Position MappingPosition(string buySell)
    {
        return buySell switch
        {
            "C" => Position.Close,
            "O" => Position.Open,
            _ => Position.Unknown
        };
    }

    public static DateTime? CalculateDateTime(DateTime? refDate, TradeDetailConfirmTime confirmTime)
    {
        return refDate?.AddHours(confirmTime.Hours)
            .AddMinutes(confirmTime.Minutes)
            .AddSeconds(confirmTime.Seconds);
    }

    public static double? CalculateTotalAmount(string refType, double amount, double commission, double vat)
    {
        var comAndVat = commission + vat;
        var side = MappingSide(refType);

        return side switch
        {
            Side.Long => amount + comAndVat,
            Side.Short => amount - comAndVat,
            _ => null
        };
    }

    public static IEnumerable<T> OrderByProperty<T>(this IEnumerable<T> list, string propertyName, bool ascending = true)
    {
        var propertyInfo = typeof(T).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        if (propertyInfo == null)
        {
            throw new ArgumentException("Property not found.");
        }

        return ascending
            ? list.OrderBy(x => propertyInfo.GetValue(x, null))
            : list.OrderByDescending(x => propertyInfo.GetValue(x, null));
    }
}