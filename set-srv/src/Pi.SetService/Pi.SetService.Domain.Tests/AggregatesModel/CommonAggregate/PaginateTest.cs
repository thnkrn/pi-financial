using Pi.SetService.Domain.AggregatesModel.CommonAggregate;

namespace Pi.SetService.Domain.Tests.AggregatesModel.CommonAggregate;

public class PaginateTest
{
    [Fact]
    public void Should_ReturnExpectedDefaultValue_When_InitPaginateQuery()
    {
        // Arrange

        // Act
        var actual = new PaginateQuery();

        // Assert
        Assert.Equal(1, actual.Page);
        Assert.Equal(20, actual.PageSize);
    }

    [Fact]
    public void Should_ReturnExpectedValue_When_InitPaginateQuery()
    {
        // Arrange
        var page = 10;
        var pageSize = 10;

        // Act
        var actual = new PaginateQuery()
        {
            Page = page,
            PageSize = pageSize
        };

        // Assert
        Assert.Equal(page, actual.Page);
        Assert.Equal(pageSize, actual.PageSize);
    }
}
