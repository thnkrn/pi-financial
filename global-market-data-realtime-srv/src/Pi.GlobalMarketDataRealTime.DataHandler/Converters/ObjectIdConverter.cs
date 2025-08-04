using MongoDB.Bson;
using Newtonsoft.Json;

namespace Pi.GlobalMarketDataRealTime.DataHandler.Converters;

public class ObjectIdConverter : JsonConverter<ObjectId>
{
    public override ObjectId ReadJson(JsonReader reader, Type objectType, ObjectId existingValue, bool hasExistingValue,
        JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.String)
        {
            var value = reader.Value?.ToString() ?? string.Empty;
            return ObjectId.Parse(value);
        }

        throw new JsonSerializationException($"Unexpected token type: {reader.TokenType}");
    }

    public override void WriteJson(JsonWriter writer, ObjectId value, JsonSerializer serializer)
    {
        writer.WriteValue(value.ToString());
    }
}