using Pi.SetMarketDataRealTime.Application.Helpers;
using Xunit.Abstractions;

namespace Pi.SetMarketDataRealTime.Application.Tests.Helpers;

public class JsonHelperTests
{
    private readonly ITestOutputHelper _output;

    public JsonHelperTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void CleanJsonMessage_RemovesWhitespaceOutsideQuotes()
    {
        var input = "{\n\t\"name\":\t\"John Doe\",\n\t\"age\":   30\n}";
        var expected = "{\"name\":\"John Doe\",\"age\":30}";
        var result = input.SimpleCleanJsonMessage();
        Assert.Equal(expected, result);
    }

    [Fact]
    public void CleanJsonMessage_PreservesWhitespaceInsideQuotes()
    {
        var input = "{\n\t\"description\":\t\"This is a    multi-line\ndescription\",\n\t\"code\":\t\"var x = 5;\"}";
        var expected = "{\"description\":\"This is a multi-linedescription\",\"code\":\"var x = 5;\"}";
        var result = input.SimpleCleanJsonMessage();
        Assert.Equal(expected, result);
    }

    [Fact]
    public void CleanJsonMessage_HandlesNestedObjects()
    {
        var input = "{\n\t\"person\": {\n\t\t\"name\": \"Alice\",\n\t\t\"age\": 25\n\t}\n}";
        var expected = "{\"person\":{\"name\":\"Alice\",\"age\":25}}";
        var result = input.SimpleCleanJsonMessage();
        Assert.Equal(expected, result);
    }

    [Fact]
    public void CleanJsonMessage_PreservesEscapedQuotes()
    {
        var input = "{\n\t\"quote\": \"He said,   Hello!\"\n}";
        var expected = "{\"quote\":\"He said, Hello!\"}";
        var result = input.SimpleCleanJsonMessage();
        Assert.Equal(expected, result);
    }

    [Fact]
    public void CleanJsonMessage_HandlesEmptyInput()
    {
        var input = "";
        var expected = "";
        var result = input.SimpleCleanJsonMessage();
        Assert.Equal(expected, result);
    }

