using Pi.BackofficeService.Application.Models;
using System.ComponentModel;
using System.Reflection;

namespace Pi.BackofficeService.Application.Utils;

public static class EnumUtil
{
    public static string? GetEnumDescription<T>(T data) where T : Enum
    {
        var field = typeof(T).GetField(data.ToString());

        if (field == null) return null;

        var attribute = field.GetCustomAttribute<DescriptionAttribute>();

        return attribute?.Description;

    }

    public static string? GetApiPath(this Enum value)
    {
        var type = value.GetType();
        var name = Enum.GetName(type, value);
        if (name == null) { return null; }

        var field = type.GetField(name);
        if (field == null) { return null; }

        var attr = Attribute.GetCustomAttribute(field, typeof(ApiPathAttribute)) as ApiPathAttribute;
        if (attr == null) { return null; }

        return attr.ApiPath;
    }

}
