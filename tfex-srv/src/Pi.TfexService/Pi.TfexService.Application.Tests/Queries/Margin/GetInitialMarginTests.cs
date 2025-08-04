using Moq;
using Pi.TfexService.Domain.Exceptions;
using Pi.TfexService.Domain.Models.InitialMargin;

namespace Pi.TfexService.Application.Tests.Queries.Margin;

public class GetInitialMarginTests : InitialMarginQueriesBaseTests
{
    [Fact]
    public async void GetInitialMargin_Should_Failed_Empty()
    {
        var exception = await Assert.ThrowsAsync<ArgumentException>(async () => await InitialMarginQueries.GetInitialMargin("", new CancellationToken()));
        Assert.Equal("Invalid Series", exception.Message);
    }

    [Fact]
    public async void GetInitialMargin_Should_Failed_Invalid()
    {
        var exception = await Assert.ThrowsAsync<ArgumentException>(async () => await InitialMarginQueries.GetInitialMargin("A", new CancellationToken()));
        Assert.Equal("Invalid Series", exception.Message);
    }

    [Fact]
    public async void GetInitialMargin_Should_Failed_Invalid_Expire_month()
    {
        var exception = await Assert.ThrowsAsync<ArgumentException>(async () => await InitialMarginQueries.GetInitialMargin("AAVW24", new CancellationToken()));
        Assert.Equal("Invalid Expiration month", exception.Message);
    }

    [Fact]
    public async void GetInitialMargin_Should_NotFound()
    {
        InitialMarginRepository.Setup(a => a.GetInitialMargin("AAV", It.IsAny<CancellationToken>()))
            .ReturnsAsync((InitialMargin?)null);
        var exception = await Assert.ThrowsAsync<SetTradeNotFoundException>(async () => await InitialMarginQueries.GetInitialMargin("AAVH25", new CancellationToken()));
        Assert.Equal("Series not found", exception.Message);
    }

    [Fact]
    public async void GetInitialMargin_Should_Success()
    {
        InitialMarginRepository.Setup(a => a.GetInitialMargin("ADVANC", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new InitialMargin
            {
                Symbol = "ADVANC",
                Im = 1000,
                ImOutright = 1750,
                ImSpread = 500,
                ProductType = "FUT"
            });

        var res = await InitialMarginQueries.GetInitialMargin("ADVANCH25", new CancellationToken());

        Assert.NotNull(res);
        Assert.Equal("ADVANC", res.Symbol);
        Assert.Equal(1000, res.Im);
    }
}