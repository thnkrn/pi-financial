using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pi.SetMarketDataWSS.Application.Services.Types.ItchParser;

namespace Pi.SetMarketDataWSS.Infrastructure.Converters;

public class TimestampJsonConverter : JsonConverter<Timestamp>
{
    public override Timestamp ReadJson(JsonReader reader, Type objectType, Timestamp existingValue,
        bool hasExistingValue, JsonSerializer serializer)
    {
        switch (reader.TokenType)
        {
            case JsonToken.Null:
                return default;

            case JsonToken.String:
                var timestampString = reader.Value as string;
                return ParseTimestampString(timestampString);

            case JsonToken.StartObject:
                var jObject = JObject.Load(reader);
                return ParseTimestampObject(jObject);

            case JsonToken.Integer:
                if (reader.Value is long longValue) return new Timestamp(longValue);
                if (long.TryParse(reader.Value?.ToString(), out var parsedLong)) return new Timestamp(parsedLong);
                break;

            default:
                throw new JsonSerializationException($"Unexpected token {reader.TokenType} when parsing Timestamp.");
        }

        return default;
    }

    private static Timestamp ParseTimestampString(string? timestampString)
    {
        if (string.IsNullOrEmpty(timestampString))
            return default;

        try
        {
            return Timestamp.Parse(timestampString);
        }
        catch (Exception ex)
        {
            throw new JsonSerializationException($"Error parsing Timestamp string: {timestampString}", ex);
        }
    }

    private static Timestamp ParseTimestampObject(JObject? jObject)
    {
        if (jObject is not { HasValues: true }) return default;

        var valueToken = jObject["Value"];
        if (valueToken == null) return default;

        switch (valueToken.Type)
        {
            case JTokenType.String:
            {
                var stringValue = valueToken.Value<string>();
                return ParseTimestampString(stringValue);
            }
            case JTokenType.Integer:
            {
                var longValue = valueToken.Value<long>();
                return new Timestamp(longValue);
            }
            default:
                throw new JsonSerializationException($"Unable to parse Timestamp from object: {jObject}");
        }
    }

    public override void WriteJson(JsonWriter writer, Timestamp value, JsonSerializer serializer)
    {
        if (value == default)
        {
            writer.WriteStartObject();
            writer.WriteEndObject();
        }
        else
        {
            writer.WriteValue(value.ToString());
        }
    }
}