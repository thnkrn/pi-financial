using MongoDB.Bson;
using Pi.SetMarketData.Domain.Entities;
using Pi.SetMarketData.Domain.Models.Response;
namespace Pi.SetMarketData.Application.Tests.ItchMapper.Mocks.ItchOrderBookMappingService;
public static class MockOrderBook
{
    public static OrderBook GetMockOrderBook()
    {
        return new OrderBook
        {
            Id = ObjectId.GenerateNewId(),
            Symbol = "AAPL",
            OrderBookId = 12345,
            InstrumentId = 12345,
            BidPrice = "135.67",
            BidQuantity = "1000",
            Bid = new List<BidAsk>
            {
                new BidAsk { Price = "135.67", Quantity = "500" },
                new BidAsk { Price = "135.50", Quantity = "300" },
                new BidAsk { Price = "135.30", Quantity = "200" }
            },
            OfferPrice = "135.80",
            OfferQuantity = "1500",
            Offer = new List<BidAsk>
            {
                new BidAsk { Price = "135.80", Quantity = "700" },
                new BidAsk { Price = "135.90", Quantity = "500" },
                new BidAsk { Price = "136.00", Quantity = "300" }
            },
        };
    }
    public static List<OrderBook> GetMockOrderBooks()
    {
        return new List<OrderBook>
        {
            new OrderBook
            {
                Id = ObjectId.GenerateNewId(),
                Symbol = "AAPL",
                OrderBookId = 12345,
                InstrumentId = 12345,
                BidPrice = "135.67",
                BidQuantity = "1000",
                Bid = new List<BidAsk>
                {
                    new BidAsk { Price = "135.67", Quantity = "500" },
                    new BidAsk { Price = "135.50", Quantity = "300" },
                    new BidAsk { Price = "135.30", Quantity = "200" }
                },
                OfferPrice = "135.80",
                OfferQuantity = "1500",
                Offer = new List<BidAsk>
                {
                    new BidAsk { Price = "135.80", Quantity = "700" },
                    new BidAsk { Price = "135.90", Quantity = "500" },
                    new BidAsk { Price = "136.00", Quantity = "300" }
                },
            },
            new OrderBook
            {
                Id = ObjectId.GenerateNewId(),
                Symbol = "GOOGL",
                OrderBookId = 67890,
                InstrumentId = 67890,
                BidPrice = "2735.67",
                BidQuantity = "2000",
                Bid = new List<BidAsk>
                {
                    new BidAsk { Price = "2735.67", Quantity = "1000" },
                    new BidAsk { Price = "2735.50", Quantity = "600" },
                    new BidAsk { Price = "2735.30", Quantity = "400" }
                },
                OfferPrice = "2736.80",
                OfferQuantity = "2500",
                Offer = new List<BidAsk>
                {
                    new BidAsk { Price = "2736.80", Quantity = "1200" },
                    new BidAsk { Price = "2736.90", Quantity = "800" },
                    new BidAsk { Price = "2737.00", Quantity = "500" }
                },
            }
        };
    }
    public static List<InstrumentDetail> GetMockInstrumentDetail()
    {
        return new List<InstrumentDetail>
        {
            new InstrumentDetail
            {
                Id = ObjectId.GenerateNewId(),
                DecimalsInPrice = 2,
                DecimalInStrikePrice = 2,
                DecimalInContractSizePQF = 2,
                StrikePrice = "135.00",
                ContractSize = "100",
                InstrumentId = 12345,
                InstrumentDetailId = 12345
            },
            new InstrumentDetail
            {
                Id = ObjectId.GenerateNewId(),
                DecimalsInPrice = 2,
                DecimalInStrikePrice = 2,
                DecimalInContractSizePQF = 2,
                StrikePrice = "120.00",
                ContractSize = "100",
                InstrumentId = 67890,
                InstrumentDetailId = 67890
            }
        };
    }

    public static List<string> GetMockVenues()
    {
        return new List<string>
        {
            "Set",
            "Equity"
        };
    }

    public static List<MarketStreamingResponse> GetMockMarketStreamingResponses()
    {
        return [
            new MarketStreamingResponse
            {
                Code = "0",
                Op = "0",
                Message = "Success",
                Response = new StreamingResponse
                {
                    Data = new List<StreamingBody>
                    {
                        new StreamingBody
                        {
                        Symbol = "AAPL",
                        Venue = "NASDAQ",
                        Price = "135.67",
                        AuctionPrice = "135.50",
                        AuctionVolume = "1000",
                        IsProjected = false,
                        LastPriceTime = 1695645600, // Unix timestamp example
                        Open = "135.00",
                        High24H = "136.00",
                        Low24H = "134.50",
                        PriceChanged = "0.17",
                        PriceChangedRate = "0.13%",
                        Volume = "1500",
                        Amount = "203505.00",
                        TotalAmount = "305700.00",
                        TotalAmountK = "305.7K",
                        TotalVolume = "1500",
                        TotalVolumeK = "1.5K",
                        Open1 = "134.00",
                        Open2 = "134.50",
                        Ceiling = "140.00",
                        Floor = "130.00",
                        Average = "135.25",
                        AverageBuy = "135.30",
                        AverageSell = "135.20",
                        Aggressor = "Buy",
                        PreClose = "135.50",
                        Status = "Active",
                        Yield = "0.01%",
                            OrderBook = new StreamingOrderBook
                            {
                                Bid = new List<List<string>>
                                {
                                    new List<string> { "135.67", "500" },
                                    new List<string> { "135.50", "300" },
                                    new List<string> { "135.30", "200" }
                                },
                                Offer = new List<List<string>>
                                {
                                    new List<string> { "135.80", "700" },
                                    new List<string> { "135.90", "500" },
                                    new List<string> { "136.00", "300" }
                                }
                            }
                        }
                    }
                }
            }
        ];
    }
}