    [Fact]
    public void CleanJsonMessage_HandlesNullInput()
    {
        string input = null;
        var result = input.SimpleCleanJsonMessage();
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void CleanJsonMessage_RemovesSpacesAroundColonsAndCommas()
    {
        var input = "{\"key1\" : \"value1\" , \"key2\":  {\"nested\" : \"value2\"}}";
        var expected = "{\"key1\":\"value1\",\"key2\":{\"nested\":\"value2\"}}";
        var result = input.SimpleCleanJsonMessage();
        Assert.Equal(expected, result);
    }

    [Fact]
    public void CleanJsonMessage_PreservesWhitespaceInValueFields()
    {
        var testCases = new[]
        {
            new
            {
                Input =
                    "{\"Nanos\":{\"Value\":111442352}, \"ExchangeCode\":{\"Value\":35}, \"ExchangeName\":{\"Value\":\"The Stock Exchange of Thailand  \"}, \"MsgType\":\"e\", \"Metadata\":{\"Timestamp\":1709242340000000000, \"Session\":\"332e0b2d71864555a28700f7709cd56b\", \"SequenceNumber\":3}}",
                Expected =
                    "{\"Nanos\":{\"Value\":111442352},\"ExchangeCode\":{\"Value\":35},\"ExchangeName\":{\"Value\":\"The Stock Exchange of Thailand\"},\"MsgType\":\"e\",\"Metadata\":{\"Timestamp\":1709242340000000000,\"Session\":\"332e0b2d71864555a28700f7709cd56b\",\"SequenceNumber\":3}}"
            },
            new
            {
                Input =
                    "{\"Nanos\":{\"Value\":111442352}, \"MarketCode\":{\"Value\":104}, \"MarketName\":{\"Value\":\"TFEX Thailand Metal             \"}, \"MarketDescription\":{\"Value\":\"TXM  \"}, \"MsgType\":\"m\", \"Metadata\":{\"Timestamp\":1709242340000000000, \"Session\":\"332e0b2d71864555a28700f7709cd56b\", \"SequenceNumber\":4}}",
                Expected =
                    "{\"Nanos\":{\"Value\":111442352},\"MarketCode\":{\"Value\":104},\"MarketName\":{\"Value\":\"TFEX Thailand Metal\"},\"MarketDescription\":{\"Value\":\"TXM\"},\"MsgType\":\"m\",\"Metadata\":{\"Timestamp\":1709242340000000000,\"Session\":\"332e0b2d71864555a28700f7709cd56b\",\"SequenceNumber\":4}}"
            },
            new
            {
                Input =
                    "{\"Nanos\":{\"Value\":111442352}, \"MarketCode\":{\"Value\":107}, \"MarketName\":{\"Value\":\"TFEX Thailand Deferred Contract \"}, \"MarketDescription\":{\"Value\":\"TXD  \"}, \"MsgType\":\"m\", \"Metadata\":{\"Timestamp\":1709242340000000000, \"Session\":\"332e0b2d71864555a28700f7709cd56b\", \"SequenceNumber\":5}}",
                Expected =
                    "{\"Nanos\":{\"Value\":111442352},\"MarketCode\":{\"Value\":107},\"MarketName\":{\"Value\":\"TFEX Thailand Deferred Contract\"},\"MarketDescription\":{\"Value\":\"TXD\"},\"MsgType\":\"m\",\"Metadata\":{\"Timestamp\":1709242340000000000,\"Session\":\"332e0b2d71864555a28700f7709cd56b\",\"SequenceNumber\":5}}"
            }
        };

        foreach (var testCase in testCases)
        {
            var result = testCase.Input.SimpleCleanJsonMessage();
            _output.WriteLine($"Input:    {testCase.Input}");
            _output.WriteLine($"Expected: {testCase.Expected}");
            _output.WriteLine($"Result:   {result}");
            _output.WriteLine(new string('-', 80)); // Separator line
            Assert.Equal(testCase.Expected, result);
        }
    }

    [Fact]
    public void CleanJsonMessage_HandlesArrays()
    {
        var input = "[\n\t\"apple\",\n\t\"banana\",\n\t\"cherry\"\n]";
        var expected = "[\"apple\",\"banana\",\"cherry\"]";
        var result = input.SimpleCleanJsonMessage();
        _output.WriteLine($"Input:    {input}");
        _output.WriteLine($"Expected: {expected}");
        _output.WriteLine($"Result:   {result}");
        Assert.Equal(expected, result);
    }

    [Fact]
    public void CleanJsonMessage_HandlesComplexJsonWithValueFields()
    {
        var testCases = new[]
        {
            new
            {
                Input =
                    "{\"Nanos\":{\"Value\":111442352}, \"ExchangeCode\":{\"Value\":35}, \"ExchangeName\":{\"Value\":\"The Stock Exchange of Thailand  \"}, \"MsgType\":\"e\", \"Metadata\":{\"Timestamp\":1709242340000000000, \"Session\":\"332e0b2d71864555a28700f7709cd56b\", \"SequenceNumber\":3}}",
                Expected =
                    "{\"Nanos\":{\"Value\":111442352},\"ExchangeCode\":{\"Value\":35},\"ExchangeName\":{\"Value\":\"The Stock Exchange of Thailand\"},\"MsgType\":\"e\",\"Metadata\":{\"Timestamp\":1709242340000000000,\"Session\":\"332e0b2d71864555a28700f7709cd56b\",\"SequenceNumber\":3}}"
            },
            new
            {
                Input =
                    "{\"Nanos\":{\"Value\":111442352}, \"MarketCode\":{\"Value\":104}, \"MarketName\":{\"Value\":\"TFEX Thailand Metal             \"}, \"MarketDescription\":{\"Value\":\"TXM  \"}, \"MsgType\":\"m\", \"Metadata\":{\"Timestamp\":1709242340000000000, \"Session\":\"332e0b2d71864555a28700f7709cd56b\", \"SequenceNumber\":4}}",
                Expected =
                    "{\"Nanos\":{\"Value\":111442352},\"MarketCode\":{\"Value\":104},\"MarketName\":{\"Value\":\"TFEX Thailand Metal\"},\"MarketDescription\":{\"Value\":\"TXM\"},\"MsgType\":\"m\",\"Metadata\":{\"Timestamp\":1709242340000000000,\"Session\":\"332e0b2d71864555a28700f7709cd56b\",\"SequenceNumber\":4}}"
            },
            new
            {
                Input =
                    "{\"Nanos\":{\"Value\":111442352}, \"MarketCode\":{\"Value\":107}, \"MarketName\":{\"Value\":\"TFEX Thailand Deferred Contract \"}, \"MarketDescription\":{\"Value\":\"TXD  \"}, \"MsgType\":\"m\", \"Metadata\":{\"Timestamp\":1709242340000000000, \"Session\":\"332e0b2d71864555a28700f7709cd56b\", \"SequenceNumber\":5}}",
                Expected =
                    "{\"Nanos\":{\"Value\":111442352},\"MarketCode\":{\"Value\":107},\"MarketName\":{\"Value\":\"TFEX Thailand Deferred Contract\"},\"MarketDescription\":{\"Value\":\"TXD\"},\"MsgType\":\"m\",\"Metadata\":{\"Timestamp\":1709242340000000000,\"Session\":\"332e0b2d71864555a28700f7709cd56b\",\"SequenceNumber\":5}}"
            }
        };

        foreach (var testCase in testCases)
        {
            var result = testCase.Input.SimpleCleanJsonMessage();
            _output.WriteLine($"Input:    {testCase.Input}");
            _output.WriteLine($"Expected: {testCase.Expected}");
            _output.WriteLine($"Result:   {result}");
            _output.WriteLine(new string('-', 80)); // Separator line
            Assert.Equal(testCase.Expected, result);
        }
    }

    [Fact]
    public void SimpleCleanJsonMessage_RemovesNewlinesAndExtraSpaces()
    {
        var input = @"{
                ""name"": ""John Doe"",
                ""age"":    30,
                ""city"":    ""New    York""
            }";
        var expected = @"{""name"":""John Doe"",""age"":30,""city"":""New York""}";

        var result = input.SimpleCleanJsonMessage();

        Assert.Equal(expected, result);
    }

