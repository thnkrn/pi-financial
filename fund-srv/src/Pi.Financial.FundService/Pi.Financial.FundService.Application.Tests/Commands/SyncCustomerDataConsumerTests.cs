using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using Pi.Common.Features;
using Pi.Common.SeedWork;
using Pi.Financial.FundService.Application.Commands;
using Pi.Financial.FundService.Application.Models.Bank;
using Pi.Financial.FundService.Application.Models.Customer;
using Pi.Financial.FundService.Application.Models.Enums;
using Pi.Financial.FundService.Application.Services.CustomerService;
using Pi.Financial.FundService.Application.Services.FundConnextService;
using Pi.Financial.FundService.Application.Services.OnboardService;
using Pi.Financial.FundService.Application.Services.UserService;
using Pi.Financial.FundService.Application.Utils;
using Pi.Financial.FundService.Domain.AggregatesModel.AccountOpeningAggregate;
using Pi.Financial.FundService.Domain.AggregatesModel.CustomerDataAggregate;
using Pi.Financial.FundService.Domain.AggregatesModel.FinancialAssetAggregate;
using Pi.Financial.FundService.Domain.Events;

namespace Pi.Financial.FundService.Application.Tests.Commands;
public class SyncCustomerDataConsumerTests
{
    private readonly Mock<ILogger<SyncCustomerDataConsumer>> _loggerMock;
    private readonly Mock<IFundConnextService> _fundConnextServiceMock;
    private readonly Mock<IOnboardService> _onboardServiceMock;
    private readonly Mock<ICustomerService> _customerServiceMock;
    private readonly Mock<ICustomerDataSyncHistoryRepository> _customerDataSyncHistoryRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IFeatureService> _featureServiceMock;
    private readonly Mock<IUserService> _userServiceMock;

