using Pi.SetMarketDataWSS.Infrastructure.Interfaces.Redis;
using Pi.SetMarketDataWSS.DataSubscriber.BidOfferMapper;

namespace Pi.SetMarketDataWSS.DataSubscriber.Tests.BidOfferMapper;


public class BidOfferMapperTest
{
    private readonly Mock<IRedisV2Publisher> _redisPublisher;
    private readonly BidOfferService bidOfferService;

    public BidOfferMapperTest()
    {
        _redisPublisher = new Mock<IRedisV2Publisher>();
        bidOfferService = new BidOfferService(_redisPublisher.Object);
    }

    [Fact]
    public void Init_BidOfferArray_Return_Correct_Array()
    {
        // Arrange
        var (initBids, initOffers) = BidOfferService.InitBidOfferArray(10);

        // Act

        // Assert
        Assert.Equal(new List<List<string>> {
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
        }, initBids);
        Assert.Equal(new List<List<string>> {
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
        }, initOffers);
    }

    [Fact]
    public void CalculateBidOfferArray_Return_Correct_Array()
    {

        var Orderbook = "CPALL";
        var updateMembers = new List<BidOfferItem>
        {
            new BidOfferItem { A = "N", S = "B", L = 1, P = 4050, Q = 100, Sq = 1, O = Orderbook },
            new BidOfferItem { A = "N", S = "B", L = 2, P = 4000, Q = 100, Sq = 1, O = Orderbook },
            new BidOfferItem { A = "N", S = "A", L = 1, P = 3900, Q = 100, Sq = 1, O = Orderbook },
            new BidOfferItem { A = "N", S = "A", L = 2, P = 3850, Q = 100, Sq = 1, O = Orderbook }
        };

        var (initBids, initOffers) = BidOfferService.InitBidOfferArray(10);
        var (bids, offers) = BidOfferService.CalculateBidOfferArray(initBids, initOffers, updateMembers, 10);

        Assert.Equal(new List<List<string>>
        {
            new List<string> { "4050.00", "100.00" },
            new List<string> { "4000.00", "100.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
        }, bids);
        Assert.Equal(new List<List<string>>
        {
            new List<string> { "3900.00", "100.00" },
            new List<string> { "3850.00", "100.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
        }, offers);
    }

    [Fact]
    public void CalculateBidOfferArray_With_New_Bid_Offer_Return_Correct_Array()
    {
        // Arrange
        var initBids = new List<List<string>> {
            new List<string> { "4050.00", "100.00" },
            new List<string> { "4000.00", "100.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
        };

        var initOffers = new List<List<string>> {
            new List<string> { "3900.00", "100.00" },
            new List<string> { "3850.00", "100.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
        };

        var Orderbook = "CPALL";
        var updateMembers = new List<BidOfferItem>
        {
            new BidOfferItem { A = "N", S = "B", L = 1, P = 4100, Q = 999, Sq = 2, O = Orderbook },
            new BidOfferItem { A = "N", S = "A", L = 2, P = 3875, Q = 999, Sq = 2, O = Orderbook },
        };

        var (bids, offers) = BidOfferService.CalculateBidOfferArray(initBids, initOffers, updateMembers, 10);

        Assert.Equal(new List<List<string>>
        {
            new List<string> { "4100.00", "999.00" },
            new List<string> { "4050.00", "100.00" },
            new List<string> { "4000.00", "100.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
        }, bids);
        Assert.Equal(new List<List<string>>
        {
            new List<string> { "3900.00", "100.00" },
            new List<string> { "3875.00", "999.00" },
            new List<string> { "3850.00", "100.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
        }, offers);
    }

    [Fact]
    public void CalculateBidOfferArray_With_Change_Bid_Offer_Return_Correct_Array()
    {
        // Arrange
        var initBids = new List<List<string>> {
            new List<string> { "4050.00", "100.00" },
            new List<string> { "4000.00", "100.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "3800.00", "1000.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
        };

        var initOffers = new List<List<string>> {
            new List<string> { "3900.00", "100.00" },
            new List<string> { "3850.00", "100.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "3800.00", "1000.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
        };

        var Orderbook = "CPALL";
        var updateMembers = new List<BidOfferItem>
        {
            new BidOfferItem { A = "C", S = "B", L = 8, P = 3775, Q = 1000, Sq = 2, O = Orderbook },
            new BidOfferItem { A = "C", S = "A", L = 5, P = 3775, Q = 1000, Sq = 2, O = Orderbook },
        };

        var (bids, offers) = BidOfferService.CalculateBidOfferArray(initBids, initOffers, updateMembers, 10);

        Assert.Equal(new List<List<string>>
        {
            new List<string> { "4050.00", "100.00" },
            new List<string> { "4000.00", "100.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "3775.00", "1000.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
        }, bids);
        Assert.Equal(new List<List<string>>
        {
            new List<string> { "3900.00", "100.00" },
            new List<string> { "3850.00", "100.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "3775.00", "1000.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
        }, offers);
    }

    [Fact]
    public void CalculateBidOfferArray_With_Delete_Bid_Offer_Return_Correct_Array()
    {
        // Arrange
        var initBids = new List<List<string>> {
            new List<string> { "4050.00", "100.00" },
            new List<string> { "4000.00", "100.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "3800.00", "1000.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
        };

        var initOffers = new List<List<string>> {
            new List<string> { "3900.00", "100.00" },
            new List<string> { "3850.00", "100.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "3800.00", "1000.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
        };

        var Orderbook = "CPALL";
        var updateMembers = new List<BidOfferItem>
        {
            new BidOfferItem { A = "D", S = "B", L = 8, P = -2147483648, Q = 0, Sq = 2, O = Orderbook },
            new BidOfferItem { A = "D", S = "A", L = 5, P = -2147483648, Q = 0, Sq = 2, O = Orderbook },
        };

        var (bids, offers) = BidOfferService.CalculateBidOfferArray(initBids, initOffers, updateMembers, 10);

        Assert.Equal(new List<List<string>>
        {
            new List<string> { "4050.00", "100.00" },
            new List<string> { "4000.00", "100.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
        }, bids);
        Assert.Equal(new List<List<string>>
        {
            new List<string> { "3900.00", "100.00" },
            new List<string> { "3850.00", "100.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
        }, offers);
    }

    [Fact]
    public void CalculateBidOfferArray_With_Delete_Bid_Offer_Followed_By_New_Bid_Offer_Return_Correct_Array()
    {
        // Example:
        // 153294,<bound method Arrow.to of <Arrow [2024-03-01T09:31:46+07:00]>>,765545599,3867423,10,3,"
        // [
        //     {'level_update_action': b'D', 'side': 'B', 'level': 1, 'price': -2147483648, 'quantity': 0, 'number_of_delete': 2}, 
        //     {'level_update_action': b'N', 'side': 'B', 'level': 1, 'price': 148000, 'quantity': 13182000, 'number_of_delete': 0}, 
        //     {'level_update_action': b'N', 'side': 'B', 'level': 10, 'price': 3000, 'quantity': 200000, 'number_of_delete': 0}
        // ]

        // Arrange
        var initBids = new List<List<string>> {
            new List<string> { "4050.00", "100.00" },
            new List<string> { "4000.00", "100.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "3800.00", "1000.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
        };

        var initOffers = new List<List<string>> {
            new List<string> { "3900.00", "100.00" },
            new List<string> { "3850.00", "100.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "3800.00", "1000.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
        };

        var Orderbook = "CPALL";
        var updateMembers = new List<BidOfferItem>
        {
            new BidOfferItem { A = "D", S = "B", L = 1, P = -2147483648, Q = 0, Sq = 2, O = Orderbook },
            new BidOfferItem { A = "N", S = "B", L = 1, P = 4100, Q = 100000, Sq = 2, O = Orderbook },
            new BidOfferItem { A = "D", S = "A", L = 1, P = -2147483648, Q = 0, Sq = 2, O = Orderbook },
            new BidOfferItem { A = "N", S = "A", L = 1, P = 3950, Q = 100000, Sq = 2, O = Orderbook },
        };

        var (bids, offers) = BidOfferService.CalculateBidOfferArray(initBids, initOffers, updateMembers, 10);

        Assert.Equal(new List<List<string>>
        {
            new List<string> { "4100.00", "100000.00" },
            new List<string> { "4000.00", "100.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "3800.00", "1000.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
        }, bids);
        Assert.Equal(new List<List<string>>
        {
            new List<string> { "3950.00", "100000.00" },
            new List<string> { "3850.00", "100.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "3800.00", "1000.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
            new List<string> { "0.00", "0.00" },
        }, offers);
    }
}
