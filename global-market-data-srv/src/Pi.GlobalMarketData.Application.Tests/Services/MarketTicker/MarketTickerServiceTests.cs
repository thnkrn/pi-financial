using AutoFixture;
using Pi.GlobalMarketData.Application.Services.MarketData.MarketTicker;
using Pi.GlobalMarketData.Domain.Entities;
using Pi.GlobalMarketData.Domain.Models.Response;

public class MarketTickerServiceTests
{
    private readonly MarketTickerService _marketTickerService;
    public MarketTickerServiceTests()
    {
        _marketTickerService = new MarketTickerService();
    }

    // [Fact]
    // public void GetResult_ReturnValueWithFriendlyName()
    // {
    //     // Arrange
    //     var fixture = new Fixture();
    //     var high52 = fixture.CreateMany<double>(1).ToList();
    //     var low52 = fixture.CreateMany<double>(1).ToList();
    //     var instrument_category = fixture.CreateMany<string>(1).ToList();
    //     List<string> symbols = fixture.CreateMany<string>(1).ToList();
    //     List<string> venues = fixture.CreateMany<string>(1).ToList();
    //     var geInstrument = fixture.Build<GeInstrument>()
    //         .With(x => x.Id, new MongoDB.Bson.ObjectId()).Create();
    //     var exchangeTimezone = fixture.Build<ExchangeTimezone>()
    //         .With(x => x.Id, new MongoDB.Bson.ObjectId())
    //         .With(x => x.Timezone, string.Empty).Create();
    //     var morningStarStock = fixture.Build<MorningStarStocks>()
    //         .With(x => x.Id, new MongoDB.Bson.ObjectId()).Create();
    //     List<GeInstrument> geInstrumentLIst = new List<GeInstrument> { geInstrument };
    //     List<ExchangeTimezone> exchangeTimezoneList = new List<ExchangeTimezone> { exchangeTimezone };
    //     List<MorningStarStocks> morningStarStockList = new List<MorningStarStocks> {morningStarStock };
    //     List<StreamingBody> streamingBodyList = fixture.CreateMany<StreamingBody>().ToList();
    //     List<MarketSchedule> marketSchedule = new List<MarketSchedule> { new MarketSchedule {MarketSession = "Mainsession"}};
    //     _marketTickerService.SetInstrumentCategory(instrument_category);
    //     _marketTickerService.SetTickerPriceParams(high52, low52);
    //     _marketTickerService.SetTickerParams(symbols, venues, geInstrumentLIst, exchangeTimezoneList, morningStarStockList, streamingBodyList);
    //     _marketTickerService.SetMarketSchedule(marketSchedule);
    //     // Act
    //     var result = _marketTickerService.GetResult();
    //     // Assert
    //     
    //     Assert.Equal(geInstrumentLIst[0].Name, result.Response.Data[0].FriendlyName);
    //     
    // }
    // [Fact]
    // public void GetResult_ReturnStreamingBody_WithValidVolume()
    // {
    //     // Arrange
    //     var fixture = new Fixture();
    //     var high52 = fixture.CreateMany<double>(1).ToList();
    //     var low52 = fixture.CreateMany<double>(1).ToList();
    //     var instrument_category = fixture.CreateMany<string>(1).ToList();
    //     List<string> symbols = fixture.CreateMany<string>(1).ToList();
    //     List<string> venues = fixture.CreateMany<string>(1).ToList();
    //     List<string> logos = fixture.CreateMany<string>(1).ToList();
    //     List<MarketSchedule> marketSchedule = new List<MarketSchedule> { new MarketSchedule {MarketSession = "Mainsession"}};
    //     var geInstrument = fixture.Build<GeInstrument>()
    //         .With(x => x.Id, new MongoDB.Bson.ObjectId()).Create();
    //     var exchangeTimezone = fixture.Build<ExchangeTimezone>()
    //         .With(x => x.Id, new MongoDB.Bson.ObjectId())
    //         .With(x => x.Timezone, string.Empty).Create();
    //     var morningStarStock = fixture.Build<MorningStarStocks>()
    //         .With(x => x.Id, new MongoDB.Bson.ObjectId()).Create();
    //     List<GeInstrument> geInstrumentLIst = new List<GeInstrument> { geInstrument };
    //     List<ExchangeTimezone> exchangeTimezoneList = new List<ExchangeTimezone> { exchangeTimezone };
    //     List<MorningStarStocks> morningStarStockList = new List<MorningStarStocks> {morningStarStock };
    //     List<StreamingBody> streamingBodyList = fixture.CreateMany<StreamingBody>(1).ToList();
    //     _marketTickerService.SetInstrumentCategory(instrument_category);
    //     _marketTickerService.SetTickerPriceParams(high52, low52);
    //     _marketTickerService.SetLogoParams(logos);
    //     _marketTickerService.SetMarketSchedule(marketSchedule);
    //     _marketTickerService.SetTickerParams(symbols, venues, geInstrumentLIst, exchangeTimezoneList, morningStarStockList, streamingBodyList);
    //     // Act
    //     var result = _marketTickerService.GetResult();
    //     // Assert
    //     Assert.NotNull(result);
    //     Assert.Equal(result.Response.Data[0].Volume, streamingBodyList[0].Volume);
    //     Assert.Equal(result.Response.Data[0].TotalVolume, streamingBodyList[0].TotalVolume);
    //     Assert.Equal(result.Response.Data[0].TotalVolumeK, streamingBodyList[0].TotalVolumeK);
    // }
}