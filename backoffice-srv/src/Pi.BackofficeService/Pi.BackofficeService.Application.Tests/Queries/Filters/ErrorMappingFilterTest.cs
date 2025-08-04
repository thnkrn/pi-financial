using Pi.BackofficeService.Application.Queries.Filters;
using Pi.BackofficeService.Domain.AggregateModels.ResponseCodeAggregate;
using Pi.BackofficeService.Domain.AggregateModels.TransactionAggregate;

namespace Pi.BackofficeService.Application.Tests.Queries.Filters;

public class ResponseCodeFilterTest
{

    [Theory]
    [InlineData(Machine.Deposit, ProductType.GlobalEquity, 3)]
    [InlineData(null, ProductType.GlobalEquity, 2)]
    [InlineData(Machine.Deposit, null, 2)]
    [InlineData(null, null, 1)]
    public void Should_Return_N_Expressions_As_Expected_When_GetExpressions(Machine? machine, ProductType? productType, int expected)
    {
        // Arrange
        var filters = new ResponseCodeFilter()
        {
            Machine = machine,
            ProductType = productType
        };

        // Act
        var expressions = filters.GetExpressions();

        // Arrange
        Assert.Equal(expected, expressions.Count);
    }
}
