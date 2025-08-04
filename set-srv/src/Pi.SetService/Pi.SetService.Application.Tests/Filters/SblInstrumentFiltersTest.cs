using Pi.SetService.Application.Filters;

namespace Pi.SetService.Application.Tests.Filters;

public class SblInstrumentFiltersTest
{
    [Fact]
    public void Should_ReturnExpectedExpressions_When_GetExpressions()
    {
        var filters = new SblInstrumentFilters()
        {
            Symbol = "123",
        };

        var expressions = filters.GetExpressions();

        Assert.Single(expressions);
    }

    [Fact]
    public void Should_ReturnExpectedExpressions_When_GetExpressions_With_Empty()
    {
        var filters = new SblInstrumentFilters();

        var expressions = filters.GetExpressions();

        Assert.Empty(expressions);
    }
}
