using Moq;
using Pi.TfexService.Application.Models;
using Pi.TfexService.Application.Tests.Mock;
using Pi.TfexService.Domain.Models.InitialMargin;
using PortfolioResponse = Pi.TfexService.Application.Services.SetTrade.PortfolioResponse;

namespace Pi.TfexService.Application.Tests.Queries.Margin;

public class GetEstRequiredInitialMarginTests : InitialMarginQueriesBaseTests
{

    [Theory]
    [InlineData(Side.Long, "TTCLZ24", 5, 1575)]
    [InlineData(Side.Short, "TTCLZ24", 3, -945)]
    [InlineData(Side.Short, "TTCLZ24", 5, -945)]
    [InlineData(Side.Long, "TTCLM25", 2, 630)]
    [InlineData(Side.Short, "TTCLZ24", 20, 3780)]
    public async void GetEstRequiredInitialMargin_When_PlacingSameUnderlying_Should_Returns_Correctly(Side side, string series, int placingUnit, int expectedEstRequiredInitialMargin)
    {
        InitialMarginRepository
            .Setup(i => i.GetInitialMargin(It.IsAny<string>(), CancellationToken.None))
            .ReturnsAsync(new InitialMargin
            {
                Symbol = "TTCL",
                Im = 1000,
                ImOutright = 315,
                ImSpread = 78,
                ProductType = "FUT"
            });
        FeatureService.Setup(f => f.IsOn(Features.DefaultTfexInitialMargin)).Returns(false);

        var portfolioResponse = new PortfolioResponse(
        [
            MockSetTrade.GeneratePortfolio(true) with
            {
                Symbol = "TTCLZ24",
                Underlying = "TTCL",
                ActualLongPosition = 4
            }
        ], MockSetTrade.GenerateTotalPortfolio());

        SetTradeService
            .Setup(s => s.GetPortfolio(It.IsAny<string>(), CancellationToken.None))
            .ReturnsAsync(portfolioResponse);
        var res = await InitialMarginQueries.GetEstRequiredInitialMargin("123456789", side, series, placingUnit, CancellationToken.None);
        Assert.Equal(expectedEstRequiredInitialMargin, res);
    }

    [Theory]
    [InlineData(Side.Short, "TTCLM25", 5, -81)]
    [InlineData(Side.Short, "TTCLM25", 6, 234)]
    [InlineData(Side.Short, "TTCLM25", 2, -474)]
    public async void GetEstRequiredInitialMargin_When_PlacingDifferentUnderlying_Should_Returns_Correctly(Side side, string series, int placingUnit, int expectedEstRequiredInitialMargin)
    {
        InitialMarginRepository
            .Setup(i => i.GetInitialMargin(It.IsAny<string>(), CancellationToken.None))
            .ReturnsAsync(new InitialMargin
            {
                Symbol = "TTCL",
                Im = 1000,
                ImOutright = 315,
                ImSpread = 78,
                ProductType = "FUT"
            });
        FeatureService.Setup(f => f.IsOn(Features.DefaultTfexInitialMargin)).Returns(false);

        var portfolioResponse = new PortfolioResponse(
        [
            MockSetTrade.GeneratePortfolio(true) with
            {
                Symbol = "TTCLZ24",
                Underlying = "TTCL",
                ActualLongPosition = 4
            },
            MockSetTrade.GeneratePortfolio() with
            {
                Symbol = "TTCLM25",
                Underlying = "TTCL",
                ActualShortPosition = 1
            }
        ], MockSetTrade.GenerateTotalPortfolio());

        SetTradeService
            .Setup(s => s.GetPortfolio(It.IsAny<string>(), CancellationToken.None))
            .ReturnsAsync(portfolioResponse);
        var res = await InitialMarginQueries.GetEstRequiredInitialMargin("123456789", side, series, placingUnit, CancellationToken.None);
        Assert.Equal(expectedEstRequiredInitialMargin, res);
    }

    [Theory]
    [InlineData(Side.Short, "TTCLZ24", 5, 1575)]
    [InlineData(Side.Long, "TTCLM25", 2, 630)]
    [InlineData(Side.Short, "TTCLM25", 20, 6300)]
    public async void GetEstRequiredInitialMargin_When_DefaultInitialMarginIsOn_Should_Returns_Correctly(Side side, string series, int placingUnit, int expectedEstRequiredInitialMargin)
    {
        InitialMarginRepository
            .Setup(i => i.GetInitialMargin(It.IsAny<string>(), CancellationToken.None))
            .ReturnsAsync(new InitialMargin
            {
                Symbol = "TTCL",
                Im = 1000,
                ImOutright = 315,
                ImSpread = 78,
                ProductType = "FUT"
            });
        FeatureService.Setup(f => f.IsOn(Features.DefaultTfexInitialMargin)).Returns(true);

        var portfolioResponse = new PortfolioResponse(
        [
            MockSetTrade.GeneratePortfolio(true) with
            {
                Symbol = "TTCLZ24",
                Underlying = "TTCL",
                ActualLongPosition = 4
            }
        ], MockSetTrade.GenerateTotalPortfolio());

        SetTradeService
            .Setup(s => s.GetPortfolio(It.IsAny<string>(), CancellationToken.None))
            .ReturnsAsync(portfolioResponse);
        var res = await InitialMarginQueries.GetEstRequiredInitialMargin("123456789", side, series, placingUnit, CancellationToken.None);
        Assert.Equal(expectedEstRequiredInitialMargin, res);
    }
}