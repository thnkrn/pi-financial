using Pi.BackofficeService.Application.Factories;
using Pi.BackofficeService.Application.Models;
using Pi.BackofficeService.Application.Queries;
using Pi.BackofficeService.Domain.AggregateModels.ResponseCodeAggregate;
using Pi.BackofficeService.Domain.AggregateModels.TicketAggregate;
using Pi.BackofficeService.Domain.AggregateModels.User;

namespace Pi.BackofficeService.Application.Tests.Factories;

public class QueriesFactoryTest
{
    [Fact]
    public void Should_Return_ResponseCodeDetail_When_NewResponseCodeAction_With_ResponseCode_And_ResponseCodeAction()
    {
        // Arrange
        var responseCode = new ResponseCode() { Id = Guid.NewGuid() };
        var responseCodeActions = new List<ResponseCodeAction>();

        // Act
        var actual = QueriesFactory.NewResponseCodeDetail(responseCode, responseCodeActions);

        // Assert
        Assert.IsType<ResponseCodeDetail>(actual);
    }

    [Fact]
    public void Should_Return_ResponseCodeDetail_And_ResponseCodeDetailId_Equal_ResponseCodeId_When_NewResponseCodeAction_With_ResponseCode_And_ResponseCodeAction()
    {
        // Arrange
        var responseCode = new ResponseCode() { Id = Guid.NewGuid() };
        var responseCodeActions = new List<ResponseCodeAction>();

        // Act
        var actual = QueriesFactory.NewResponseCodeDetail(responseCode, responseCodeActions);

        // Assert
        Assert.Equal(responseCode.Id, actual.Id);
    }

    [Fact]
    public void Should_Return_ResponseCodeDetail_And_ResponseCodeDetailAction_Equal_ResponseCodeAction_When_NewResponseCodeAction_With_ResponseCode_And_ResponseCodeAction()
    {
        // Arrange
        var responseCode = new ResponseCode() { Id = Guid.NewGuid() };
        var responseCodeActions = new List<ResponseCodeAction>() { new(Guid.NewGuid(), Guid.NewGuid(), Method.Approve) };

        // Act
        var actual = QueriesFactory.NewResponseCodeDetail(responseCode, responseCodeActions);

        // Assert
        Assert.Same(responseCodeActions, actual.Actions);
    }

    [Fact]
    public void Should_Return_TicketResult_When_NewTicketResponse()
    {
        // Arrange
        var ticket = new TicketState() { TicketNo = "TIC1123" };

        // Act
        var actual = QueriesFactory.NewTicketResponse(ticket, null, null, null);

        // Assert
        Assert.IsType<TicketResult>(actual);
    }

    [Fact]
    public void Should_Return_TicketResult_And_TicketResultTicketNo_Equal_TicketTicketNo_When_NewTicketResponse()
    {
        // Arrange
        var ticket = new TicketState() { TicketNo = "TIC1123" };

        // Act
        var actual = QueriesFactory.NewTicketResponse(ticket, null, null, null);

        // Assert
        Assert.Equal(ticket.TicketNo, actual.TicketNo);
    }

    [Fact]
    public void Should_Return_TicketResult_With_Maker_When_NewTicketResponse_And_MakerNotNull()
    {
        // Arrange
        var ticket = new TicketState() { TicketNo = "TIC1123" };
        var user = new User(Guid.NewGuid(), Guid.NewGuid(), "FirstName", "LastName", "some@mail.com");

        // Act
        var actual = QueriesFactory.NewTicketResponse(ticket, user, null, null);

        // Assert
        Assert.Same(user, actual.Maker);
    }

    [Fact]
    public void Should_Return_TicketResult_With_Checker_When_NewTicketResponse_And_CheckerNotNull()
    {
        // Arrange
        var ticket = new TicketState() { TicketNo = "TIC1123" };
        var user = new User(Guid.NewGuid(), Guid.NewGuid(), "FirstName", "LastName", "some@mail.com");

        // Act
        var actual = QueriesFactory.NewTicketResponse(ticket, null, user, null);

        // Assert
        Assert.Same(user, actual.Checker);
    }

    [Fact]
    public void Should_Return_TicketResult_With_ResponseCode_When_NewTicketResponse_And_ResponseCodeNotNull()
    {
        // Arrange
        var ticket = new TicketState() { TicketNo = "TIC1123" };
        var responseCode = new ResponseCode() { Id = Guid.NewGuid() };

        // Act
        var actual = QueriesFactory.NewTicketResponse(ticket, null, null, responseCode);

        // Assert
        Assert.Same(responseCode, actual.ResponseCode);
    }
}
