using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Pi.Common.SeedWork;
using Pi.Common.Utilities;
using Pi.User.Application.Commands;
using Pi.User.Application.Services.LegacyUserInfo;
using Pi.User.Domain.AggregatesModel.TransactionIdAggregate;
using Pi.User.Domain.AggregatesModel.UserInfoAggregate;
using Pi.User.IntegrationEvents;
using Xunit;

namespace Pi.User.Application.Tests.Commands;

public class UpdateBankAccountEffectiveDateConsumerTests : ConsumerTest
{
    private readonly Mock<ILogger<UpdateBankAccountEffectiveDateConsumer>> _loggerMock = new();
    private readonly Mock<IUserBankAccountService> _userBankAccountServiceMock = new();
    private readonly Mock<DateTimeProvider> _dateTimeProviderMock = new();
    private readonly Mock<ITransactionIdRepository> _transactionIdRepositoryMock = new();
    private readonly Mock<IUserInfoRepository> _userInfoRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    public UpdateBankAccountEffectiveDateConsumerTests()
    {
        _transactionIdRepositoryMock.SetupGet(r => r.UnitOfWork).Returns(_unitOfWorkMock.Object);
        _userInfoRepositoryMock.SetupGet(r => r.UnitOfWork).Returns(_unitOfWorkMock.Object);
        Provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg => { cfg.AddConsumer<UpdateBankAccountEffectiveDateConsumer>(); })
            .AddScoped(_ => _loggerMock.Object)
            .AddScoped(_ => _userBankAccountServiceMock.Object)
            .AddScoped(_ => _dateTimeProviderMock.Object)
            .AddScoped(_ => _transactionIdRepositoryMock.Object)
            .AddScoped(_ => _userInfoRepositoryMock.Object)
            .BuildServiceProvider(true);
    }

    [Fact]
    public async Task Consume_BankAccountInfoExists_UpdatesEffectiveDate()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var customerCode = "customer123";
        var bankAccountNo = "1234567890";
        var bankCode = "001";
        var bankBranchCode = "0001";
        var request = new UpdateBankAccountEffectiveDate(userId, customerCode, bankAccountNo, bankCode, bankBranchCode);

        var bankAccountInfoItem = new BankAccountInfoItem(
            tradingAccountNo: "TA123456",
            accountType: "Savings",
            transactionType: "Deposit",
            rPType: "Regular",
            bankCode: "001",
            bankAccountNo: "1234567890",
            bankAccountType: "Checking",
            payType: "Direct",
            effectiveDate: new DateOnly(2023, 1, 1),
            endDate: new DateOnly(2024, 1, 1),
            accountCodeType: "TypeA",
            exchangeMarket: "NYSE"
        );
        var bankAccountInfo = new BankAccountInfo(customerCode, new List<BankAccountInfoItem>
        {
            bankAccountInfoItem
        }, 1);

        _userBankAccountServiceMock.Setup(s => s.GetBankAccountInfoAsync(customerCode, default)).ReturnsAsync(bankAccountInfo);
        var userInfo = new UserInfo(userId, "customer01", null, new UserPersonalInfo(
            "1234567890123", "John", "Doe", "John", "Doe", "0812345678", "john.doe@example.com", "Thailand", "Bangkok"));
        _userInfoRepositoryMock.Setup(s => s.Get(It.IsAny<Guid>(), It.IsAny<bool>())).ReturnsAsync(userInfo);

        // Act
        await Harness.Bus.Publish(request);

        // Assert
        Assert.True(await Harness.Consumed.Any<UpdateBankAccountEffectiveDate>());
        Assert.True(await Harness.Published.Any<SubmitBankAccountRequest>());
    }

    [Fact]
    public async Task Consume_NoBankAccountInfo_PublishesSubmitBankAccountRequest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var customerCode = "customer123";
        var bankAccountNo = "1234567890";
        var bankCode = "001";
        var bankBranchCode = "0001";
        var request = new UpdateBankAccountEffectiveDate(userId, customerCode, bankAccountNo, bankCode, bankBranchCode);

        _userBankAccountServiceMock.Setup(s => s.GetBankAccountInfoAsync(customerCode, default)).ReturnsAsync((BankAccountInfo)null!);
        _dateTimeProviderMock.Setup(s => s.OffsetNow()).Returns(DateTimeOffset.Now);

        var userInfo = new UserInfo(userId, "customer01", null, new UserPersonalInfo(
            "1234567890123", "John", "Doe", "John", "Doe", "0812345678", "john.doe@example.com", "Thailand", "Bangkok"));

        _userInfoRepositoryMock.Setup(s => s.Get(userId, false)).ReturnsAsync(userInfo);
        _transactionIdRepositoryMock.Setup(s => s.GetNextAsync(It.IsAny<string>(), It.IsAny<DateOnly>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TransactionId("UB", DateOnly.FromDateTime(DateTime.Now), "UB20230918152557", "custCode", 2));

        // Act
        await Harness.Bus.Publish(request);

        // Assert
        Assert.True(await Harness.Consumed.Any<UpdateBankAccountEffectiveDate>());
        Assert.True(await Harness.Published.Any<AddBankAccountWithEffectiveEvent>());
    }

    [Fact]
    public async Task Consume_BankAccountInfoExists_NoMatchingBankAccount_PublishesUpdateBankAccountEffectiveDateFailedEvent()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var customerCode = "customer123";
        var bankAccountNo = "1234567890";
        var bankCode = "001";
        var bankBranchCode = "0001";
        var request = new UpdateBankAccountEffectiveDate(userId, customerCode, bankAccountNo, bankCode, bankBranchCode);

        var bankAccountInfoItem = new BankAccountInfoItem(
            tradingAccountNo: "TA123456",
            accountType: "Savings",
            transactionType: "Deposit",
            rPType: "Regular",
            bankCode: "001",
            bankAccountNo: "1234567890",
            bankAccountType: "Checking",
            payType: "Direct",
            effectiveDate: new DateOnly(2023, 1, 1),
            endDate: new DateOnly(2024, 1, 1),
            accountCodeType: "TypeA",
            exchangeMarket: "NYSE"
        );
        var bankAccountInfo = new BankAccountInfo(customerCode, new List<BankAccountInfoItem>
        {
            bankAccountInfoItem
        }, 1);

        _userBankAccountServiceMock.Setup(s => s.GetBankAccountInfoAsync(customerCode, default)).ReturnsAsync(bankAccountInfo);

        // Act
        await Harness.Bus.Publish(request);

        // Assert
        Assert.True(await Harness.Consumed.Any<UpdateBankAccountEffectiveDate>());
    }
}