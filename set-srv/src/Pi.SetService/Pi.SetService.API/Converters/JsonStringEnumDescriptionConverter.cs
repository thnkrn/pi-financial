using System.Text.Json;
using System.Text.Json.Serialization;
using Pi.Common.ExtensionMethods;

namespace Pi.SetService.API.Converters;

public class JsonStringEnumDescriptionConverter<TEnum> : JsonConverter<TEnum> where TEnum : struct, Enum
{
    public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var description = reader.GetString();

        foreach (var value in Enum.GetValues<TEnum>())
        {
            if (value.GetEnumDescription().Equals(description, StringComparison.CurrentCultureIgnoreCase))
            {
                return value;
            }
        }

        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
    {
        var description = value.GetEnumDescription();
        writer.WriteStringValue(description);
    }
}
