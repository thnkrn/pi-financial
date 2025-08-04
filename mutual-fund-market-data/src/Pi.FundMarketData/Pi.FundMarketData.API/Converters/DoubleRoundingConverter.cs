using System.Text.Json;
using System.Text.Json.Serialization;

namespace Pi.FundMarketData.API.Converters;

public class DoubleRoundingConverter(int decimalPlaces = 4) : JsonConverter<double>
{
    public override double Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.GetDouble();
    }

    public override void Write(Utf8JsonWriter writer, double value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(Math.Round(value, decimalPlaces));
    }
}
