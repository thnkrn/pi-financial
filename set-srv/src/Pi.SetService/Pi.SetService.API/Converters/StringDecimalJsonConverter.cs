using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Pi.SetService.API.Converters;

public class StringDecimalJsonConverter : JsonConverter<decimal>
{
    public int? Digit { get; init; }

    private const NumberStyles StringDecimalStyle =
        NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign |
        NumberStyles.AllowTrailingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands |
        NumberStyles.AllowExponent;

    public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return default;

        if (reader.TokenType == JsonTokenType.Number && reader.TryGetDecimal(out var value))
            return value;

        if (reader.TokenType == JsonTokenType.String)
        {
            var text = reader.GetString();

            if (string.IsNullOrWhiteSpace(text))
                return default;

            if (decimal.TryParse(text, StringDecimalStyle, CultureInfo.InvariantCulture, out var result))
                return result;
        }

        throw new JsonException($"Expected String, Number or Null, found: {reader.TokenType}");
    }

    public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
    {
        var rounded = Digit != null ? decimal.Round(value, (int)Digit) : value;
        var text = Convert.ToString(rounded, CultureInfo.InvariantCulture);
        if (string.IsNullOrWhiteSpace(text))
            writer.WriteNullValue();
        else
            writer.WriteStringValue(text);
    }
}
