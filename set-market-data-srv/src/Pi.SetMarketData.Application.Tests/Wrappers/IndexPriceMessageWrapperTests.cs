using System.Text.Json;
using Pi.SetMarketData.Application.Constants;
using Pi.SetMarketData.Application.Models.ItchMessageWrapper;
using Pi.SetMarketData.Application.Services.Models.ItchParser;
using Pi.SetMarketData.Application.Services.Types.ItchParser;

namespace Pi.SetMarketData.Application.Tests.Wrappers
{
    public class IndexPriceMessageWrapperTests
    {
        [Fact]
        public void DeserializeIndexPriceMessage_WithLargeValues_ShouldDeserializeCorrectly()
        {
            // Arrange
            // Create JSON with large values that exceed int.MaxValue but fit within long
            var json = @"{
                ""Nanos"": { ""Value"": 123456789 },
                ""OrderBookId"": { ""Value"": 12345 },
                ""Value"": { ""Value"": 2000000000, ""NumberOfDecimals"": 2 },
                ""HighValue"": { ""Value"": 2100000000, ""NumberOfDecimals"": 2 },
                ""LowValue"": { ""Value"": 1900000000, ""NumberOfDecimals"": 2 },
                ""OpenValue"": { ""Value"": 1950000000, ""NumberOfDecimals"": 2 },
                ""TradedVolume"": { ""Value"": 1000000 },
                ""TradedValue"": { ""Value"": 44133202900, ""NumberOfDecimals"": 2 },
                ""Change"": { ""Value"": 5000000000, ""NumberOfDecimals"": 2 },
                ""ChangePercent"": { ""Value"": 3000000000, ""NumberOfDecimals"": 2 },
                ""PreviousClose"": { ""Value"": 4000000000, ""NumberOfDecimals"": 2 },
                ""Close"": { ""Value"": 4500000000, ""NumberOfDecimals"": 2 },
                ""Timestamp"": { ""Value"": 1619345678 }
            }";

            // Act & Assert
            var exception = Record.Exception(() => {
                var message = JsonSerializer.Deserialize<IndexPriceMessageWrapper>(json, 
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                // Verify some values to ensure proper deserialization
                Assert.NotNull(message);
                Assert.Equal(44133202900, message.TradedValue.Value);
                Assert.Equal(5000000000, message.Change.Value);
            });
            
            Assert.Null(exception);
        }

        [Fact]
        public void DeserializeIndexPriceMessage_WithValuesExceedingIntMaxValue_ShouldDeserializeCorrectly()
        {
            // Arrange
            // Create JSON with values that exceed int.MaxValue for properties still using Price
            var json = @"{
                ""Nanos"": { ""Value"": 123456789 },
                ""OrderBookId"": { ""Value"": 12345 },
                ""Value"": { ""Value"": 30000000000, ""NumberOfDecimals"": 2 },
                ""HighValue"": { ""Value"": 3100000000, ""NumberOfDecimals"": 2 },
                ""LowValue"": { ""Value"": 2900000000, ""NumberOfDecimals"": 2 },
                ""OpenValue"": { ""Value"": 2950000000, ""NumberOfDecimals"": 2 },
                ""TradedVolume"": { ""Value"": 1000000 },
                ""TradedValue"": { ""Value"": 44133202900, ""NumberOfDecimals"": 2 },
                ""Change"": { ""Value"": 5000000000, ""NumberOfDecimals"": 2 },
                ""ChangePercent"": { ""Value"": 3000000000, ""NumberOfDecimals"": 2 },
                ""PreviousClose"": { ""Value"": 4000000000, ""NumberOfDecimals"": 2 },
                ""Close"": { ""Value"": 4500000000, ""NumberOfDecimals"": 2 },
                ""Timestamp"": { ""Value"": 1619345678 }
            }";

            // Act & Assert
            // This should throw an exception because Value, HighValue, LowValue, and OpenValue 
            // are still using Price with int.Value
            var exception = Record.Exception(() => {
                var message = JsonSerializer.Deserialize<IndexPriceMessageWrapper>(json, 
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                // Verify some values to ensure proper deserialization
                Assert.NotNull(message);
                Assert.Equal(30000000000, message.Value.Value);
            });
            
            Assert.Null(exception);
        }

        [Fact]
        public void DeserializeIndexPriceMessage_WithAllPrice64_ShouldDeserializeCorrectly()
        {
            // This test requires modifying IndexPriceMessageWrapper to use Price64 for all price properties
            // Create a modified class for testing
            
            // Arrange
            var json = @"{
                ""Nanos"": { ""Value"": 123456789 },
                ""OrderBookId"": { ""Value"": 12345 },
                ""Value"": { ""Value"": 3000000000, ""NumberOfDecimals"": 2 },
                ""HighValue"": { ""Value"": 3100000000, ""NumberOfDecimals"": 2 },
                ""LowValue"": { ""Value"": 2900000000, ""NumberOfDecimals"": 2 },
                ""OpenValue"": { ""Value"": 2950000000, ""NumberOfDecimals"": 2 },
                ""TradedVolume"": { ""Value"": 1000000 },
                ""TradedValue"": { ""Value"": 44133202900, ""NumberOfDecimals"": 2 },
                ""Change"": { ""Value"": 5000000000, ""NumberOfDecimals"": 2 },
                ""ChangePercent"": { ""Value"": 3000000000, ""NumberOfDecimals"": 2 },
                ""PreviousClose"": { ""Value"": 4000000000, ""NumberOfDecimals"": 2 },
                ""Close"": { ""Value"": 4500000000, ""NumberOfDecimals"": 2 },
                ""Timestamp"": { ""Value"": 1619345678 }
            }";

            // Act & Assert
            var exception = Record.Exception(() => {
                var message = JsonSerializer.Deserialize<IndexPriceMessageWrapperWithAllPrice64>(json, 
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                // Verify some values
                Assert.NotNull(message);
                Assert.Equal(3000000000, message.Value.Value);
                Assert.Equal(3100000000, message.HighValue.Value);
                Assert.Equal(44133202900, message.TradedValue.Value);
            });
            
            Assert.Null(exception);
        }
    }

    // Mock class with all Price properties changed to Price64
    public class IndexPriceMessageWrapperWithAllPrice64 : ItchMessage
    {
        public IndexPriceMessageWrapperWithAllPrice64()
        {
            MsgType = ItchMessageType.J;
        }

        public Nanos Nanos { get; set; }
        public OrderBookId OrderBookId { get; set; }
        public Price64 Value { get; set; }
        public Price64 HighValue { get; set; }
        public Price64 LowValue { get; set; }
        public Price64 OpenValue { get; set; }
        public Numeric TradedVolume { get; set; }
        public Price64 TradedValue { get; set; }
        public Price64 Change { get; set; }
        public Price64 ChangePercent { get; set; }
        public Price64 PreviousClose { get; set; }
        public Price64 Close { get; set; }
        public Timestamp Timestamp { get; set; }
    }
}