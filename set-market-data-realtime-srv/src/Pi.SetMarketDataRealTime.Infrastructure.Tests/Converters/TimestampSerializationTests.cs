using Newtonsoft.Json;
using Pi.SetMarketDataRealTime.Application.Services.Types.ItchParser;
using Pi.SetMarketDataRealTime.Infrastructure.Converters;
using Xunit.Abstractions;

namespace Pi.SetMarketDataRealTime.Infrastructure.Tests.Converters;

public class TimestampSerializationTests
{
    private readonly ITestOutputHelper _output;

    public TimestampSerializationTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void SerializeTimestamp_WithoutConverter_ResultsInEmptyObject()
    {
        // Arrange
        var timestamp = new Timestamp(1709265069954454552); // 2024-03-01 03:51:09.954454552 UTC
        var settingsWithoutConverter = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented
        };

        // Act
        var serializedWithoutConverter = JsonConvert.SerializeObject(timestamp, settingsWithoutConverter);

        // Assert
        _output.WriteLine("Serialized without converter:");
        _output.WriteLine(serializedWithoutConverter);
        Assert.Equal("{}", serializedWithoutConverter);
    }

    [Fact]
    public void SerializeTimestamp_WithConverter_IncludesFormattedValue()
    {
        // Arrange
        var timestamp = new Timestamp(1709265069954454552); // 2024-03-01 03:51:09.954454552 UTC
        var settingsWithConverter = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            Converters = new JsonConverter[] { new TimestampJsonConverter() }
        };

        // Act
        var serializedWithConverter = JsonConvert.SerializeObject(timestamp, settingsWithConverter);

        // Assert
        _output.WriteLine("Serialized with converter:");
        _output.WriteLine(serializedWithConverter);
        Assert.Equal("\"2024-03-01 03:51:09.954454552 UTC\"", serializedWithConverter);
    }
}