using System.Reflection;
using System.Runtime.Serialization;
using Pi.FundMarketData.Constants;

namespace Pi.FundMarketData.Utils;

public static class EnumUtils
{
    public static string GetEnumMemberValue(Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        var attribute = field.GetCustomAttribute<EnumMemberAttribute>();
        return attribute?.Value ?? value.ToString();
    }
}
