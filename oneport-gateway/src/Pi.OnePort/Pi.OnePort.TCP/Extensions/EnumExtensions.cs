using Pi.OnePort.TCP.Enums;

namespace Pi.OnePort.TCP.Extensions;

public static class EnumExtensions
{
    public static string GetSerializedValue(this Enum value)
    {
        var enumType = value.GetType();
        var name = Enum.GetName(enumType, value)!;
        return enumType.GetField(name)?.GetCustomAttributes(false).OfType<SerializedValue>().SingleOrDefault()?.Value
            ?? throw new ArgumentException("Item not found.", nameof(value));
    }

    public static TAttribute? GetAttribute<TAttribute>(this Enum value)
        where TAttribute : Attribute
    {
        var enumType = value.GetType();
        var name = Enum.GetName(enumType, value)!;
        return enumType.GetField(name)?.GetCustomAttributes(false).OfType<TAttribute>().SingleOrDefault();
    }

    public static TEnum ParseFromSerializedValue<TEnum>(string value) where TEnum : Enum
    {
        var enumType = typeof(TEnum);
        foreach (Enum val in Enum.GetValues(enumType))
        {
            var fi = enumType.GetField(val.ToString());
            if (fi == null) continue;

            var attributes = fi.GetCustomAttributes(typeof(SerializedValue), false);
            if (attributes.Length == 0) continue;

            var attr = attributes[0];

            if (attr.ToString() == value)
            {
                return (TEnum)val;
            }
        }
        throw new ArgumentException($"The value '{value}' is not supported by {enumType}.");
    }

    public static TEnum TryParseFromSerializedValue<TEnum>(string value, TEnum defaultValue) where TEnum : Enum
    {
        try
        {
            return ParseFromSerializedValue<TEnum>(value);
        }
        catch (ArgumentException)
        {
            return defaultValue;
        }
    }

    public static TEnum GetEnumFromAttribute<TEnum, TAttribute>(string value)
        where TEnum : Enum
        where TAttribute : Attribute
    {
        var enumType = typeof(TEnum);
        foreach (Enum val in Enum.GetValues(enumType))
        {
            var fi = enumType.GetField(val.ToString());
            if (fi == null) continue;

            var attributes = fi.GetCustomAttributes(typeof(TAttribute), false);
            if (attributes.Length == 0) continue;

            var attr = attributes[0];

            if (attr.ToString() == value)
            {
                return (TEnum)val;
            }
        }
        throw new ArgumentException($"The value '{value}' is not supported by {enumType}.");
    }

    public static TEnum TryGetEnumFromAttribute<TEnum, TAttribute>(string value, TEnum defaultValue)
        where TEnum : Enum
        where TAttribute : Attribute
    {
        try
        {
            return GetEnumFromAttribute<TEnum, TAttribute>(value);
        }
        catch (ArgumentException)
        {
            return defaultValue;
        }
    }
}
