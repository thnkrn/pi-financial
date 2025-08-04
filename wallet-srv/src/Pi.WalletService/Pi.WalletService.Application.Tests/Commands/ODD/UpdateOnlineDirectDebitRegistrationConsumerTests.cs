using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using Pi.Common.SeedWork;
using Pi.WalletService.Application.Commands.ODD;
using Pi.WalletService.Domain.AggregatesModel.OnlineDirectDebitRegistrationAggregate;
using Pi.WalletService.IntegrationEvents;

namespace Pi.WalletService.Application.Tests.Commands.ODD
{
    public class UpdateOnlineDirectDebitRegistrationConsumerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IOnlineDirectDebitRegistrationRepository> _mockRepository;
        private readonly Mock<ILogger<UpdateOnlineDirectDebitRegistrationConsumer>> _mockLogger;

        public UpdateOnlineDirectDebitRegistrationConsumerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockRepository = new Mock<IOnlineDirectDebitRegistrationRepository>();
            _mockLogger = new Mock<ILogger<UpdateOnlineDirectDebitRegistrationConsumer>>();
        }

        [Fact]
        public async Task Consume_WhenResultIsSuccess_UpdateAndPublishSuccessEvent()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.UnitOfWork).Returns(_mockUnitOfWork.Object);
            var consumer = new UpdateOnlineDirectDebitRegistrationConsumer(_mockRepository.Object, _mockLogger.Object);

            var refCode = "testRefCode";
            var bankAccountNo = "testBankAccountNo";
            var isSuccess = true;
            var externalStatusCode = "testStatusCode";
            var externalStatusDescription = "testStatusDescription";
            var updateOnlineDirectDebitRegistration = new UpdateOnlineDirectDebitRegistration(refCode, bankAccountNo, isSuccess, externalStatusCode, externalStatusDescription);
            var context = new Mock<ConsumeContext<UpdateOnlineDirectDebitRegistration>>();
            context.Setup(m => m.Message).Returns(updateOnlineDirectDebitRegistration);
            context.Setup(m => m.CancellationToken).Returns(new CancellationToken());
            _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var registration = new OnlineDirectDebitRegistration(refCode, Guid.NewGuid(), "bank", new DateTime(2022, 12, 30, 0, 0, 0, DateTimeKind.Utc));
            registration.Success();
            _mockRepository.Setup(r => r.GetAsync(refCode, It.IsAny<CancellationToken>())).ReturnsAsync(registration);

            // Act
            await consumer.Consume(context.Object);

            // Assert
            _mockRepository.Verify(r => r.Update(registration), Times.Once);
            _mockRepository.Verify(r => r.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            context.Verify(c => c.Publish(It.Is<OnlineDirectDebitRegistrationSuccessEvent>(e => e.RefCode == refCode && e.BankAccountNo == bankAccountNo), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Consume_WhenResultIsFailure_UpdateAndPublishFailureEvent()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.UnitOfWork).Returns(_mockUnitOfWork.Object);
            var consumer = new UpdateOnlineDirectDebitRegistrationConsumer(_mockRepository.Object, _mockLogger.Object);

            var refCode = "testRefCode";
            var bankAccountNo = "testBankAccountNo";
            var isSuccess = false; // Set isSuccess to false
            var externalStatusCode = "testStatusCode";
            var externalStatusDescription = "testStatusDescription";
            var updateOnlineDirectDebitRegistration = new UpdateOnlineDirectDebitRegistration(refCode, bankAccountNo, isSuccess, externalStatusCode, externalStatusDescription);
            var context = new Mock<ConsumeContext<UpdateOnlineDirectDebitRegistration>>();
            context.Setup(m => m.Message).Returns(updateOnlineDirectDebitRegistration);
            context.Setup(m => m.CancellationToken).Returns(new CancellationToken());
            _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var registration = new OnlineDirectDebitRegistration(refCode, Guid.NewGuid(), "bank", new DateTime(2022, 12, 30, 0, 0, 0, DateTimeKind.Utc));
            registration.Failed(externalStatusCode); // Call Failed method instead of Success
            _mockRepository.Setup(r => r.GetAsync(refCode, It.IsAny<CancellationToken>())).ReturnsAsync(registration);

            // Act
            await consumer.Consume(context.Object);

            // Assert
            _mockRepository.Verify(r => r.Update(registration), Times.Once);
            _mockRepository.Verify(r => r.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            context.Verify(c => c.Publish(It.Is<OnlineDirectDebitRegistrationFailedEvent>(e => e.RefCode == refCode && e.BankAccountNo == bankAccountNo), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Consume_WhenRegistrationIsNull_ThrowsException()
        {
            // Arrange
            var consumer = new UpdateOnlineDirectDebitRegistrationConsumer(_mockRepository.Object, _mockLogger.Object);

            var refCode = "testRefCode";
            var bankAccountNo = "testBankAccountNo";
            var isSuccess = false; // Set isSuccess to false
            var externalStatusCode = "testStatusCode";
            var externalStatusDescription = "testStatusDescription";
            var updateOnlineDirectDebitRegistration = new UpdateOnlineDirectDebitRegistration(refCode, bankAccountNo, isSuccess, externalStatusCode, externalStatusDescription);
            var context = new Mock<ConsumeContext<UpdateOnlineDirectDebitRegistration>>();
            context.Setup(m => m.Message).Returns(updateOnlineDirectDebitRegistration);
            context.Setup(m => m.CancellationToken).Returns(new CancellationToken());

            _mockRepository.Setup(r => r.GetAsync(refCode, It.IsAny<CancellationToken>())).ReturnsAsync((OnlineDirectDebitRegistration?)null); // Return null

            // Act & Assert
            await Assert.ThrowsAsync<InvalidDataException>(() => consumer.Consume(context.Object));
        }
    }
}