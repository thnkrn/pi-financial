using Moq;
using Pi.BackofficeService.Application.Queries;
using Pi.BackofficeService.Application.Queries.Filters;
using Pi.BackofficeService.Domain.AggregateModels.ResponseCodeAggregate;
using Pi.BackofficeService.Domain.AggregateModels.TicketAggregate;
using Pi.BackofficeService.Domain.AggregateModels.User;
using Pi.BackofficeService.Domain.Exceptions;
using Pi.BackofficeService.Domain.SeedWork;

namespace Pi.BackofficeService.Application.Tests.Queries;

public class TicketQueriesTest
{
    private readonly Mock<IResponseCodeRepository> _errorRepository;
    private readonly TicketQueries _queries;
    private readonly Mock<ITicketRepository> _ticketRepository;
    private readonly Mock<IUserRepository> _userRepository;

    public TicketQueriesTest()
    {
        _ticketRepository = new Mock<ITicketRepository>();
        _errorRepository = new Mock<IResponseCodeRepository>();
        _userRepository = new Mock<IUserRepository>();

        _queries = new TicketQueries(_ticketRepository.Object, _errorRepository.Object, _userRepository.Object);
    }

    [Fact]
    public async Task Should_Return_TicketResults_When_GetTickets()
    {
        // Arrange
        var ticket = new TicketState { TicketNo = "TIC0091" };
        _ticketRepository.Setup(x => x.Get(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<IQueryFilter<TicketState>>()
        )).ReturnsAsync(new List<TicketState> { ticket });
        _errorRepository.Setup(q => q.Get(It.IsAny<Guid[]>())).ReturnsAsync(new List<ResponseCode>());

        // Act
        var actual = await _queries.GetTickets(1, 10, null, null, null);

        // Assert
        Assert.Equal(ticket.TicketNo, actual.First().TicketNo);
    }

    [Fact]
    public async Task Should_Return_TicketResults_Without_Maker_When_GetTickets_And_Maker_Is_Null()
    {
        // Arrange
        var ticket = new TicketState { TicketNo = "TIC0091" };
        _ticketRepository.Setup(x => x.Get(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<IQueryFilter<TicketState>>()
        )).ReturnsAsync(new List<TicketState> { ticket });
        _errorRepository.Setup(q => q.Get(It.IsAny<Guid[]>())).ReturnsAsync(new List<ResponseCode>());

        // Act
        var actual = await _queries.GetTickets(1, 10, null, null, null);

        // Assert
        Assert.Null(actual.First().Maker);
    }

    [Fact]
    public async Task Should_Return_TicketResults_Without_Checker_When_GetTickets_And_Checker_Is_Null()
    {
        // Arrange
        var ticket = new TicketState { TicketNo = "TIC0091" };
        _ticketRepository.Setup(x => x.Get(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<IQueryFilter<TicketState>>()
        )).ReturnsAsync(new List<TicketState> { ticket });
        _errorRepository.Setup(q => q.Get(It.IsAny<Guid[]>())).ReturnsAsync(new List<ResponseCode>());

        // Act
        var actual = await _queries.GetTickets(1, 10, null, null, null);

        // Assert
        Assert.Null(actual.First().Checker);
    }

    [Fact]
    public async Task Should_Return_TicketResults_Without_Error_When_GetTickets_And_Error_Is_Null()
    {
        // Arrange
        var ticket = new TicketState { TicketNo = "TIC0091" };
        _ticketRepository.Setup(x => x.Get(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<IQueryFilter<TicketState>>()
        )).ReturnsAsync(new List<TicketState> { ticket });
        _errorRepository.Setup(q => q.Get(It.IsAny<Guid[]>())).ReturnsAsync(new List<ResponseCode>());

        // Act
        var actual = await _queries.GetTickets(1, 10, null, null, null);

        // Assert
        Assert.Null(actual.First().ResponseCode);
    }

    [Fact]
    public async Task Should_Return_TicketResults_With_Error_When_GetTickets_And_Error_Found()
    {
        // Arrange
        var ticket = new TicketState { TicketNo = "TIC0091", ResponseCodeId = Guid.NewGuid() };
        _ticketRepository.Setup(x => x.Get(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<IQueryFilter<TicketState>>()
        )).ReturnsAsync(new List<TicketState> { ticket });
        _errorRepository.Setup(q => q.Get(It.IsAny<Guid[]>())).ReturnsAsync(new List<ResponseCode>
        {
            new() { Id = (Guid)ticket.ResponseCodeId }
        });

        // Act
        var actual = await _queries.GetTickets(1, 10, null, null, null);

        // Assert
        Assert.Equal(ticket.ResponseCodeId, actual.First().ResponseCode!.Id);
    }

