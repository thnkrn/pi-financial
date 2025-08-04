using Pi.SetMarketData.Application.Services.MarketData.MarketOrderBook;
using Pi.SetMarketData.Domain.Models.Response;
using Pi.SetMarketData.Application.Tests.ItchMapper.Mocks.ItchOrderBookMappingService;

namespace Pi.SetMarketData.Application.Tests.Services.MarketOrderBook
{
    public class MarketOrderBookTest
    {
        [Fact]
        public void GetMarketOrderBook_ReturnsOrderBook()
        {
            // Arrange
            var mockOrderBook = MockOrderBook.GetMockOrderBooks();
            var mockVenues = MockOrderBook.GetMockVenues();
            var mockStreamingResponse = MockOrderBook.GetMockMarketStreamingResponses();
            var mockInstrumentDetails = MockOrderBook.GetMockInstrumentDetail();

            // Act
            // var result = MarketOrderBookService.GetResult(mockOrderBook, mockVenues, mockStreamingResponse, mockInstrumentDetails);
            //
            // // Assert
            // Assert.NotNull(result);
            // Assert.IsType<MarketOrderBookResponse>(result);
            // Assert.Equal("0", result.Code);
            // Assert.Equal("", result.Message);
            // Assert.Equal("AAPL", result?.Response?.BidOfferList?[0]?.Symbol);
            // Assert.Equal("135.67", result?.Response?.BidOfferList?[0]?.Bid);
            // Assert.Equal("135.80", result?.Response?.BidOfferList?[0]?.Offer);
        }
    }
}