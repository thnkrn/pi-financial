using Moq;
using MySqlX.XDevAPI.Common;
using Pi.SetMarketData.Application.Constants;
using Pi.SetMarketData.Application.Models.ItchMessageWrapper;
using Pi.SetMarketData.Domain.Entities;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;
using Pi.SetMarketData.Infrastructure.Services.Helpers;

namespace Pi.SetMarketData.Infrastructure.Test.Helpers;
public class InstrumentHelperTest
{
    private readonly InstrumentHelper _instrumentHelper;
    private readonly Mock<IMongoService<SetMonthMapping>> _mockSetMonthMappingService;
    public InstrumentHelperTest()
    {
        _mockSetMonthMappingService = new Mock<IMongoService<SetMonthMapping>>();

        _instrumentHelper = new InstrumentHelper(_mockSetMonthMappingService.Object);
    }

    [InlineData("S50H22P900", "SET50 Index Mar 22 Put 900")]
    [Theory]
    public async void GetFriendlyName_ReturnFriendlyName_When_GetSET50IndexOptions(string symbol, string friendlyName)
    {
        var result = await _instrumentHelper.GetFriendlyName(symbol, InstrumentConstants.SET50IndexOptions);

        Assert.NotEmpty(result);
        Assert.Equal(result, friendlyName);
    }
    [InlineData("S50H22", "SET50 Index Mar 22 Futures")]
    [Theory]
    public async void GetFriendlyName_ReturnFriendlyName_When_GetSet50IndexFutures(string symbol, string friendlyName)
    {
        var result = await _instrumentHelper.GetFriendlyName(symbol, InstrumentConstants.SET50IndexFutures);

        Assert.NotEmpty(result);
        Assert.Equal(result, friendlyName);
    }
    [InlineData("FOODZ22", "SET FOOD Sector Dec 22 Futures")]
    [Theory]
    public async void GetFriendlyName_ReturnFriendlyName_When_GetSectorIndexFutures(string symbol, string friendlyName)
    {
        var result = await _instrumentHelper.GetFriendlyName("FOODZ22", InstrumentConstants.SectorIndexFutures);

        Assert.NotEmpty(result);
        Assert.Equal(result, friendlyName);
    }
    [InlineData("AOTH22", "AOT Mar 22 Futures")]
    [InlineData("AAVH25", "AAV Mar 25 Futures")]
    [InlineData("BBLU25", "BBL Sep 25 Futures")]
    [Theory]
    public async void GetFriendlyName_ReturnFriendlyName_When_GetSingleStockFutures(string symbol, string friendlyName)
    {
        var result = await _instrumentHelper.GetFriendlyName(symbol, InstrumentConstants.SingleStockFutures);

        Assert.NotEmpty(result);
        Assert.Equal(result, friendlyName);
    }

    [InlineData("USDH22", "USD Currency Mar 22 Futures")]
    [Theory]
    public async void GetFriendlyName_ReturnFriendlyName_When_GetCurrencyFutures(string symbol, string friendlyName)
    {
        var result = await _instrumentHelper.GetFriendlyName(symbol, InstrumentConstants.CurrencyFutures);

        Assert.NotEmpty(result);
        Assert.Equal(result, friendlyName);
    }

    [InlineData("GF10H24", "Gold 10 Baht Mar 24 Futures")]
    [InlineData("GFH22", "Gold 50 Baht Mar 22 Futures")]
    [InlineData("GOZ24", "Gold Online Dec 24 Futures")]
    [InlineData("SVFZ24", "Silver Online Dec 24 Futures")]
    [Theory]
    public async void GetFriendlyName_ReturnFriendlyName_When_GetPreciousMetalFutures(string symbol, string friendlyName)
    {
        var result = await _instrumentHelper.GetFriendlyName(symbol, InstrumentConstants.PreciousMetalFutures);

        Assert.NotEmpty(result);
        Assert.Equal(result, friendlyName);
    }
}