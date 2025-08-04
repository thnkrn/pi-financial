using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using Pi.Common.Features;
using Pi.Financial.FundService.Application.Commands;
using Pi.Financial.FundService.Application.Models.Customer;
using Pi.Financial.FundService.Application.Models.Enums;
using Pi.Financial.FundService.Application.Services.CustomerService;
using Pi.Financial.FundService.Application.Services.FundConnextService;
using Pi.Financial.FundService.Application.Services.OnboardService;
using Pi.Financial.FundService.Application.Services.UserService;
using Pi.Financial.FundService.Domain.Events;

namespace Pi.Financial.FundService.Application.Tests.Commands;

public class CreateFundCustomerConsumerTests
{
    private readonly Mock<ILogger<CreateFundCustomerConsumer>> _loggerMock = new();
    private readonly Mock<ICustomerService> _customerServiceMock = new();
    private readonly Mock<IFundConnextService> _fundConnextServiceMock = new();
    private readonly Mock<IOnboardService> _onboardServiceMock = new();
    private readonly Mock<IFeatureService> _featureServiceMock = new();
    private readonly Mock<IUserService> _userServiceMock = new();
    private readonly Mock<ConsumeContext<CreateFundCustomer>> _consumeContextMock = new();
    private readonly CreateFundCustomerConsumer _consumer;
    private readonly Guid _userId = Guid.NewGuid();
    private readonly CustomerInfo _customerInfo;
    private readonly Crs _crs;

    public CreateFundCustomerConsumerTests()
    {
        _consumer = new CreateFundCustomerConsumer(
            _loggerMock.Object,
            _customerServiceMock.Object,
            _fundConnextServiceMock.Object,
            _onboardServiceMock.Object,
            _featureServiceMock.Object,
            _userServiceMock.Object
        );
        var suitabilityForm = new SuitabilityForm(
                SuitabilityAnswer.One,
                SuitabilityAnswer.Two,
                SuitabilityAnswer.Three,
                new List<SuitabilityAnswer> { SuitabilityAnswer.Four, SuitabilityAnswer.One },
                SuitabilityAnswer.Two,
                SuitabilityAnswer.Three,
                SuitabilityAnswer.Four,
                SuitabilityAnswer.One,
                SuitabilityAnswer.Two,
                SuitabilityAnswer.Three,
                SuitabilityAnswer.Four,
                SuitabilityAnswer.One,
                SuitabilityAnswer.Two,
                SuitabilityAnswer.Three
            );
        _customerInfo = new CustomerInfo(
            IdentificationCardType.Citizen,
            "123456789",
            DateOnly.FromDateTime(new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc)),
            DateOnly.FromDateTime(new DateTime(2032, 1, 1, 0, 0, 0, DateTimeKind.Utc)),
            true,
            Title.Mr,
            "FirstNameTh",
            "LastNameTh",
            "FirstNameEn",
            "LastNameEn",
            DateOnly.FromDateTime(new DateTime(1992, 1, 1, 0, 0, 0, DateTimeKind.Utc)),
            Nationality.Thai,
            "0123456789",
            MaritalStatus.Single,
            Occupation.CompanyEmployee,
            MonthlyIncomeLevel.Level1,
            1000000,
            new List<IncomeSource> { IncomeSource.Salary }.ToLookup(x => x, x => x.ToString()),
            new Address("123", "sub-district", "district", "province", "12345", Country.Thailand),
            CurrentAddressSameAsFlag.True,
            false,
            true,
            false,
            SuitabilityRiskLevel.Level1,
            DateOnly.FromDateTime(new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc)),
            true,
            DateOnly.FromDateTime(new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc)),
            DateOnly.FromDateTime(new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc)),
            Country.Thailand,
            OpenFundConnextFormFlag.Yes,
            true,
            false,
            MailingAddressOption.CurrentAddress,
            MailingMethod.Email,
            suitabilityForm,
            new List<InvestmentObjective> { InvestmentObjective.Investment }.ToLookup(x => x, x => x.ToString())
        );
        _crs = new Crs("test-crs", "bkk", false, new List<Models.Customer.CrsDetail>(), "some-date");
    }

    [Fact]
    public async Task Consumer_ShouldPublishFundCustomerCreatedEvent_WhenCreateCustomerInFundConnextSuccess()
    {
        // Arrange
        var createFundCustomer = new CreateFundCustomer(Guid.NewGuid(), "test-customer-code", true, "test-ndid-request-id");
        _consumeContextMock.Setup(m => m.Message).Returns(createFundCustomer);
        _userServiceMock.Setup(m => m.GetUserIdByCustomerCode(createFundCustomer.CustomerCode, It.IsAny<CancellationToken>())).ReturnsAsync(_userId);
        _onboardServiceMock.Setup(m => m.GetUserCrsByUserId(_userId)).ReturnsAsync(_crs);
        _customerServiceMock.Setup(m => m.GetCustomerInfo(createFundCustomer.CustomerCode)).ReturnsAsync(_customerInfo);
        _fundConnextServiceMock.Setup(m => m.CreateIndividualCustomerV5(_customerInfo, _crs, createFundCustomer.NdidRequestId, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        await _consumer.Consume(_consumeContextMock.Object);

        // Assert
        _consumeContextMock.Verify(m => m.RespondAsync(It.Is<FundCustomerCreated>(x => x.CustomerCode == createFundCustomer.CustomerCode && x.Ndid == createFundCustomer.IsNdid && x.UserId == _userId)), Times.Once);
    }

    [Fact]
    public async Task Consume_ShouldThrowException_WhenUserIdNotFound()
    {
        // Arrange
        var createFundCustomer = new CreateFundCustomer(Guid.NewGuid(), "test-customer-code", true, "test-ndid-request-id");
        _consumeContextMock.Setup(m => m.Message).Returns(createFundCustomer);
        _userServiceMock.Setup(m => m.GetUserIdByCustomerCode(createFundCustomer.CustomerCode, It.IsAny<CancellationToken>())).ReturnsAsync((Guid?)null);

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _consumer.Consume(_consumeContextMock.Object));

        // Assert
        Assert.Equal($"UserId not found for customer code: {createFundCustomer.CustomerCode}", exception.Message);
    }
}
