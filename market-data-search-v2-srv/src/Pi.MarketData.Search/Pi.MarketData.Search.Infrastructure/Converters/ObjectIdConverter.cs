using MongoDB.Bson;
using Newtonsoft.Json;

namespace Pi.MarketData.Search.Infrastructure.Converters;

public class ObjectIdConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(ObjectId);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
            return ObjectId.Empty;

        if (reader.TokenType == JsonToken.String)
        {
            var value = (string?)reader?.Value;

            // Check if the string is a valid ObjectId
            if (ObjectId.TryParse(value, out ObjectId objectId))
                return objectId;
        }

        return ObjectId.Empty;
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value is ObjectId objectId)
        {
            writer.WriteValue(objectId.ToString());
        }
        else
        {
            writer.WriteNull();
        }
    }
}