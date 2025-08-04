using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using Pi.Common.Features;
using Pi.WalletService.Application.Commands.Deposit;
using Pi.WalletService.Application.Commands.SendNotification;
using Pi.WalletService.Application.Options;
using Pi.WalletService.Application.Queries;
using Pi.WalletService.Application.Services.Bank;
using Pi.WalletService.Application.Services.UserService;
using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;
using Pi.WalletService.Domain.Events.Deposit;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Application.Tests.Commands.Deposit;

public class KkpDepositCallbackConsumerTests : ConsumerTest
{
    private readonly Mock<IUserService> _userService;
    private readonly Mock<IBankInfoService> _bankInfoService;
    private readonly Mock<IFeatureService> _featureService;
    private readonly Mock<ITransactionQueriesV2> _transactionQueriesV2;

    public KkpDepositCallbackConsumerTests()
    {
        _userService = new Mock<IUserService>();
        _bankInfoService = new Mock<IBankInfoService>();
        _featureService = new Mock<IFeatureService>();
        _transactionQueriesV2 = new Mock<ITransactionQueriesV2>();
        var feeOptionsMock = new Mock<IOptionsSnapshot<FeeOptions>>();

        feeOptionsMock.Setup(f => f.Value).Returns(new FeeOptions
        {
            KKP = new Kkp
            {
                DepositFee = "200",
                GlobalDepositFee = "200"
            }
        });

        var featuresOptionsMock = new Mock<IOptionsSnapshot<FeaturesOptions>>();

        featuresOptionsMock.Setup(f => f.Value).Returns(new FeaturesOptions
        {
            AllowDepositWithdrawV2CustCode = new List<string>() { "7711496" },
            ShouldGetBankAccountFromOnboardService = false
        });

        Provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg => { cfg.AddConsumer<ProcessKkpDepositCallbackConsumer>(); })
            .AddScoped(_ => _userService.Object)
            .AddScoped(_ => _bankInfoService.Object)
            .AddScoped(_ => _transactionQueriesV2.Object)
            .AddScoped(_ => feeOptionsMock.Object)
            .AddScoped(_ => featuresOptionsMock.Object)
            .AddScoped(_ => _featureService.Object)
            .BuildServiceProvider(true);
    }

    [Fact]
    public async void Should_Be_Able_To_Publish_DepositPaymentCallbackReceived_When_Product_Is_Global_Correctly()
    {
        // Arrange
        const string transactionNo = "123456";
        const double amount = 200;
        const string payerName = "Tik Jetsadaporn";
        const string bankName = "Smile Comedy Bank";
        const string bankShortName = "SCB";
        const string bankCode = "022";
        const string bankIconUrl = "";

        _bankInfoService
            .Setup(b => b.GetByBankCode(bankCode))
            .ReturnsAsync(new BankInfo(
                bankName,
                bankShortName,
                bankCode,
                bankIconUrl
            ));

        // Act
        var client = Harness.GetRequestClient<KkpDeposit>();

        var eventResp = await client.GetResponse<KkpDepositCompleted>(
            new KkpDeposit(
            true,
            amount,
            "7711431",
            Product.GlobalEquities.ToString(),
            transactionNo,
            "ref_code",
            "20220525145048",
            payerName,
            "1112221543",
            bankCode));

        await Harness.Published.Any<DepositPaymentCallbackReceived>();

        var response = Harness.Published.Select<DepositPaymentCallbackReceived>().First().Context;

        // Assert
        Assert.Equal(transactionNo, eventResp.Message.TransactionNo);
        Assert.Equal(transactionNo, response.Message.TransactionNo);
        Assert.Equal(200, response.Message.BankFee);
        Assert.Equal((decimal)amount, response.Message.PaymentReceivedAmount);
        Assert.Equal(payerName, response.Message.BankAccountName);
        Assert.Equal(bankName, response.Message.BankName);
        Assert.Equal(bankShortName, response.Message.BankShortName);
        Assert.Equal(bankCode, response.Message.BankCode);
    }

    [Fact]
    public async void Should_Be_Able_To_Publish_DepositPaymentCallbackReceived_When_Product_Is_Not_Global_And_ForceV2_Correctly()
    {
        // Arrange
        const string transactionNo = "123456";
        const double amount = 200;
        const string payerName = "Tik Jetsadaporn";
        const string bankName = "Smile Comedy Bank";
        const string bankShortName = "SCB";
        const string bankCode = "022";
        const string bankIconUrl = "";

        _bankInfoService
            .Setup(b => b.GetByBankCode(bankCode))
            .ReturnsAsync(new BankInfo(
                bankName,
                bankShortName,
                bankCode,
                bankIconUrl
            ));

        _featureService.Setup(f => f.IsOn("deposit-v2")).Returns(false);

        // Act
        var client = Harness.GetRequestClient<KkpDeposit>();

        var eventResp = await client.GetResponse<KkpDepositCompleted>(
            new KkpDeposit(
                true,
                amount,
                "77114968",
                Product.GlobalEquities.ToString(),
                transactionNo,
                "ref_code",
                "20220525145048",
                payerName,
                "1112221543",
                bankCode));

        await Harness.Published.Any<DepositPaymentCallbackReceived>();

        var response = Harness.Published.Select<DepositPaymentCallbackReceived>().First().Context;

        // Assert
        Assert.Equal(transactionNo, eventResp.Message.TransactionNo);
        Assert.Equal(transactionNo, response.Message.TransactionNo);
        Assert.Equal(200, response.Message.BankFee);
        Assert.Equal((decimal)amount, response.Message.PaymentReceivedAmount);
        Assert.Equal(payerName, response.Message.BankAccountName);
        Assert.Equal(bankName, response.Message.BankName);
        Assert.Equal(bankShortName, response.Message.BankShortName);
        Assert.Equal(bankCode, response.Message.BankCode);
    }

    [Fact]
    public async void Should_Be_Able_To_Publish_Notification_When_Product_Is_Not_Global_Or_Cash_Or_Derivaties()
    {
        // Arrange
        const string transactionNo = "123456";
        const double amount = 200;
        const string payerName = "Tik Jetsadaporn";
        const string bankName = "Smile Comedy Bank";
        const string bankShortName = "SCB";
        const string bankCode = "022";
        const string bankIconUrl = "";
        const string custCode = "7711431";
        const string tradingAccountCode = "7711431M";
        var userId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        _bankInfoService
            .Setup(b => b.GetByBankCode(bankCode))
            .ReturnsAsync(new BankInfo(
                bankName,
                bankShortName,
                bankCode,
                bankIconUrl
            ));
        _userService
            .Setup(u => u.GetUserInfoByCustomerCode(It.IsAny<string>()))
            .ReturnsAsync(new User(
                userId,
                new List<string>
                {
                    custCode
                },
                new List<string>
                {
                    tradingAccountCode
                },
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty
            ));
        _transactionQueriesV2
            .Setup(u => u.GetTransactionByTransactionNo(It.IsAny<string>(), null))
            .ReturnsAsync(new Transaction(correlationId, null, transactionNo, userId.ToString(), string.Empty
                , string.Empty, Channel.QR, Product.Cash, Purpose.Collateral, 10, 10, null, null, null, null, null
                , string.Empty, null, null, TransactionType.Deposit, DateTime.Now, DateTime.Now));

        // Act
        var client = Harness.GetRequestClient<KkpDeposit>();

        var eventResp = await client.GetResponse<KkpDepositCompleted>(
            new KkpDeposit(
                true,
                amount,
                tradingAccountCode,
                Product.Funds.ToString(),
                transactionNo,
                "ref_code",
                "20220525145048",
                payerName,
                "1112221543",
                bankCode
            ));

        var response = Harness.Published.Select<DepositWithdrawSuccessNotification>().First().Context;

        // Assert
        Assert.Equal(transactionNo, eventResp.Message.TransactionNo);
        Assert.Equal(correlationId, response.Message.CorrelationId);
    }

    [Fact]
    public async void Should_Be_Able_To_Publish_DepositFailedEvent_When_Product_Is_Not_Global_Or_Cash_Or_Derivatives_And_Status_Is_Not_Success()
    {
        // Arrange
        const string transactionNo = "123456";
        const double amount = 200;
        const string payerName = "Tik Jetsadaporn";
        const string bankName = "Smile Comedy Bank";
        const string bankShortName = "SCB";
        const string bankCode = "022";
        const string bankIconUrl = "";
        const string custCode = "7711431";
        const string tradingAccountCode = "77114311";
        var userId = Guid.NewGuid();

        _bankInfoService
            .Setup(b => b.GetByBankCode(bankCode))
            .ReturnsAsync(new BankInfo(
                bankName,
                bankShortName,
                bankCode,
                bankIconUrl
            ));
        _userService
            .Setup(u => u.GetUserInfoByCustomerCode(It.IsAny<string>()))
            .ReturnsAsync(new User(
                userId,
                new List<string>
                {
                    custCode
                },
                new List<string>
                {
                    tradingAccountCode
                },
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty
            ));
        var client = Harness.GetRequestClient<KkpDeposit>();

        // Act
        await client.GetResponse<KkpDepositCompleted>(
            new KkpDeposit(
                false,
                amount,
                tradingAccountCode,
                Product.CashBalance.ToString(),
                transactionNo,
                "ref_code",
                "20220525145048",
                payerName,
                "1112221543",
                bankCode
            ));

        // Assert
        Assert.True(await Harness.Published.Any<DepositPaymentFailed>());
        var failedEvent = Harness.Published.Select<DepositPaymentFailed>().First().Context;
        Assert.Equal(transactionNo, failedEvent.Message.TransactionNo);
    }
}