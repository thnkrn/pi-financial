using Pi.BackofficeService.Application.Queries.Filters;
using Pi.BackofficeService.Domain.AggregateModels.TicketAggregate;

namespace Pi.BackofficeService.Application.Tests.Queries.Filters;

public class TicketFiltersTest
{
    [Theory]
    [InlineData(true, "custCode", Status.Approved, 4)]
    [InlineData(false, "custCode", Status.Approved, 3)]
    [InlineData(false, null, Status.Approved, 2)]
    [InlineData(false, null, null, 1)]
    public void Should_Return_N_Expressions_As_Expected_When_GetExpressions(bool error, string? customerCode, Status? status, int expected)
    {
        // Arrange
        var filters = new TicketFilters(error ? Guid.NewGuid() : null, customerCode, status);

        // Act
        var expressions = filters.GetExpressions();

        // Arrange
        Assert.Equal(expected, expressions.Count);
    }

    [Fact]
    public void Should_Return_Filtered_Ticket_When_Filter_By_ErrorId()
    {
        // Arrange
        var filters = new TicketFilters(Guid.NewGuid(), null, null);
        var expressions = filters.GetExpressions();
        var records = new List<TicketState>()
        {
            new() { TicketNo = "TIC111", ResponseCodeId = filters.ResponseCode },
            new() { TicketNo = "TIC112", ResponseCodeId = filters.ResponseCode },
            new() { TicketNo = "TIC113", ResponseCodeId = Guid.NewGuid() },
        };

        // Act
        var actual = expressions.Aggregate(records, (current, expression) =>
        {
            return current.Where(expression.Compile()).ToList();
        });

        // Arrange
        Assert.Equal(2, actual.Count);
    }

    [Fact]
    public void Should_Return_Filtered_Ticket_When_Filter_By_CustomerCode()
    {
        // Arrange
        var filters = new TicketFilters(null, "custCode1", null);
        var expressions = filters.GetExpressions();
        var records = new List<TicketState>()
        {
            new() { TicketNo = "TIC111", CustomerCode = filters.CustomerCode },
            new() { TicketNo = "TIC112", CustomerCode = "CustCode2" },
            new() { TicketNo = "TIC113", CustomerCode = "CustCode2" },
        };

        // Act
        var actual = expressions.Aggregate(records, (current, expression) =>
        {
            return current.Where(expression.Compile()).ToList();
        });

        // Arrange
        Assert.Single(actual);
    }

    [Fact]
    public void Should_Return_Filtered_Ticket_When_Filter_Status()
    {
        // Arrange
        var filters = new TicketFilters(null, null, Status.Approved);
        var expressions = filters.GetExpressions();
        var records = new List<TicketState>()
        {
            new() { TicketNo = "TIC111", Status = filters.Status },
            new() { TicketNo = "TIC112", Status = Status.Pending },
            new() { TicketNo = "TIC113", Status = Status.Rejected },
        };

        // Act
        var actual = expressions.Aggregate(records, (current, expression) =>
        {
            return current.Where(expression.Compile()).ToList();
        });

        // Arrange
        Assert.Single(actual);
    }

    [Fact]
    public void Should_Return_Ticket_With_TicketNo_When_Filter()
    {
        // Arrange
        var filters = new TicketFilters(null, null, null);
        var expressions = filters.GetExpressions();
        var records = new List<TicketState>()
        {
            new() { TicketNo = "TIC111", Status = filters.Status },
            new() { TicketNo = "TIC112", Status = Status.Pending },
            new() { TicketNo = null, Status = Status.Rejected },
        };

        // Act
        var actual = expressions.Aggregate(records, (current, expression) =>
        {
            return current.Where(expression.Compile()).ToList();
        });

        // Arrange
        Assert.Equal(2, actual.Count);
    }
}
