using System.ComponentModel;

namespace Pi.Financial.FundService.Application.Utils;

public static class MyEnumExtensions
{
    public static string ToDescriptionString(this Enum val)
    {
        var field = val.GetType().GetField(val.ToString());

        if (field == null)
        {
            throw new InvalidOperationException();
        }

        DescriptionAttribute[]? attributes =
            field.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

        return attributes?.Length > 0 ? attributes[0].Description : throw new InvalidOperationException();
    }

    public static string NumberString(this Enum enVal)
    {
        return enVal.ToString("D");
    }
}