    [Fact]
    public async Task Should_Return_TicketResults_Without_Error_When_GetTickets_And_Error_Not_Found()
    {
        // Arrange
        var ticket = new TicketState { TicketNo = "TIC0091", ResponseCodeId = Guid.NewGuid() };
        _ticketRepository.Setup(x => x.Get(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<IQueryFilter<TicketState>>()
        )).ReturnsAsync(new List<TicketState> { ticket });
        _errorRepository.Setup(q => q.Get(It.IsAny<Guid[]>())).ReturnsAsync(new List<ResponseCode>());

        // Act
        var actual = await _queries.GetTickets(1, 10, null, null, null);

        // Assert
        Assert.Null(actual.First().ResponseCode);
    }

    [Fact]
    public async Task Should_Return_TicketResults_With_Maker_When_GetTickets_And_Maker_Not_Null()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), Guid.NewGuid(), "FirstName", "LastName", "some@mail.com");
        var ticket = new TicketState { TicketNo = "TIC0091", MakerId = user.Id };
        _ticketRepository.Setup(x => x.Get(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<IQueryFilter<TicketState>>()
        )).ReturnsAsync(new List<TicketState> { ticket });
        _errorRepository.Setup(q => q.Get(It.IsAny<Guid[]>())).ReturnsAsync(new List<ResponseCode>());
        _userRepository.Setup(q => q.GetIds(It.IsAny<Guid[]>())).ReturnsAsync(new List<User> { user });

        // Act
        var actual = await _queries.GetTickets(1, 10, null, null, null);

        // Assert
        Assert.Equal(ticket.MakerId, actual.First().Maker!.Id);
    }

    [Fact]
    public async Task Should_Return_TicketResults_With_Checker_When_GetTickets_And_Checker_Not_Null()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), Guid.NewGuid(), "FirstName", "LastName", "some@mail.com");
        var ticket = new TicketState { TicketNo = "TIC0091", CheckerId = user.Id };
        _ticketRepository.Setup(x => x.Get(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<IQueryFilter<TicketState>>()
        )).ReturnsAsync(new List<TicketState> { ticket });
        _errorRepository.Setup(q => q.Get(It.IsAny<Guid[]>())).ReturnsAsync(new List<ResponseCode>());
        _userRepository.Setup(q => q.GetIds(It.IsAny<Guid[]>())).ReturnsAsync(new List<User> { user });

        // Act
        var actual = await _queries.GetTickets(1, 10, null, null, null);

        // Assert
        Assert.Equal(ticket.CheckerId, actual.First().Checker!.Id);
    }

    [Fact]
    public async Task Should_Return_TicketAmount_As_Expect_When_CountTickets()
    {
        // Arrange
        var expected = 1;
        _ticketRepository.Setup(q => q.Count(It.IsAny<TicketFilters>())).ReturnsAsync(expected);

        // Act
        var actual = await _queries.CountTickets(null);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task Should_Return_TicketResults_When_GetTicketsByTransactionNo()
    {
        // Arrange
        var ticket = new TicketState { TicketNo = "TIC0091" };
        _ticketRepository.Setup(x => x.GetByTransactionNo(It.IsAny<string>()))
            .ReturnsAsync(new List<TicketState> { ticket });
        _errorRepository.Setup(q => q.Get(It.IsAny<Guid[]>())).ReturnsAsync(new List<ResponseCode>());

        // Act
        var actual = await _queries.GetTicketsByTransactionNo("transactionNo");

        // Assert
        Assert.Equal(ticket.TicketNo, actual.First().TicketNo);
    }

    [Fact]
    public async Task Should_Return_TicketResults_Without_Maker_When_GetTicketsByTransactionNo_And_Maker_Is_Null()
    {
        // Arrange
        var ticket = new TicketState { TicketNo = "TIC0091" };
        _ticketRepository.Setup(x => x.GetByTransactionNo(It.IsAny<string>()))
            .ReturnsAsync(new List<TicketState> { ticket });

        _errorRepository.Setup(q => q.Get(It.IsAny<Guid[]>())).ReturnsAsync(new List<ResponseCode>());

        // Act
        var actual = await _queries.GetTicketsByTransactionNo("transactionNo");

        // Assert
        Assert.Null(actual.First().Maker);
    }

    [Fact]
    public async Task Should_Return_TicketResults_Without_Checker_When_GetTicketsByTransactionNo_And_Checker_Is_Null()
    {
        // Arrange
        var ticket = new TicketState { TicketNo = "TIC0091" };
        _ticketRepository.Setup(x => x.GetByTransactionNo(It.IsAny<string>()))
            .ReturnsAsync(new List<TicketState> { ticket });

        _errorRepository.Setup(q => q.Get(It.IsAny<Guid[]>())).ReturnsAsync(new List<ResponseCode>());

        // Act
        var actual = await _queries.GetTicketsByTransactionNo("transactionNo");

        // Assert
        Assert.Null(actual.First().Checker);
    }

    [Fact]
    public async Task Should_Return_TicketResults_Without_Error_When_GetTicketsByTransactionNo_And_Error_Is_Null()
    {
        // Arrange
        var ticket = new TicketState { TicketNo = "TIC0091" };
        _ticketRepository.Setup(x => x.GetByTransactionNo(It.IsAny<string>()))
            .ReturnsAsync(new List<TicketState> { ticket });

        _errorRepository.Setup(q => q.Get(It.IsAny<Guid[]>())).ReturnsAsync(new List<ResponseCode>());

        // Act
        var actual = await _queries.GetTicketsByTransactionNo("transactionNo");

        // Assert
        Assert.Null(actual.First().ResponseCode);
    }

    [Fact]
    public async Task Should_Return_TicketResults_With_Error_When_GetTicketsByTransactionNo_And_Error_Found()
    {
        // Arrange
        var ticket = new TicketState { TicketNo = "TIC0091", ResponseCodeId = Guid.NewGuid() };
        _ticketRepository.Setup(x => x.GetByTransactionNo(It.IsAny<string>()))
            .ReturnsAsync(new List<TicketState> { ticket });

        _errorRepository.Setup(q => q.Get(It.IsAny<Guid[]>())).ReturnsAsync(new List<ResponseCode>
        {
            new() { Id = (Guid)ticket.ResponseCodeId }
        });

        // Act
        var actual = await _queries.GetTicketsByTransactionNo("transactionNo");

        // Assert
        Assert.Equal(ticket.ResponseCodeId, actual.First().ResponseCode!.Id);
    }

    [Fact]
    public async Task Should_Return_TicketResults_Without_Error_When_GetTicketsByTransactionNo_And_Error_Not_Found()
    {
        // Arrange
        var ticket = new TicketState { TicketNo = "TIC0091", ResponseCodeId = Guid.NewGuid() };
        _ticketRepository.Setup(x => x.GetByTransactionNo(It.IsAny<string>()))
            .ReturnsAsync(new List<TicketState> { ticket });

        _errorRepository.Setup(q => q.Get(It.IsAny<Guid[]>())).ReturnsAsync(new List<ResponseCode>());

        // Act
        var actual = await _queries.GetTicketsByTransactionNo("transactionNo");

        // Assert
        Assert.Null(actual.First().ResponseCode);
    }

    [Fact]
    public async Task Should_Return_TicketResults_With_Maker_When_GetTicketsByTransactionNo_And_Maker_Not_Null()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), Guid.NewGuid(), "FirstName", "LastName", "some@mail.com");
        var ticket = new TicketState { TicketNo = "TIC0091", MakerId = user.Id };
        _ticketRepository.Setup(x => x.GetByTransactionNo(It.IsAny<string>()))
            .ReturnsAsync(new List<TicketState> { ticket });

        _errorRepository.Setup(q => q.Get(It.IsAny<Guid[]>())).ReturnsAsync(new List<ResponseCode>());
        _userRepository.Setup(q => q.GetIds(It.IsAny<Guid[]>())).ReturnsAsync(new List<User> { user });

        // Act
        var actual = await _queries.GetTicketsByTransactionNo("transactionNo");

        // Assert
        Assert.Equal(ticket.MakerId, actual.First().Maker!.Id);
    }

    [Fact]
    public async Task Should_Return_TicketResults_With_Checker_When_GetTicketsByTransactionNo_And_Checker_Not_Null()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), Guid.NewGuid(), "FirstName", "LastName", "some@mail.com");
        var ticket = new TicketState { TicketNo = "TIC0091", CheckerId = user.Id };
        _ticketRepository.Setup(x => x.GetByTransactionNo(It.IsAny<string>()))
            .ReturnsAsync(new List<TicketState> { ticket });
        _errorRepository.Setup(q => q.Get(It.IsAny<Guid[]>())).ReturnsAsync(new List<ResponseCode>());
        _userRepository.Setup(q => q.GetIds(It.IsAny<Guid[]>())).ReturnsAsync(new List<User> { user });

        // Act
        var actual = await _queries.GetTicketsByTransactionNo("transactionNo");

        // Assert
        Assert.Equal(ticket.CheckerId, actual.First().Checker!.Id);
    }


    [Fact]
    public async Task Should_Return_Ticket_When_GetTicketByTicketNo()
    {
        // Arrange
        var ticket = new TicketState { TicketNo = "TIC0091" };
        _ticketRepository.Setup(x => x.GetByTicketNo(It.IsAny<string>()))
            .ReturnsAsync(ticket);
        _errorRepository.Setup(q => q.Get(It.IsAny<Guid[]>())).ReturnsAsync(new List<ResponseCode>());

        // Act
        var actual = await _queries.GetTicketByTicketNo("123");

        // Assert
        Assert.Equal(ticket.TicketNo, actual.TicketNo);
    }

    [Fact]
    public async Task Should_Throw_Exception_When_Record_Is_Null_GetTicketByTicketNo()
    {
        // Arrange
        _ticketRepository
            .Setup(repo => repo.GetByTicketNo(It.IsAny<string>()))
            .ReturnsAsync((TicketState?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _queries.GetTicketByTicketNo("123"));

        _ticketRepository.Verify(repo => repo.GetByTicketNo("123"), Times.Once);
    }
}
