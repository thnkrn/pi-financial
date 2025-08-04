/*using System.Text;
using MassTransit;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Pi.SetMarketDataWSS.Application.Services.StreamingResponseBuilder;
using Pi.SetMarketDataWSS.Application.Services.Types.ItchParser;
using Pi.SetMarketDataWSS.Domain.Entities;
using Pi.SetMarketDataWSS.Domain.Models.Response;
using Pi.SetMarketDataWSS.Infrastructure.Helpers;
using Xunit.Abstractions;
using PublicTrade = Pi.SetMarketDataWSS.Application.Services.Models.ItchOrderBookMapper.PublicTrade;

namespace Pi.SetMarketDataWSS.Application.Tests.StreamingResponseBuilder;

public class MarketStreamingResponseBuilderTests
{
    private readonly MarketStreamingResponseBuilder _builder;
    private readonly Mock<ILogger<MarketStreamingResponseBuilder>> _loggerMock;
    private readonly ITestOutputHelper _testOutputHelper;

    public MarketStreamingResponseBuilderTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _loggerMock = new Mock<ILogger<MarketStreamingResponseBuilder>>();
        _builder = new MarketStreamingResponseBuilder(_loggerMock.Object);
    }

    [Fact]
    public void WithOrderBookId_ShouldSetOrderBookId()
    {
        var orderBookId = "TEST123";
        var result = _builder.WithOrderBookId(orderBookId, "");
        Assert.Same(_builder, result);
    }

    [Fact]
    public void WithOrderBookId_ShouldThrowException_WhenOrderBookIdIsNullOrEmpty()
    {
        Assert.Throws<ArgumentException>(() => _builder.WithOrderBookId(null, null));
        Assert.Throws<ArgumentException>(() => _builder.WithOrderBookId(string.Empty, string.Empty));
    }

    [Fact]
    public async Task FetchDataAsync_ShouldDeserializeData()
    {
        var orderBookId = "TEST123";
        _builder.WithOrderBookId(orderBookId, "");

        var priceInfo = new PriceInfo { Symbol = "TEST" };
        var marketByPrice = new OrderBook();
        var publicTrades = new PublicTrade[]
        {
            new(
                new Numeric32(BitConverter.GetBytes(1000000000).Reverse().ToArray()),
                Timestamp.Parse("2023-05-01 10:00:00.123456789"),
                new Alpha(Encoding.ASCII.GetBytes("B"), 1),
                new Numeric64(BitConverter.GetBytes(100UL).Reverse().ToArray()),
                new Price32(BitConverter.GetBytes(1000000).Reverse().ToArray()) { NumberOfDecimals = 2 }
            )
        };
        var orderBookState = new MarketStatus { Status = "OPEN" };
        var instrumentDetail = new InstrumentDetail { Decimals = 2 };
        var underlyingInstrumentDetail = new InstrumentDetail { Decimals = 5 };
        var marketDirectory = new MarketDirectory { MarketCode = 11, MarketName = "EQSM", MarketDescription = "EQSM" };
        var openInterest = new OpenInterest { POI = 0 };

        var priceInfoCached = JsonConvert.SerializeObject(priceInfo);
        var marketByPriceCached = JsonConvert.SerializeObject(marketByPrice);
        var publicTradeCached = JsonConvert.SerializeObject(publicTrades);
        var orderBookStateCached = JsonConvert.SerializeObject(orderBookState);
        var instrumentDetailCached = JsonConvert.SerializeObject(instrumentDetail);
        var underlyingInstrumentDetailCached = JsonConvert.SerializeObject(underlyingInstrumentDetail);
        var marketDirectoryCached = JsonConvert.SerializeObject(marketDirectory);
        var openInterestCached = JsonConvert.SerializeObject(openInterest);

        var result = await _builder.FetchDataAsync(
            priceInfoCached,
            marketByPriceCached,
            publicTradeCached,
            orderBookStateCached,
            instrumentDetailCached,
            marketDirectoryCached,
            openInterestCached,
            underlyingInstrumentDetailCached);

        Assert.Same(_builder, result);
    }

    [Fact]
    public void Build_ShouldReturnMarketStreamingResponse()
    {
        JsonConfigHelper.ConfigureGlobalJsonSettings();

        var orderBookId = "TEST123";
        _builder.WithOrderBookId(orderBookId, "");
        var priceInfo = new PriceInfo { Symbol = "TEST", Price = "100.00", Amount = "2300000" };
        var marketByPrice = new OrderBook();
        var publicTrades = new PublicTrade[]
        {
            new(
                new Numeric32(BitConverter.GetBytes(1000000000).Reverse().ToArray()),
                Timestamp.Parse("2023-05-01 10:00:00.123456789"),
                new Alpha(Encoding.ASCII.GetBytes("B"), 1),
                new Numeric64(BitConverter.GetBytes(100UL).Reverse().ToArray()),
                new Price32(BitConverter.GetBytes(1000000).Reverse().ToArray()) { NumberOfDecimals = 2 }
            )
        };

        var orderBookState = new MarketStatus { Status = "OPEN" };
        var instrumentDetail = new InstrumentDetail { Decimals = 2 };
        var underlyingInstrumentDetail = new InstrumentDetail { Decimals = 5 };
        var marketDirectory = new MarketDirectory { MarketCode = 11, MarketName = "EQSM", MarketDescription = "EQSM" };
        var openInterest = new OpenInterest { POI = 0 };

        var priceInfoCached = JsonConvert.SerializeObject(priceInfo);
        var marketByPriceCached = JsonConvert.SerializeObject(marketByPrice);
        var publicTradeCached = JsonConvert.SerializeObject(publicTrades);
        var orderBookStateCached = JsonConvert.SerializeObject(orderBookState);
        var instrumentDetailCached = JsonConvert.SerializeObject(instrumentDetail);
        var underlyingInstrumentDetailCached = JsonConvert.SerializeObject(underlyingInstrumentDetail);
        var marketDirectoryCached = JsonConvert.SerializeObject(marketDirectory);
        var openInterestCached = JsonConvert.SerializeObject(openInterest);

        _builder.FetchDataAsync(
            priceInfoCached,
            marketByPriceCached,
            publicTradeCached,
            orderBookStateCached,
            instrumentDetailCached,
            marketDirectoryCached,
            openInterestCached,
            underlyingInstrumentDetailCached).Wait();

        var result = _builder.Build();

        Assert.NotNull(result);
        Assert.Equal("0", result.Code);
        Assert.Equal("Streaming", result.Op);
        Assert.Equal("Success", result.Message);
        Assert.NotNull(result.Response);
        Assert.Single(result.Response.Data);
        Assert.Equal("TEST", result.Response.Data[0].Symbol);
        Assert.Equal("100.00", result.Response.Data[0].Price);
        Assert.Single(result.Response.Data[0].PublicTrades);
        Assert.Equal(6, result.Response.Data[0].PublicTrades[0].Count);
        Assert.Equal(1000000000L, result.Response.Data[0].PublicTrades[0][0]);
        Assert.Equal("08:46:40", result.Response.Data[0].PublicTrades[0][1]);
        Assert.Equal("B", result.Response.Data[0].PublicTrades[0][2]);
        //Assert.Equal("B", result.Response.Data[0].PublicTrades[0][3]);
        Assert.Equal("100", result.Response.Data[0].PublicTrades[0][3]);
        Assert.Equal("10000.00", result.Response.Data[0].PublicTrades[0][4]);
        Assert.Equal("23000.00", result.Response.Data[0].Amount);
    }

    [Fact]
    public void Build_ShouldThrowException_WhenDataNotFetched()
    {
        // Arrange
        var orderBookId = "TEST123";
        _builder.WithOrderBookId(orderBookId, "");

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => _builder.Build());
        Assert.Equal("Data must be fetched before building the response", exception.Message);
    }

    [Fact]
    public void Build_ShoudReturnOnlyValidAggressor()
    {
        JsonConfigHelper.ConfigureGlobalJsonSettings();

        var orderBookId = "TEST123";
        _builder.WithOrderBookId(orderBookId, "");
        var priceInfo = new PriceInfo { Symbol = "TEST", Price = "100.00", Amount = "2300000" };
        var marketByPrice = new OrderBook();
        var publicTrades = new PublicTrade[]
        {
            new(
                new Numeric32(BitConverter.GetBytes(1000000000).Reverse().ToArray()),
                Timestamp.Parse("2024-12-01 10:00:00.123456789"),
                new Alpha(Encoding.ASCII.GetBytes("B"), 1),
                new Numeric64(BitConverter.GetBytes(100UL).Reverse().ToArray()),
                new Price32(BitConverter.GetBytes(1000000).Reverse().ToArray()) { NumberOfDecimals = 2 }
            ),
            new(
                new Numeric32(BitConverter.GetBytes(1000000000).Reverse().ToArray()),
                Timestamp.Parse("2024-12-01 10:00:01.12345"),
                new Alpha(Encoding.ASCII.GetBytes("N"), 1),
                new Numeric64(BitConverter.GetBytes(100UL).Reverse().ToArray()),
                new Price32(BitConverter.GetBytes(1000000).Reverse().ToArray()) { NumberOfDecimals = 2 }
            )
        };

        var orderBookState = new MarketStatus { Status = "OPEN" };
        var instrumentDetail = new InstrumentDetail { Decimals = 2 };
        var underlyingInstrumentDetail = new InstrumentDetail { Decimals = 5 };
        var marketDirectory = new MarketDirectory { MarketCode = 11, MarketName = "EQSM", MarketDescription = "EQSM" };
        var openInterest = new OpenInterest { POI = 0 };

        var priceInfoCached = JsonConvert.SerializeObject(priceInfo);
        var marketByPriceCached = JsonConvert.SerializeObject(marketByPrice);
        var publicTradeCached = JsonConvert.SerializeObject(publicTrades);
        var orderBookStateCached = JsonConvert.SerializeObject(orderBookState);
        var instrumentDetailCached = JsonConvert.SerializeObject(instrumentDetail);
        var underlyingInstrumentDetailCached = JsonConvert.SerializeObject(underlyingInstrumentDetail);
        var marketDirectoryCached = JsonConvert.SerializeObject(marketDirectory);
        var openInterestCached = JsonConvert.SerializeObject(openInterest);

        _builder.FetchDataAsync(
            priceInfoCached,
            marketByPriceCached,
            publicTradeCached,
            orderBookStateCached,
            instrumentDetailCached,
            marketDirectoryCached,
            openInterestCached,
            underlyingInstrumentDetailCached).Wait();

        var result = _builder.Build();

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Response);
        Assert.NotNull(result.Response.Data);
        Assert.Equal(1, result.Response.Data[0].PublicTrades.Count);
    }

    [Fact]
    public void Build_ShouldReturnMarketStreamingResponse_When_UnderlyingIsNotExists()
    {
        JsonConfigHelper.ConfigureGlobalJsonSettings();

        var orderBookId = "TEST123";
        _builder.WithOrderBookId(orderBookId, "");
        var priceInfo = new PriceInfo { Symbol = "TEST", Price = "100.00", Amount = "2300000" };
        var marketByPrice = new OrderBook();
        var publicTrades = new PublicTrade[]
        {
            new(
                new Numeric32(BitConverter.GetBytes(1000000000).Reverse().ToArray()),
                Timestamp.Parse("2023-05-01 10:00:00.123456789"),
                new Alpha(Encoding.ASCII.GetBytes("B"), 1),
                new Numeric64(BitConverter.GetBytes(100UL).Reverse().ToArray()),
                new Price32(BitConverter.GetBytes(1000000).Reverse().ToArray()) { NumberOfDecimals = 2 }
            )
        };

        var orderBookState = new MarketStatus { Status = "OPEN" };
        var instrumentDetail = new InstrumentDetail { Decimals = 2 };
        var underlyingStreamingResponse = new MarketStreamingResponse {  };
        var marketDirectory = new MarketDirectory { MarketCode = 11, MarketName = "EQSM", MarketDescription = "EQSM" };
        var openInterest = new OpenInterest { POI = 0 };

        var priceInfoCached = JsonConvert.SerializeObject(priceInfo);
        var marketByPriceCached = JsonConvert.SerializeObject(marketByPrice);
        var publicTradeCached = JsonConvert.SerializeObject(publicTrades);
        var orderBookStateCached = JsonConvert.SerializeObject(orderBookState);
        var instrumentDetailCached = JsonConvert.SerializeObject(instrumentDetail);
        var underlyingStreamingResponseCached = JsonConvert.SerializeObject(underlyingStreamingResponse);
        var marketDirectoryCached = JsonConvert.SerializeObject(marketDirectory);
        var openInterestCached = JsonConvert.SerializeObject(openInterest);

        _builder.FetchDataAsync(
            priceInfoCached,
            marketByPriceCached,
            publicTradeCached,
            orderBookStateCached,
            instrumentDetailCached,
            marketDirectoryCached,
            openInterestCached,
            underlyingStreamingResponseCached).Wait();

        var result = _builder.Build();

        Assert.NotNull(result);
        Assert.Equal("0", result.Code);
        Assert.Equal("Streaming", result.Op);
        Assert.Equal("Success", result.Message);
        Assert.NotNull(result.Response);
        Assert.Single(result.Response.Data);
        Assert.Equal("TEST", result.Response.Data[0].Symbol);
        Assert.Equal("100.00", result.Response.Data[0].Price);
        Assert.Single(result.Response.Data[0].PublicTrades);
        Assert.Equal(6, result.Response.Data[0].PublicTrades[0].Count);
        Assert.Equal(1000000000L, result.Response.Data[0].PublicTrades[0][0]);
        Assert.Equal("08:46:40", result.Response.Data[0].PublicTrades[0][1]);
        Assert.Equal("B", result.Response.Data[0].PublicTrades[0][2]);
        //Assert.Equal("B", result.Response.Data[0].PublicTrades[0][3]);
        Assert.Equal("100", result.Response.Data[0].PublicTrades[0][3]);
        Assert.Equal("10000.00", result.Response.Data[0].PublicTrades[0][4]);
        Assert.Equal("23000.00", result.Response.Data[0].Amount);
    }
}*/