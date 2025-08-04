using MongoDB.Bson;
using Newtonsoft.Json;

namespace Pi.GlobalMarketDataWSS.Infrastructure.Converters;

/// <summary>
///     JSON converter for MongoDB ObjectId
/// </summary>
public class ObjectIdConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(ObjectId);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object? existingValue,
        JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
            return ObjectId.Empty;

        if (reader.TokenType == JsonToken.String)
        {
            var value = reader.Value?.ToString();
            return string.IsNullOrEmpty(value) ? ObjectId.Empty : ObjectId.Parse(value);
        }

        throw new JsonSerializationException($"Unexpected token type: {reader.TokenType}");
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value == null)
        {
            writer.WriteNull();
            return;
        }

        var objectId = (ObjectId)value;
        writer.WriteValue(objectId.ToString());
    }
}