using AutoFixture;
using MongoDB.Bson;
using Pi.SetMarketData.Application.Services.MarketData.MarketDerivativeInformation;
using Pi.SetMarketData.Domain.Entities;

namespace Pi.SetMarketData.Application.Tests.Services.MarketDerivativeInformation;

public class MarketDerivativeInformationServiceTests
{
    private readonly Fixture _fixture;

    public MarketDerivativeInformationServiceTests()
    {
        _fixture = new Fixture();
        _fixture.Register(ObjectId.GenerateNewId);
    }

    [Fact]
    public void GetResult_ShouldReturnCorrectResponse_WhenInstrumentHasAllFields()
    {
        var instrument = _fixture.Create<Mock<Instrument>>().Object;
        instrument.SecurityType = "FC";
        instrument.TradingUnit = "10";
        instrument.MinBidUnit = "0.01";
        instrument.Multiplier = "10";
        instrument.InitialMargin = "75775";
        var instrumentDetail = _fixture.Build<InstrumentDetail>()
            .With(x => x.Instrument, (Instrument?)null)
            .With(x => x.DecimalsInPrice, 2).Create() ?? throw new ArgumentNullException(
            "_fixture.Build<InstrumentDetail>()\n            .With(x => x.Instrument, (Instrument?)null)\n            .With(x => x.DecimalsInPrice, 2).Create()");

        // Act
        var result = MarketDerivativeInformationService.GetResult(instrument, instrumentDetail, null, 0);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("0", result.Code);
        Assert.Equal("", result.Message);
        Assert.NotNull(result.Response);
        Assert.Equal(instrument.SecurityType, result.Response.SecurityType);
        Assert.Equal("0.10", result.Response.TradingUnit);
        Assert.Equal(instrument.MinBidUnit, result.Response.MinBidUnit);
        Assert.Equal(instrument.Multiplier, result.Response.Multiplier);
        Assert.Equal(instrument.InitialMargin, result.Response.InitialMargin);
    }

    [Fact]
    public void GetResult_ShouldReturnDefaultValues_WhenInstrumentFieldsAreNull()
    {
        // Arrange
        var instrument = _fixture.Create<Mock<Instrument>>().Object;
        var instrumentDetail = _fixture.Build<InstrumentDetail>()
            .With(x => x.Instrument, (Instrument?)null)
            .With(x => x.DecimalsInPrice, 5).Create();

        // Act
        var result = MarketDerivativeInformationService.GetResult(instrument, instrumentDetail, null, 0);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("0", result.Code);
        Assert.Equal("", result.Message);
        Assert.NotNull(result.Response);
        Assert.Equal("", result.Response.SecurityType);
        Assert.Equal("0.00", result.Response.TradingUnit);
        Assert.Equal("0", result.Response.MinBidUnit);
        Assert.Equal("0", result.Response.Multiplier);
        Assert.Equal("0", result.Response.InitialMargin);
    }
}