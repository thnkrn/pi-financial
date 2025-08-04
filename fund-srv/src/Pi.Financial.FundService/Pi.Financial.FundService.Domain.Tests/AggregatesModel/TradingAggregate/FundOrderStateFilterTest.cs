using System.Linq.Expressions;
using Pi.Financial.FundService.Domain.AggregatesModel.TradingAggregate;

namespace Pi.Financial.FundService.Domain.Tests.AggregatesModel.TradingAggregate;

public class FundOrderStateFilterTest
{
    [Fact]
    public void Should_ReturnExpectedExpressions_When_GetExpressions()
    {
        // Arrange
        var filters = new FundOrderStateFilter()
        {
            DummyUnitHolder = true,
        };

        // Act
        var actual = filters.GetExpressions();

        // Assert
        Assert.IsType<List<Expression<Func<FundOrderState, bool>>>>(actual);
        Assert.Single(actual);
    }
}
