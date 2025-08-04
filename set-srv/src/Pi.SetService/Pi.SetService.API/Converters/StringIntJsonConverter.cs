using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Pi.SetService.API.Converters;

public class StringIntJsonConverter : JsonConverter<int>
{
    private const NumberStyles StringIntStyle =
        NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign |
        NumberStyles.AllowTrailingSign | NumberStyles.Integer | NumberStyles.AllowThousands |
        NumberStyles.AllowExponent;

    public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return default;

        if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt32(out var value))
            return value;

        if (reader.TokenType == JsonTokenType.String)
        {
            var text = reader.GetString();

            if (string.IsNullOrWhiteSpace(text))
                return default;

            if (int.TryParse(text, StringIntStyle, CultureInfo.InvariantCulture, out var result))
                return result;
        }

        throw new JsonException($"Expected String, Number or Null, found: {reader.TokenType}");
    }

    public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
    {
        var text = Convert.ToString(value, CultureInfo.InvariantCulture);
        if (string.IsNullOrWhiteSpace(text))
            writer.WriteNullValue();
        else
            writer.WriteStringValue(text);
    }
}
