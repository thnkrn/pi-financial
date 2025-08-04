
using Pi.SetMarketData.Application.Services.ItchMapper;
using Newtonsoft.Json;
using Pi.SetMarketData.Application.Models.ItchMessageWrapper;
using Pi.SetMarketData.Application.Tests.ItchMapper.Mocks.ItchOrderBookMappingService;


namespace Pi.SetMarketData.Application.Tests.ItchMapper;
public class ItchOrderBookMapperServiceTests
{
    private readonly ItchOrderBookMapperService _service;

    public ItchOrderBookMapperServiceTests()
    {
        _service = new ItchOrderBookMapperService();
    }

    [Fact]
    public void Map_ShouldReturnNull_WhenMessageIsNull()
    {
        // Act
        var result = _service.Map(null, null);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Map_ShouldMapCorrectly_WhenValidMessageIsPassed()
    {
        // Arrange
        // Load JSON file and deserialize
        var json = File.ReadAllText("ItchMapper/Mocks/ItchOrderBookMappingService/market_by_price_level_message.json");

        // Deserialize the actual message content
        var marketByPriceLevelUpdates = JsonConvert.DeserializeObject<MarketByPriceLevelWrapper>(json ?? "");

        // Assert before mapping
        Assert.NotNull(marketByPriceLevelUpdates);
        Assert.Equal("1", marketByPriceLevelUpdates.OrderBookID.Value.ToString());
        Assert.Equal("12345678", marketByPriceLevelUpdates.Nanos.Value.ToString());
        Assert.Equal("5", marketByPriceLevelUpdates.MaximumLevel.ToString());
        Assert.Equal("1", marketByPriceLevelUpdates.NumberOfLevelItems.ToString());


        // Act
        var result = _service.Map(marketByPriceLevelUpdates!, null);

        // Assert after mapping
        Assert.NotNull(result);
        Assert.Equal("1", result.OrderBookId.ToString());
        Assert.Single(result.Bid); // Ensure there's one bid
        Assert.Single(result.Offer); // Ensure there's one offer
        Assert.Equal("200", result.BidPrice);

        var bid = result.Bid[0]; // Checking first value of bid
        Assert.Equal("200", bid.Price);
        Assert.Equal("10", bid.Quantity);

        var offer = result.Offer[0]; // Checking first value of offer
        Assert.Equal("200", offer.Price);
        Assert.Equal("10", offer.Quantity);
    }

    [Fact]
    public void Map_ShouldMapCorrectly_WhenStoredValueIsPassed()
    {
        // Arrange
        // Load JSON file and deserialize
        var json = File.ReadAllText("ItchMapper/Mocks/ItchOrderBookMappingService/market_by_price_level_message.json");

        // Mock OrderBook
        var mockOrderBook = MockOrderBook.GetMockOrderBook();

        // Deserialize the actual message content
        var marketByPriceLevelUpdates = JsonConvert.DeserializeObject<MarketByPriceLevelWrapper>(json ?? "");


        // Act
        var result = _service.Map(marketByPriceLevelUpdates!, mockOrderBook);

        Assert.Equal(1, result?.OrderBookId);
        Assert.Equal("AAPL", result?.Symbol);
        Assert.Equal(12345, result?.InstrumentId);
        Assert.Equal("200", result?.BidPrice); // First level of bid price
        Assert.Equal("10", result?.BidQuantity); // First level of bid quantity
        Assert.Equal("200", result?.OfferPrice); // First level of offer price
        Assert.Equal("10", result?.OfferQuantity); // First level of offer quantity
        Assert.Equal(4, result?.Bid.Count);
        Assert.Equal(4, result?.Offer.Count);
        Assert.Equal("200", result?.Bid[0].Price); // Check price on first item in bid array
        Assert.Equal("10", result?.Bid[0].Quantity); // Check quantity on first item in bid array
        Assert.Equal("135.30", result?.Bid.Last().Price); // Check price on last item in bid array
        Assert.Equal("200", result?.Bid.Last().Quantity); // Check quantity on last item in bid array
    }
}