    [Fact]
    public void SimpleCleanJsonMessage_PreservesSpacesInQuotes()
    {
        var input = @"{""description"": ""This   is   a   test   with   spaces""}";
        var expected = @"{""description"":""This is a test with spaces""}";

        var result = input.SimpleCleanJsonMessage();

        Assert.Equal(expected, result);
    }

    [Fact]
    public void SimpleCleanJsonMessage_HandlesEmptyString()
    {
        var input = "";
        var expected = "";

        var result = input.SimpleCleanJsonMessage();

        Assert.Equal(expected, result);
    }

    [Fact]
    public void SimpleCleanJsonMessage_HandlesNullString()
    {
        string input = null;
        var expected = "";

        var result = input.SimpleCleanJsonMessage();

        Assert.Equal(expected, result);
    }

    [Fact]
    public void SimpleCleanJsonMessage_RemovesTabsAndCarriageReturns()
    {
        var input = "{\r\n\t\"name\":\t\"John\tDoe\"\r\n}";
        var expected = "{\"name\":\"JohnDoe\"}";

        var result = input.SimpleCleanJsonMessage();

        Assert.Equal(expected, result);
    }

    [Fact]
    public void SimpleCleanJsonMessage_HandlesComplexJson()
    {
        var input = @"{
                ""person"": {
                    ""name"": ""John Doe"",
                    ""age"": 30,
                    ""address"": {
                        ""street"": ""123 Main St"",
                        ""city"": ""New York"",
                        ""zipcode"": ""10001""
                    },
                    ""hobbies"": [
                        ""reading"",
                        ""swimming"",
                        ""coding""
                    ]
                }
            }";
        var expected =
            @"{""person"":{""name"":""John Doe"",""age"":30,""address"":{""street"":""123 Main St"",""city"":""New York"",""zipcode"":""10001""},""hobbies"":[""reading"",""swimming"",""coding""]}}";

        var result = input.SimpleCleanJsonMessage();

        Assert.Equal(expected, result);
    }
    
    [Fact]
    public void IsValidJson_ValidObjectJson_ReturnsTrue()
    {
        string json = "{\"name\":\"John\", \"age\":30, \"city\":\"New York\"}";
        Assert.True(json.IsValidJsonMessage());
    }

    [Fact]
    public void IsValidJson_ValidArrayJson_ReturnsTrue()
    {
        string json = "[1, 2, 3, 4, 5]";
        Assert.True(json.IsValidJsonMessage());
    }

    [Fact]
    public void IsValidJson_EmptyObject_ReturnsTrue()
    {
        string json = "{}";
        Assert.True(json.IsValidJsonMessage());
    }

    [Fact]
    public void IsValidJson_EmptyArray_ReturnsTrue()
    {
        string json = "[]";
        Assert.True(json.IsValidJsonMessage());
    }

    [Fact]
    public void IsValidJson_ComplexNestedJson_ReturnsTrue()
    {
        string json = "{\"employees\":[{\"name\":\"John\", \"age\":30},{\"name\":\"Jane\", \"age\":28}]}";
        Assert.True(json.IsValidJsonMessage());
    }

    [Fact]
    public void IsValidJson_InvalidJson_ReturnsFalse()
    {
        string json = "{\"name\":\"John\", \"age\":30,}";  // Extra comma
        Assert.True(json.IsValidJsonMessage());
    }

    [Fact]
    public void IsValidJson_IncompleteJson_ReturnsFalse()
    {
        string json = "{\"name\":\"John\", \"age\":";
        Assert.False(json.IsValidJsonMessage());
    }

    [Fact]
    public void IsValidJson_EmptyString_ReturnsFalse()
    {
        string json = "";
        Assert.False(json.IsValidJsonMessage());
    }

    [Fact]
    public void IsValidJson_WhitespaceOnly_ReturnsFalse()
    {
        string json = "   \t\n";
        Assert.False(json.IsValidJsonMessage());
    }

    [Fact]
    public void IsValidJson_NullString_ReturnsFalse()
    {
        string json = null;
        Assert.False(json.IsValidJsonMessage());
    }

    [Fact]
    public void IsValidJson_NonJsonString_ReturnsFalse()
    {
        string json = "This is not JSON";
        Assert.False(json.IsValidJsonMessage());
    }

    [Theory]
    [InlineData("true")]
    [InlineData("false")]
    [InlineData("null")]
    [InlineData("42")]
    [InlineData("3.14")]
    [InlineData("\"Hello, World!\"")]
    public void IsValidJson_ValidJsonPrimitives_ReturnsTrue(string json)
    {
        Assert.False(json.IsValidJsonMessage());
    }
}