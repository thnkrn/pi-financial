using Pi.SetMarketDataWSS.Domain.Entities;
using Xunit;
namespace Pi.SetMarketDataWSS.Domain.Tests.Entities
{
    public class OrderBookTest
    {
        [Fact]
        public void OrderBook_InitializesCorrectly()
        {
            var orderBook = new OrderBook();

            Assert.Equal(0, orderBook.OrderBookId);
            Assert.Equal(0, orderBook.InstrumentId);
            Assert.Null(orderBook.Symbol);
            Assert.Null(orderBook.BidPrice);
            Assert.Null(orderBook.BidQuantity);
            Assert.Null(orderBook.OfferPrice);
            Assert.Null(orderBook.OfferQuantity);
            Assert.Null(orderBook.Bid);
            Assert.Null(orderBook.Offer);
            Assert.Equal(0, orderBook.RoundLotSize);
            Assert.Null(orderBook.Instrument);
        }

        [Fact]
        public void OrderBook_PropertiesSetAndGetCorrectly()
        {
            var orderBook = new OrderBook
            {
                OrderBookId = 1,
                InstrumentId = 100,
                Symbol = "AAPL",
                BidPrice = "150.00",
                BidQuantity = "100",
                OfferPrice = "150.05",
                OfferQuantity = "200",
                RoundLotSize = 100
            };

            Assert.Equal(1, orderBook.OrderBookId);
            Assert.Equal(100, orderBook.InstrumentId);
            Assert.Equal("AAPL", orderBook.Symbol);
            Assert.Equal("150.00", orderBook.BidPrice);
            Assert.Equal("100", orderBook.BidQuantity);
            Assert.Equal("150.05", orderBook.OfferPrice);
            Assert.Equal("200", orderBook.OfferQuantity);
            Assert.Equal(100, orderBook.RoundLotSize);
        }

        [Fact]
        public void OrderBook_BidOfferListsWorkCorrectly()
        {
            var orderBook = new OrderBook
            {
                Bid = new List<BidAsk>
                {
                    new BidAsk { Price = "150.00", Quantity = "100" },
                    new BidAsk { Price = "149.95", Quantity = "200" }
                },
                Offer = new List<BidAsk>
                {
                    new BidAsk { Price = "150.05", Quantity = "150" },
                    new BidAsk { Price = "150.10", Quantity = "300" }
                }
            };

            Assert.Equal(2, orderBook.Bid.Count);
            Assert.Equal(2, orderBook.Offer.Count);
            Assert.Equal("150.00", orderBook.Bid[0].Price);
            Assert.Equal("150.05", orderBook.Offer[0].Price);
        }
    }
}