    public SyncCustomerDataConsumerTests()
    {
        _loggerMock = new Mock<ILogger<SyncCustomerDataConsumer>>();
        _fundConnextServiceMock = new Mock<IFundConnextService>();
        _onboardServiceMock = new Mock<IOnboardService>();
        _customerServiceMock = new Mock<ICustomerService>();
        _customerDataSyncHistoryRepositoryMock = new Mock<ICustomerDataSyncHistoryRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _featureServiceMock = new Mock<IFeatureService>();
        _userServiceMock = new Mock<IUserService>();
        _customerDataSyncHistoryRepositoryMock.Setup(o => o.UnitOfWork).Returns(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Consume_ShouldSyncCustomerDataAndUseSameHistory_WhenCorrelationIdIsTheSame()
    {
        // Arrange
        var consumer = new SyncCustomerDataConsumer(
            _loggerMock.Object,
            _fundConnextServiceMock.Object,
            _customerServiceMock.Object,
            _onboardServiceMock.Object,
            _customerDataSyncHistoryRepositoryMock.Object,
            _userServiceMock.Object,
            _featureServiceMock.Object
            );
        var contextMock = new Mock<ConsumeContext<SyncCustomerData>>();
        var customerCode = "test-customer-code";
        var customerAccountDetail =
            new CustomerAccountDetail
            {
                IcLicense = "icLicense",
                CustomerAccountUnitHolders = [],
                InvestorClass = null,
                InvestorType = InvestorType.INDIVIDUAL,
                JuristicNumber = null
            };
        var message = new SyncCustomerData(customerCode, Guid.NewGuid());
        contextMock.SetupGet(m => m.Message).Returns(message);
        var history = new CustomerDataSyncHistory(message.CorrelationId, message.CustomerCode);
        _customerDataSyncHistoryRepositoryMock.Setup(o => o.GetByCorrelationIdAsync(message.CorrelationId, It.IsAny<CancellationToken>())).ReturnsAsync(history);

        var customerInfo = new CustomerInfoForSyncCustomerFundAccount(
            IdentificationCardType.Citizen,
            "123456789",
            null,
            MailingAddressOption.CurrentAddress,
            MailingMethod.Email,
            new List<InvestmentObjective> { InvestmentObjective.Investment }.ToLookup(x => x, x => x.ToString()),
            true
        );
        var customerAccount = new CustomerAccount(
            "test-account-id",
            "test-sale-license",
            new List<BankAccount> { new BankAccount("test-bank-code", "test-bank-account-no", "test-bank-branch-code", true) },
            new List<BankAccount> { new BankAccount("test-bank-code", "test-bank-account-no", "test-bank-branch-code", true) },
            DateOnly.FromDateTime(new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc))
        );
        var ndidRequestId = "NDID-REQUEST-ID";

        _customerServiceMock.Setup(m => m.GetCustomerInfoForSyncFundCustomerAccount(customerCode)).ReturnsAsync(customerInfo);
        _fundConnextServiceMock.Setup(m => m.GetCustomerProfileAndAccount(customerInfo.CardNumber, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Client.FundConnext.Model.IndividualInvestorV5Response
            {
                ApplicationDate = "20220101",
                CrsDeclarationDate = "20220101",
                CrsPlaceOfBirthCity = "CityOfBirth",
                CrsPlaceOfBirthCountry = "CountryOfBirth",
                CrsTaxResidenceInCountriesOtherThanTheUS = true,
                CrsDetails = new List<Client.FundConnext.Model.IndividualInvestorV5ResponseCrsDetailsInner> {
                    new Client.FundConnext.Model.IndividualInvestorV5ResponseCrsDetailsInner("CountryOfTaxResidence1", "Tin1", Client.FundConnext.Model.IndividualInvestorV5ResponseCrsDetailsInner.ReasonEnum.A, "ReasonDesc1"),
                    new Client.FundConnext.Model.IndividualInvestorV5ResponseCrsDetailsInner("CountryOfTaxResidence2", "Tin2", Client.FundConnext.Model.IndividualInvestorV5ResponseCrsDetailsInner.ReasonEnum.B, "ReasonDesc2"),
                },
                NdidRequestId = ndidRequestId
            });
        _onboardServiceMock.Setup(m => m.GetMutualFundTradingAccountByCustCodeAsync(customerCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Models.TradingAccountInfo
            {
                SaleLicense = customerAccount.SaleLicense,
                AccountNo = customerAccount.AccountId,
                Id = Guid.NewGuid()
            });
        _onboardServiceMock
            .Setup(m => m.GetATSBankAccountsByCustomerCodeAsync(customerCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AtsBankAccounts(customerAccount.RedemptionBankAccount, customerAccount.SubscriptionBankAccount));
        _fundConnextServiceMock
            .Setup(m => m.GetCustomerAccountByAccountNoAsync($"{customerCode}-M", It.IsAny<CancellationToken>()))
            .ReturnsAsync(customerAccountDetail);

        // Act
        await consumer.Consume(contextMock.Object);

        // Assert
        var expectedUpdateCustomerAccount = new UpdateCustomerAccount(
            IdentificationCardType.Citizen.ToDescriptionString(),
            string.Empty,
            "123456789",
            $"{customerCode}-M",
            customerAccount.SaleLicense,
            "20220101",
            MailingAddressOption.CurrentAddress.ToDescriptionString(),
            MailingMethod.Email.ToDescriptionString(),
            "Investment",
            InvestmentObjectiveOther: string.Empty,
            customerAccount.RedemptionBankAccount,
            customerAccount.SubscriptionBankAccount,
            true
        );

        _fundConnextServiceMock.Verify(m => m.UpdateIndivialAccountAsync(It.Is<UpdateCustomerAccount>(actual => actual.Should().BeEquivalentTo(expectedUpdateCustomerAccount, "") != null), It.IsAny<CancellationToken>()), Times.Once);
        // TODO: comment out this block as we can't update customer info in fund connext now, wait for profile360
        // var expectedUpdateCrs = new Crs(
        //     "CountryOfBirth",
        //     "CityOfBirth",
        //     true,
        //     new List<CrsDetail>
        //     {
        //         new CrsDetail("CountryOfTaxResidence1", "Tin1", CrsReason.A, "ReasonDesc1"),
        //         new CrsDetail("CountryOfTaxResidence2", "Tin2", CrsReason.B, "ReasonDesc2")
        //     },
        //     "20220101"
        // );
        // _fundConnextServiceMock.Verify(m => m.UpdateIndividualCustomerV5(
        //     It.Is<CustomerInfo>(actual => actual.Should().BeEquivalentTo(customerInfo, "") != null),
        //     It.Is<Crs>(actual => actual.Should().BeEquivalentTo(expectedUpdateCrs, "") != null),
        //     ndidRequestId,
        //     It.IsAny<CancellationToken>()), Times.Once);
        contextMock.Verify(m => m.Publish(It.IsAny<CustomerDataSynced>(), It.IsAny<CancellationToken>()), Times.Once);
        _customerDataSyncHistoryRepositoryMock.Verify(m => m.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        // TODO: comment out this block as we can't update customer info in fund connext now, wait for profile360
        // Assert.True(history.IsAllSuccess);
        Assert.True(history.AccountUpdateSuccess);
    }

    [Fact]
    public async Task Consume_ShouldNotSyncCustomerData_WhenAlreadySyncedSuccessfully()
    {
        // Arrange
        var consumer = new SyncCustomerDataConsumer(_loggerMock.Object, _fundConnextServiceMock.Object, _customerServiceMock.Object, _onboardServiceMock.Object, _customerDataSyncHistoryRepositoryMock.Object, _userServiceMock.Object, _featureServiceMock.Object);
        var contextMock = new Mock<ConsumeContext<SyncCustomerData>>();
        var customerCode = "test-customer-code";
        var customerAccountDetail =
            new CustomerAccountDetail
            {
                IcLicense = "icLicense",
                CustomerAccountUnitHolders = [],
                InvestorClass = null,
                InvestorType = InvestorType.INDIVIDUAL,
                JuristicNumber = null
            };
        var message = new SyncCustomerData(customerCode, Guid.NewGuid());
        contextMock.SetupGet(m => m.Message).Returns(message);
        var history = new CustomerDataSyncHistory(message.CorrelationId, message.CustomerCode);
        history.MarkAccountUpdateAsSuccess();
        history.MarkProfileUpdateAsSuccess();
        _fundConnextServiceMock
            .Setup(m => m.GetCustomerAccountByAccountNoAsync($"{customerCode}-M", It.IsAny<CancellationToken>()))
            .ReturnsAsync(customerAccountDetail);
        _customerDataSyncHistoryRepositoryMock.Setup(o => o.GetByCorrelationIdAsync(message.CorrelationId, It.IsAny<CancellationToken>())).ReturnsAsync(history);

        // Act
        await consumer.Consume(contextMock.Object);

        // Assert
        _fundConnextServiceMock.Verify(m => m.UpdateIndivialAccountAsync(It.IsAny<UpdateCustomerAccount>(), It.IsAny<CancellationToken>()), Times.Never);
        _fundConnextServiceMock.Verify(m => m.UpdateIndividualCustomerV5(It.IsAny<CustomerInfo>(), It.IsAny<Crs>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        contextMock.Verify(m => m.Publish(It.IsAny<CustomerDataSynced>(), It.IsAny<CancellationToken>()), Times.Never);
        _customerDataSyncHistoryRepositoryMock.Verify(m => m.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        Assert.True(history.IsAllSuccess);
    }

    [Fact]
    public async Task Consume_ShouldMarkAsFailed_WhenGetCustomerInfoThrowsException()
    {
        // Arrange
        var consumer = new SyncCustomerDataConsumer(_loggerMock.Object, _fundConnextServiceMock.Object, _customerServiceMock.Object, _onboardServiceMock.Object, _customerDataSyncHistoryRepositoryMock.Object, _userServiceMock.Object, _featureServiceMock.Object);
        var contextMock = new Mock<ConsumeContext<SyncCustomerData>>();
        var customerCode = "test-customer-code";
        var message = new SyncCustomerData(customerCode, Guid.NewGuid());
        var customerAccountDetail =
            new CustomerAccountDetail
            {
                IcLicense = "icLicense",
                CustomerAccountUnitHolders = [],
                InvestorClass = null,
                InvestorType = InvestorType.INDIVIDUAL,
                JuristicNumber = null
            };
        contextMock.SetupGet(m => m.Message).Returns(message);
        _customerDataSyncHistoryRepositoryMock.Setup(o => o.GetByCorrelationIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((CustomerDataSyncHistory?)null);
        _fundConnextServiceMock
            .Setup(m => m.GetCustomerAccountByAccountNoAsync($"{customerCode}-M", It.IsAny<CancellationToken>()))
            .ReturnsAsync(customerAccountDetail);
        _customerServiceMock.Setup(m => m.GetCustomerInfoForSyncFundCustomerAccount(customerCode)).ThrowsAsync(new Exception("GetCustomerInfo failed"));

        // Act
        await consumer.Consume(contextMock.Object);

        // Assert
        _fundConnextServiceMock.Verify(m => m.UpdateIndivialAccountAsync(It.IsAny<UpdateCustomerAccount>(), It.IsAny<CancellationToken>()), Times.Never);
        _fundConnextServiceMock.Verify(m => m.UpdateIndividualCustomerV5(It.IsAny<CustomerInfo>(), It.IsAny<Crs>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        contextMock.Verify(m => m.Publish(It.IsAny<CustomerDataSynced>(), It.IsAny<CancellationToken>()), Times.Never);
        _customerDataSyncHistoryRepositoryMock.Verify(m => m.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
        _customerDataSyncHistoryRepositoryMock.Verify(m => m.AddAsync(It.Is<CustomerDataSyncHistory>(o => o.CorrelationId == message.CorrelationId && o.CustomerCode == message.CustomerCode), It.IsAny<CancellationToken>()), Times.Once);
        _customerDataSyncHistoryRepositoryMock.Verify(m => m.Update(It.Is<CustomerDataSyncHistory>(o => o.CorrelationId == message.CorrelationId && o.CustomerCode == message.CustomerCode && !o.IsAllSuccess && o.FailedReason == "GetCustomerInfo failed")), Times.Once);
    }
}
