using AutoFixture;
using Newtonsoft.Json;
using Pi.SetMarketData.Application.Interfaces.ItchMapper;
using Pi.SetMarketData.Application.Models.ItchMessageWrapper;
using Pi.SetMarketData.Application.Services.ItchMapper;

namespace Pi.SetMarketData.Application.Tests.ItchMapper
{
    public class ItchOrderBookDirectoryMapperServiceTests
    {
        private readonly IItchOrderBookDirectoryMapperService _service;

        public ItchOrderBookDirectoryMapperServiceTests()
        {
            _service = new ItchOrderBookDirectoryService();
        }

        // MapToInstrument
        [Fact]
        public void MapToInstrument_ShouldReturnNull_WhenOrderBookDirectoryMessageIsNull()
        {
            // Act
            var result = _service.MapToInstrument(null, null);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void MapToInstrument_ShouldReturnValidResponse_WhenOrderBookDirectoryMessageIsNotNull()
        {
            // Arrange
            // Load JSON file and deserialize
            var json = File.ReadAllText("ItchMapper/Mocks/ItchOrderBookDirectoryMappingService/order_book_directory_message.json");

            // Convert message
            var convertedResult = JsonConvert.DeserializeObject<OrderBookDirectoryMessageWrapper>(json);

            // Act
            var result = _service.MapToInstrument(convertedResult, null);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("AAPL", result.Symbol);
            Assert.Equal(1001, result.OrderBookId);
            Assert.Equal("Thai Stocks", result.InstrumentCategory);
        }

        [Fact]
        public void MapToInstrument_ShouldReturnValidResponse_WhenOrderBookDirectoryMessageIsNotNull_Unstructered_Case()
        {
            // Arrange
            // Load JSON file and deserialize
            var json = File.ReadAllText("ItchMapper/Mocks/ItchOrderBookDirectoryMappingService/order_book_directory_message.json");

            // Convert message
            var convertedResult = JsonConvert.DeserializeObject<OrderBookDirectoryMessageWrapper>(json);
            convertedResult!.MarketSegment.Value = "Mocky Mocky";

            // Act
            var result = _service.MapToInstrument(convertedResult, null);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("AAPL", result.Symbol);
            Assert.Equal(1001, result.OrderBookId);
            Assert.Equal("Unstructured", result.InstrumentCategory);
        }

        [Fact]
        public void MapToInstrument_ShouldReturnValidResponse_WhenOrderBookDirectoryMessageIsNotNull_Ignored_Case()
        {
            // Arrange
            // Load JSON file and deserialize
            var json = File.ReadAllText("ItchMapper/Mocks/ItchOrderBookDirectoryMappingService/order_book_directory_message.json");

            // Convert message
            var convertedResult = JsonConvert.DeserializeObject<OrderBookDirectoryMessageWrapper>(json);
            convertedResult!.FinancialProduct.Value = "UL";

            // Act
            var result = _service.MapToInstrument(convertedResult, null);


            // Assert
            Assert.Null(result);
        }

        // MapToInstrumentDetail
        [Fact]
        public void MapToInstrumentDetail_ShouldReturnNull_WhenOrderBookDirectoryMessageIsNull()
        {
            // Act
            var result = _service.MapToInstrumentDetail(null);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void MapToInstrumentDetail_ShouldReturnValidResponse_WhenOrderBookDirectoryMessageIsNotNull()
        {
            // Arrange
            // Load JSON file and deserialize
            var json = File.ReadAllText("ItchMapper/Mocks/ItchOrderBookDirectoryMappingService/order_book_directory_message.json");

            // Convert message
            var convertedResult = JsonConvert.DeserializeObject<OrderBookDirectoryMessageWrapper>(json);

            // Act
            var result = _service.MapToInstrumentDetail(convertedResult);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("2", result.DecimalsInPrice.ToString());
            Assert.Equal(2, result.DecimalInStrikePrice);
            Assert.Equal("2", result.DecimalInContractSizePQF.ToString());
            Assert.Equal("1550", result?.StrikePrice?.ToString() ?? "");
            Assert.Equal("100", result?.ContractSize?.ToString() ?? "");
            Assert.Equal("1001", result?.InstrumentId.ToString() ?? "");
        }

        // MapToOrderBook
        [Fact]
        public void MapToOrderBook_ShouldReturnNull_WhenOrderBookDirectoryMessageIsNull()
        {
            // Act
            var result = _service.MapToOrderBook(null);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void MapToOrderBook_ShouldReturnValidResponse_WhenOrderBookDirectoryMessageIsNotNull()
        {
            // Arrange
            // Load JSON file and deserialize
            var json = File.ReadAllText("ItchMapper/Mocks/ItchOrderBookDirectoryMappingService/order_book_directory_message.json");

            // Convert message
            var convertedResult = JsonConvert.DeserializeObject<OrderBookDirectoryMessageWrapper>(json);

            // Act
            var result = _service.MapToOrderBook(convertedResult);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("AAPL", result.Symbol);
            Assert.Equal(1001, result.OrderBookId);
        }

        // MapToWhiteList
        [Fact]
        public void MapToWhiteList_ShouldReturnNull_WhenOrderBookDirectoryMessageIsNull()
        {
            // Act
            var result = _service.MapToWhiteList(null, null);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void MapToWhiteList_ShouldReturnValidResponse_WhenOrderBookDirectoryMessageIsNotNull_And_ExchangeServer_Is_SET()
        {
            // Arrange
            // Load JSON file and deserialize
            var json = File.ReadAllText("ItchMapper/Mocks/ItchOrderBookDirectoryMappingService/order_book_directory_message.json");

            // Convert message
            var convertedResult = JsonConvert.DeserializeObject<OrderBookDirectoryMessageWrapper>(json);

            // Act
            var result = _service.MapToWhiteList(convertedResult, true);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Thai Stocks", result.Symbol);
            Assert.Equal(true, result.IsWhitelist);
            Assert.Equal("BKK", result.Mic);
            Assert.Equal("Thai Stocks", result.StandardTicker);
            Assert.Equal("SET", result.Exchange);
        }

        [Fact]
        public void MapToWhiteList_ShouldReturnValidResponse_WhenOrderBookDirectoryMessageIsNotNull_And_ExchangeServer_Is_TFEX()
        {
            // Arrange
            // Load JSON file and deserialize
            var json = File.ReadAllText("ItchMapper/Mocks/ItchOrderBookDirectoryMappingService/order_book_directory_message.json");

            // Convert message
            var convertedResult = JsonConvert.DeserializeObject<OrderBookDirectoryMessageWrapper>(json);

            // Act
            var result = _service.MapToWhiteList(convertedResult, false);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Thai Stocks", result.Symbol);
            Assert.Equal(true, result.IsWhitelist);
            Assert.Equal("BKK", result.Mic);
            Assert.Equal("Thai Stocks", result.StandardTicker);
            Assert.Equal("TFEX", result.Exchange);
        }

        [Fact]
        public void MapToDatabase_ShouldMapCorrectCurrency_ForIDX_Type()
        {
            // Arrange
            var fixture = new Fixture();
            var message = fixture.Build<OrderBookDirectoryMessageWrapper>()
            .With(x => x.FinancialProduct, new FinancialProduct { Value = "IDX" }).Create();
            var msg = new OrderBookDirectoryMessageWrapper {
                MarketSegment = new MarketSegment { Value = "IDX"}
            };

            // Act
            var result = _service.MapToInstrument(message, null);
        
            // Assert
            Assert.NotNull(result);
            Assert.Equal("points", result.Currency);
        }
        
        [Fact]
        public void MapToDatabase_ShouldMapCorrectCurrency()
        {
            // Arrange
            var fixture = new Fixture();
            var message = fixture.Build<OrderBookDirectoryMessageWrapper>().Create();

            // Act
            var result = _service.MapToInstrument(message, null);
        
            // Assert
            Assert.NotNull(result);
            Assert.Equal(message.TradingCurrency.Value, result.Currency);
        }
    }
}
