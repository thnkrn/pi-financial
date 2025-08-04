using System.Text;
using Newtonsoft.Json;
using Pi.SetMarketDataWSS.Application.Services.ItchMapper;
using Pi.SetMarketDataWSS.Application.Services.Models.ItchOrderBookMapper;
using Pi.SetMarketDataWSS.Application.Services.Types.ItchParser;
using Pi.SetMarketDataWSS.Domain.Entities;

namespace Pi.SetMarketData.Application.Tests.ItchMapper;

public class ItchOrderBookMapperServiceTests
{
    private readonly ItchOrderBookMapperService _service;

    public ItchOrderBookMapperServiceTests()
    {
        _service = new ItchOrderBookMapperService();
    }

    [Fact]
    public void Map_ShouldReturnDefaultOrderbook_WhenMessageIsNull()
    {
        // Act
        var result = _service.Map(null, null);

        // Assert
        Assert.IsType<OrderBook>(result);
    }

    [Fact]
    public void Map_ShouldMapCorrectly_WhenValidMessageIsPassed()
    {
        // Arrange
        // Load JSON file and deserialize
        var json = File.ReadAllText("ItchMapper/Mocks/ItchOrderBookMapperService/market_price_level_message.json");

        // Deserialize the actual message content
        var marketByPriceLevelUpdates = JsonConvert.DeserializeObject<MarketByPriceLevelWrapper>(json ?? "");
        ;

        // Assert before mapping
        Assert.NotNull(marketByPriceLevelUpdates);
        Assert.Equal("1", marketByPriceLevelUpdates?.OrderBookID.Value.ToString());
        Assert.Equal("12345678", marketByPriceLevelUpdates?.Nanos.Value.ToString());
        Assert.Equal("5", marketByPriceLevelUpdates?.MaximumLevel.ToString());
        Assert.Equal("1", marketByPriceLevelUpdates?.NumberOfLevelItems.ToString());


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
        var json = File.ReadAllText("ItchMapper/Mocks/ItchOrderBookMapperService/market_price_level_message.json");

        // Mock OrderBook
        var mockOrderBook = MockData.GetMockOrderBook();

        // Deserialize the actual message content
        var marketByPriceLevelUpdates = JsonConvert.DeserializeObject<MarketByPriceLevelWrapper>(json ?? "");


        // Act
        var result = _service.Map(marketByPriceLevelUpdates!, mockOrderBook);

        Assert.Equal(1, result.OrderBookId);
        Assert.Equal("AAPL", result.Symbol);
        Assert.Equal(12345, result.InstrumentId);
        Assert.Equal("200", result.BidPrice); // First level of bid price
        Assert.Equal("10", result.BidQuantity); // First level of bid quantity
        Assert.Equal("200", result.OfferPrice); // First level of offer price
        Assert.Equal("10", result.OfferQuantity); // First level of offer quantity
        Assert.Equal(4, result?.Bid?.Count);
        Assert.Equal(4, result?.Offer?.Count);
        Assert.Equal("200", result?.Bid[0].Price); // Check price on first item in bid array
        Assert.Equal("10", result?.Bid[0].Quantity); // Check quantity on first item in bid array
        Assert.Equal("135.30", result?.Bid.Last().Price); // Check price on last item in bid array
        Assert.Equal("200", result?.Bid.Last().Quantity); // Check quantity on last item in bid array
    }

    [Fact]
    public void Map_ShouldMapCorrectly_UpdateOrderBook()
    {
        // Arrange
        var mockOrderBook = new OrderBook
        {
            Bid = [
                new BidAsk {
                    Price = "5750000",
                    Quantity = "792500"
                },
                new BidAsk {
                    Price = "5775000",
                    Quantity = "61900"
                },
                new BidAsk {
                    Price = "5825000",
                    Quantity = "43400"
                },
                new BidAsk {
                    Price = "6625000",
                    Quantity = "25100"
                },

            ],
            Offer = [
                new BidAsk {
                    Price = "5700000",
                    Quantity = "420500"
                },
                new BidAsk {
                    Price = "5775000",
                    Quantity = "238000"
                },
                new BidAsk {
                    Price = "5780000",
                    Quantity = "260900"
                },
                new BidAsk {
                    Price = "5800000",
                    Quantity = "241400"
                },

            ],
        };
        byte[] side = Encoding.GetEncoding("ISO-8859-1").GetBytes("B");
        byte[] action = Encoding.GetEncoding("ISO-8859-1").GetBytes("C");
        int fieldLength = 1;

        var marketByPriceLevelUpdates = new MarketByPriceLevelWrapper
        {
            MaximumLevel = new Numeric8(10),
            PriceLevelUpdates = [
                new PriceLevelUpdate {
                    Side = new Alpha(side, fieldLength),
                    UpdateAction = new Alpha(action, fieldLength),
                    Level = new Numeric8(1),
                    Price= new Price64(5750000),
                    Quantity = new Numeric64([0, 0, 0, 0, 0, 0, 0, (byte)100])
                }
            ]
        };


        // Act
        var result = _service.Map(marketByPriceLevelUpdates, mockOrderBook);

        // Assert
        Assert.Equal("5750000", result.BidPrice); // First level of bid price
        Assert.Equal("100", result.BidQuantity); // First level of bid quantity
        Assert.Equal(4, result?.Bid?.Count);
        Assert.Equal(4, result?.Offer?.Count);
    }
}