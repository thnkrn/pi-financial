using Pi.SetService.Application.Filters;
using Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;

namespace Pi.SetService.Application.Tests.Filters;

public class SblOrderFiltersTest
{

    [Fact]
    public void Should_ReturnExpectedExpressions_When_GetExpressions()
    {
        var filters = new SblOrderFilters
        {
            TradingAccountNo = "123456-7",
            Open = true,
            Symbol = "EA",
            Statues = [SblOrderStatus.Approved],
            Type = SblOrderType.Borrow,
            CreatedDateFrom = DateOnly.FromDateTime(DateTime.UtcNow),
            CreatedDateTo = DateOnly.FromDateTime(DateTime.UtcNow)
        };

        var expressions = filters.GetExpressions();

        Assert.Equal(7, expressions.Count);
    }

    [Fact]
    public void Should_ReturnExpectedExpressions_When_GetExpressions_With_Empty()
    {
        var filters = new SblInstrumentFilters();

        var expressions = filters.GetExpressions();

        Assert.Empty(expressions);
    }
}
