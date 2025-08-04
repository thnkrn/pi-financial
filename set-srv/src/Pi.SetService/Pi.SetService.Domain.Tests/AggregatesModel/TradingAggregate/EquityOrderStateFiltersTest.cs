using Pi.SetService.Domain.AggregatesModel.TradingAggregate;

namespace Pi.SetService.Domain.Tests.AggregatesModel.TradingAggregate;

public class EquityOrderStateFiltersTest
{
    [Fact]
    public void Should_ReturnExpectedExpressions_When_GetExpressions()
    {
        var filters = new EquityOrderStateFilters
        {
            BrokerOrderId = "123",
            CreatedDate = DateOnly.FromDateTime(DateTime.UtcNow)
        };

        var expressions = filters.GetExpressions();

        Assert.Equal(2, expressions.Count);
    }

    [Fact]
    public void Should_ReturnExpectedExpressions_When_GetExpressions_With_Empty()
    {
        var filters = new EquityOrderStateFilters();

        var expressions = filters.GetExpressions();

        Assert.Empty(expressions);
    }
}
