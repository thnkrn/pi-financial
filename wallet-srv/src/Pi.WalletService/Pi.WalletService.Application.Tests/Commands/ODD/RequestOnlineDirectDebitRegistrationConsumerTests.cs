using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using Pi.Common.SeedWork;
using Pi.Common.Utilities;
using Pi.WalletService.Application.Commands.ODD;
using Pi.WalletService.Application.Models;
using Pi.WalletService.Application.Services.Bank;
using Pi.WalletService.Application.Services.UserService;
using Pi.WalletService.Domain.AggregatesModel.OnlineDirectDebitRegistrationAggregate;
using Pi.WalletService.Domain.Events.ODD;

namespace Pi.WalletService.Application.Tests.Commands.ODD;

public class RequestOnlineDirectDebitRegistrationConsumerTests
{
    private readonly Mock<IBankService> _mockBankService;
    private readonly Mock<IUserService> _mockUserService;
    private readonly Mock<ILogger<RequestOnlineDirectDebitRegistrationConsumer>> _mockLogger;
    private readonly Mock<IOnlineDirectDebitRegistrationRepository> _mockRepository;
    private readonly Mock<DateTimeProvider> _mockDateTimeProvider;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Guid _userId = Guid.NewGuid();
    private readonly string _citizenId = "testCitizenId";
    private readonly string _customerCode = "testCustomerCode";

    public RequestOnlineDirectDebitRegistrationConsumerTests()
    {
        _mockBankService = new Mock<IBankService>();
        _mockUserService = new Mock<IUserService>();
        _mockLogger = new Mock<ILogger<RequestOnlineDirectDebitRegistrationConsumer>>();
        _mockRepository = new Mock<IOnlineDirectDebitRegistrationRepository>();
        _mockDateTimeProvider = new Mock<DateTimeProvider>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockRepository.Setup(repo => repo.UnitOfWork).Returns(_mockUnitOfWork.Object);
    }

    [Fact]
    public async Task Consume_ValidRequest_CallsBankServiceWithCorrectParamsAndSavesToRepository()
    {
        // Arrange
        _mockUserService.Setup(service => service.GetUserCitizenId(_userId, It.IsAny<CancellationToken>())).ReturnsAsync(_citizenId);
        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        var oddRegisResult = new OnlineDirectDebitRegistrationResult("testWebUrl");
        _mockBankService.Setup(service => service.RegisterOnlineDirectDebit(_citizenId, "testRef", "testBank", It.IsAny<CancellationToken>())).ReturnsAsync(oddRegisResult);

        var consumer = new RequestOnlineDirectDebitRegistrationConsumer(
            _mockBankService.Object,
            _mockUserService.Object,
            _mockLogger.Object,
            _mockRepository.Object,
            _mockDateTimeProvider.Object);

        var request = new RequestOnlineDirectDebitRegistration(_userId, "testBank", "testRef");
        var context = new Mock<ConsumeContext<RequestOnlineDirectDebitRegistration>>();
        context.Setup(m => m.Message).Returns(request);

        // Act
        await consumer.Consume(context.Object);

        // Assert
        _mockBankService.Verify(service => service.RegisterOnlineDirectDebit(_citizenId, request.RefCode, request.OnlineDirectDebitBank, It.IsAny<CancellationToken>()), Times.Once);
        _mockRepository.Verify(repo => repo.AddAsync(
            It.Is<OnlineDirectDebitRegistration>(o => o.Bank == request.OnlineDirectDebitBank && o.Id == request.RefCode && o.UserId == _userId),
            It.IsAny<CancellationToken>()), Times.Once);
        _mockRepository.Verify(repo => repo.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        context.Verify(c => c.RespondAsync(It.Is<OnlineDirectDebitRegistrationRequestSuccess>(r => r.UserId == _userId && r.WebUrl == oddRegisResult.WebUrl)), Times.Once);
    }

    [Fact]
    public async Task Consume_WhenGetUserCitizenIdThrowsInvalidUserIdException_RespondsWithFailure()
    {
        // Arrange
        _mockUserService.Setup(service => service.GetUserInfoByCustomerCode(_customerCode)).ReturnsAsync(new User(_userId, new List<string>(), new List<string>(), string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty));
        var consumer = new RequestOnlineDirectDebitRegistrationConsumer(
            _mockBankService.Object,
            _mockUserService.Object,
            _mockLogger.Object,
            _mockRepository.Object,
            _mockDateTimeProvider.Object);

        var request = new RequestOnlineDirectDebitRegistration(_userId, "testBank", "testRef");
        var context = new Mock<ConsumeContext<RequestOnlineDirectDebitRegistration>>();
        context.Setup(m => m.Message).Returns(request);

        // Assume that the user service throws an InvalidUserIdException
        _mockUserService.Setup(service => service.GetUserCitizenId(request.UserId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidUserIdException());

        // Act
        await consumer.Consume(context.Object);

        // Assert
        context.Verify(c => c.RespondAsync(It.Is<OnlineDirectDebitRegistrationRequestFailed>(r => r.UserId == request.UserId && r.ErrorCode == ErrorCodes.InvalidUserId)), Times.Once);
    }

    [Fact]
    public async Task Consume_WhenBankServiceThrowsBankOperationException_RespondsWithFailure()
    {
        // Arrange
        _mockUserService.Setup(service => service.GetUserCitizenId(_userId, It.IsAny<CancellationToken>())).ReturnsAsync(_citizenId);
        var consumer = new RequestOnlineDirectDebitRegistrationConsumer(
            _mockBankService.Object,
            _mockUserService.Object,
            _mockLogger.Object,
            _mockRepository.Object,
            _mockDateTimeProvider.Object);

        var request = new RequestOnlineDirectDebitRegistration(_userId, "testBank", "testRef");
        var context = new Mock<ConsumeContext<RequestOnlineDirectDebitRegistration>>();
        context.Setup(m => m.Message).Returns(request);

        // Assume that the bank service throws a BankOperationException
        _mockBankService.Setup(service => service.RegisterOnlineDirectDebit(_citizenId, "testRef", "testBank", It.IsAny<CancellationToken>())).ThrowsAsync(new BankOperationException());

        // Act
        await consumer.Consume(context.Object);

        // Assert
        context.Verify(c => c.RespondAsync(It.Is<OnlineDirectDebitRegistrationRequestFailed>(r => r.UserId == request.UserId && r.ErrorCode == ErrorCodes.BankServiceError)), Times.Once);
    }
}
