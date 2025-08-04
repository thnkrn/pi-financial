using System.Globalization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Pi.Financial.Client.Freewill.Api;
using Pi.Financial.Client.Freewill.Model;
using Pi.WalletService.Application.Services.CustomerService;
using Pi.WalletService.Domain.AggregatesModel.LogAggregate;
using Pi.WalletService.Infrastructure.Services;
using Pi.WalletService.IntegrationEvents.AggregatesModel;
namespace Pi.WalletService.Infrastructure.Tests.Services;

public class FreewillCustomerServiceTests
{
    private readonly ICustomerService _customerService;
    private readonly Mock<ICustomerModuleApi> _freeWillClientMock;

    private const string ResponseDateFormat = "dd/MM/yyyy";

    public FreewillCustomerServiceTests()
    {
        _freeWillClientMock = new Mock<ICustomerModuleApi>();
        Mock<ILogger<FreewillCustomerService>> loggerMock = new();
        Mock<ICreditModuleApi> freewillCreditModuleMock = new();
        Mock<IFreewillRequestLogRepository> freewillRequestLogRepositoryMock = new();

        var inMemorySettings = new Dictionary<string, string> {
            {"Freewill:UseMockValue", "false"},
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings!)
            .Build();

        _customerService = new FreewillCustomerService(
            _freeWillClientMock.Object,
            loggerMock.Object,
            configuration,
            freewillCreditModuleMock.Object,
            freewillRequestLogRepositoryMock.Object
        );
    }

    [Theory]
    [InlineData(TransactionType.Deposit, "R", Product.Cash, "CC")]
    [InlineData(TransactionType.Deposit, "R", Product.CashBalance, "CH")]
    [InlineData(TransactionType.Deposit, "R", Product.CreditBalance, "CB")]
    [InlineData(TransactionType.Deposit, "R", Product.CreditBalanceSbl, "BB")]
    [InlineData(TransactionType.Deposit, "R", Product.Derivatives, "TF")]
    [InlineData(TransactionType.Deposit, "R", Product.GlobalEquities, "XU")]
    [InlineData(TransactionType.Deposit, "R", Product.Funds, "UT")]
    [InlineData(TransactionType.Withdraw, "P", Product.Cash, "CC")]
    [InlineData(TransactionType.Withdraw, "P", Product.CashBalance, "CH")]
    [InlineData(TransactionType.Withdraw, "P", Product.CreditBalance, "CB")]
    [InlineData(TransactionType.Withdraw, "P", Product.CreditBalanceSbl, "BB")]
    [InlineData(TransactionType.Withdraw, "P", Product.Derivatives, "TF")]
    [InlineData(TransactionType.Withdraw, "P", Product.GlobalEquities, "XU")]
    [InlineData(TransactionType.Withdraw, "P", Product.Funds, "UT")]
    public async Task Should_Able_To_Get_Bank_Account_Successfully(TransactionType transactionType, string rpType, Product product, string acctCode)
    {
        // Arrange
        const string custCode = "7711496";
        var now = DateTime.Now;
        const string bankAccount = "bankAccount";
        const string bankCode = "bankCode";

        _freeWillClientMock
            .Setup(f => f.QueryCustomerBankAccountInfoAsync(It.IsAny<GetBankAccInfoRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetBankAccInfoResponse(
                "",
                "",
                "",
                ResultCode._000,
                "",
                1,
                new List<GetBankAccInfoResponseItem>
                {
                    new(
                        "",
                        "",
                        "WD",
                        rpType,
                        bankCode,
                        bankAccount,
                        "",
                        "",
                        acctCode,
                        now.AddDays(-1).ToString(ResponseDateFormat, CultureInfo.InvariantCulture),
                        now.AddDays(1).ToString(ResponseDateFormat, CultureInfo.InvariantCulture),
                        custCode,
                        "",
                        "",
                        "",
                        "",
                        ""
                        ),
                    new(
                        "",
                        "",
                        "WD",
                        rptype: rpType == "R" ? "P" : rpType,
                        "1234",
                        "123",
                        "",
                        "",
                        acctCode,
                        now.AddDays(-1).ToString(ResponseDateFormat, CultureInfo.InvariantCulture),
                        now.AddDays(1).ToString(ResponseDateFormat, CultureInfo.InvariantCulture),
                        custCode,
                        "",
                        "",
                        "",
                        "",
                        ""
                        )
                }));

        // Act
        var response = await _customerService.GetCustomerBankAccount(
            custCode,
            product,
            transactionType);

        Assert.NotNull(response);
        Assert.Equal(bankCode, response.BankCode);
        Assert.Equal(bankAccount, response.BankAccountNo);
    }

    [Fact]
    public async Task Should_Able_To_Fallback_To_CashBalance_BankAccount_Successfully()
    {
        // Arrange
        const string custCode = "7711496";
        var now = DateTime.Now;
        const string bankAccount = "bankAccount";
        const string bankCode = "bankCode";

        _freeWillClientMock
            .Setup(f => f.QueryCustomerBankAccountInfoAsync(It.IsAny<GetBankAccInfoRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetBankAccInfoResponse(
                "",
                "",
                "",
                ResultCode._000,
                "",
                1,
                new List<GetBankAccInfoResponseItem>
                {
                    new(
                        "7711496-8",
                        "",
                        "WD",
                        rptype: "R",
                        bankCode,
                        bankAccount,
                        "",
                        "",
                        "CH",
                        now.AddDays(-1).ToString(ResponseDateFormat, CultureInfo.InvariantCulture),
                        now.AddDays(1).ToString(ResponseDateFormat, CultureInfo.InvariantCulture),
                        custCode,
                        "",
                        "",
                        "",
                        "",
                        ""
                        ),
                    new(
                        "7711496-7",
                        "",
                        "WD",
                        rptype: "R",
                        "1234",
                        "123",
                        "",
                        "",
                        "CB",
                        now.AddDays(-1).ToString(ResponseDateFormat, CultureInfo.InvariantCulture),
                        now.AddDays(1).ToString(ResponseDateFormat, CultureInfo.InvariantCulture),
                        custCode,
                        "",
                        "",
                        "",
                        "",
                        ""
                    )
                }));

        // Act
        var response = await _customerService.GetCustomerBankAccount(
            custCode,
            Product.Derivatives,
            TransactionType.Deposit);

        Assert.NotNull(response);
        Assert.Equal(bankCode, response.BankCode);
        Assert.Equal(bankAccount, response.BankAccountNo);
    }

    [Fact]
    public async Task Should_Select_WD_First_BankAccount_In_Correct_Order_Successfully()
    {
        // Arrange
        const string custCode = "7711496";
        var now = DateTime.Now;
        const string bankAccount = "123";
        const string bankCode = "1234";

        _freeWillClientMock
            .Setup(f => f.QueryCustomerBankAccountInfoAsync(It.IsAny<GetBankAccInfoRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetBankAccInfoResponse(
                "",
                "",
                "",
                ResultCode._000,
                "",
                1,
                new List<GetBankAccInfoResponseItem>
                {
                    new(
                        "7711496-8",
                        "",
                        "WD",
                        rptype: "R",
                        "not-expect-bank-code",
                        "not-expect-bank-account",
                        "",
                        "",
                        "CH",
                        now.AddDays(-1).ToString(ResponseDateFormat, CultureInfo.InvariantCulture),
                        now.AddDays(1).ToString(ResponseDateFormat, CultureInfo.InvariantCulture),
                        custCode,
                        "",
                        "",
                        "",
                        "",
                        ""
                        ),
                    new(
                        "7711496-7",
                        "",
                        "WD",
                        rptype: "R",
                        bankCode,
                        bankAccount,
                        "",
                        "",
                        "TF",
                        now.AddDays(-1).ToString(ResponseDateFormat, CultureInfo.InvariantCulture),
                        now.AddDays(1).ToString(ResponseDateFormat, CultureInfo.InvariantCulture),
                        custCode,
                        "",
                        "",
                        "",
                        "",
                        ""
                    ),
                    new(
                        "7711496-7",
                        "",
                        "TRADE",
                        rptype: "R",
                        "not-expect-bank-code",
                        "not-expect-bank-account",
                        "",
                        "",
                        "TF",
                        now.AddDays(-1).ToString(ResponseDateFormat, CultureInfo.InvariantCulture),
                        now.AddDays(1).ToString(ResponseDateFormat, CultureInfo.InvariantCulture),
                        custCode,
                        "",
                        "",
                        "",
                        "",
                        ""
                    )
                }));

        // Act
        var response = await _customerService.GetCustomerBankAccount(
            custCode,
            Product.Derivatives,
            TransactionType.Deposit);

        Assert.NotNull(response);
        Assert.Equal(bankCode, response.BankCode);
        Assert.Equal(bankAccount, response.BankAccountNo);
    }

    [Fact]
    public async Task Should_Select_TRADE_IF_WD_Missing_BankAccount_In_Correct_Order_Successfully()
    {
        // Arrange
        const string custCode = "7711496";
        var now = DateTime.Now;
        const string bankAccount = "123";
        const string bankCode = "1234";

        _freeWillClientMock
            .Setup(f => f.QueryCustomerBankAccountInfoAsync(It.IsAny<GetBankAccInfoRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetBankAccInfoResponse(
                "",
                "",
                "",
                ResultCode._000,
                "",
                1,
                new List<GetBankAccInfoResponseItem>
                {
                    new(
                        "7711496-8",
                        "",
                        "WD",
                        rptype: "R",
                        "not-expect-bank-code",
                        "not-expect-bank-account",
                        "",
                        "",
                        "CH",
                        now.AddDays(-1).ToString(ResponseDateFormat, CultureInfo.InvariantCulture),
                        now.AddDays(1).ToString(ResponseDateFormat, CultureInfo.InvariantCulture),
                        custCode,
                        "",
                        "",
                        "",
                        "",
                        ""
                        ),
                    new(
                        "7711496-7",
                        "",
                        "MEDIA",
                        rptype: "R",
                        "not-expect-bank-code",
                        "not-expect-bank-account",
                        "",
                        "",
                        "TF",
                        now.AddDays(-1).ToString(ResponseDateFormat, CultureInfo.InvariantCulture),
                        now.AddDays(1).ToString(ResponseDateFormat, CultureInfo.InvariantCulture),
                        custCode,
                        "",
                        "",
                        "",
                        "",
                        ""
                    ),
                    new(
                        "7711496-7",
                        "",
                        "TRADE",
                        rptype: "R",
                        bankCode,
                        bankAccount,
                        "",
                        "",
                        "TF",
                        now.AddDays(-1).ToString(ResponseDateFormat, CultureInfo.InvariantCulture),
                        now.AddDays(1).ToString(ResponseDateFormat, CultureInfo.InvariantCulture),
                        custCode,
                        "",
                        "",
                        "",
                        "",
                        ""
                    )
                }));

        // Act
        var response = await _customerService.GetCustomerBankAccount(
            custCode,
            Product.Derivatives,
            TransactionType.Deposit);

        Assert.NotNull(response);
        Assert.Equal(bankCode, response.BankCode);
        Assert.Equal(bankAccount, response.BankAccountNo);
    }

    [Fact]
    public async Task Should_Select_WD_IN_CH_IF_AccCode_Missing_BankAccount_In_Correct_Order_Successfully()
    {
        // Arrange
        const string custCode = "7711496";
        var now = DateTime.Now;
        const string bankAccount = "123";
        const string bankCode = "1234";

        _freeWillClientMock
            .Setup(f => f.QueryCustomerBankAccountInfoAsync(It.IsAny<GetBankAccInfoRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetBankAccInfoResponse(
                "",
                "",
                "",
                ResultCode._000,
                "",
                1,
                new List<GetBankAccInfoResponseItem>
                {
                    new(
                        "7711496-8",
                        "",
                        "WD",
                        rptype: "R",
                        bankCode,
                        bankAccount,
                        "",
                        "",
                        "CH",
                        now.AddDays(-1).ToString(ResponseDateFormat, CultureInfo.InvariantCulture),
                        now.AddDays(1).ToString(ResponseDateFormat, CultureInfo.InvariantCulture),
                        custCode,
                        "",
                        "",
                        "",
                        "",
                        ""
                        ),
                    new(
                        "7711496-7",
                        "",
                        "MEDIA",
                        rptype: "R",
                        "not-expect-bank-code",
                        "not-expect-bank-account",
                        "",
                        "",
                        "",
                        now.AddDays(-1).ToString(ResponseDateFormat, CultureInfo.InvariantCulture),
                        now.AddDays(1).ToString(ResponseDateFormat, CultureInfo.InvariantCulture),
                        custCode,
                        "",
                        "",
                        "",
                        "",
                        ""
                    ),
                    new(
                        "7711496-7",
                        "",
                        "TRADE",
                        rptype: "R",
                        "not-expect-bank-code",
                        "not-expect-bank-account",
                        "",
                        "",
                        "",
                        now.AddDays(-1).ToString(ResponseDateFormat, CultureInfo.InvariantCulture),
                        now.AddDays(1).ToString(ResponseDateFormat, CultureInfo.InvariantCulture),
                        custCode,
                        "",
                        "",
                        "",
                        "",
                        ""
                    )
                }));

        // Act
        var response = await _customerService.GetCustomerBankAccount(
            custCode,
            Product.Derivatives,
            TransactionType.Deposit);

        Assert.NotNull(response);
        Assert.Equal(bankCode, response.BankCode);
        Assert.Equal(bankAccount, response.BankAccountNo);
    }

    [Fact]
    public async Task Should_Select_TRADE_IF_WD_IN_CH_And_AccCode_Missing_BankAccount_In_Correct_Order_Successfully()
    {
        // Arrange
        const string custCode = "7711496";
        var now = DateTime.Now;
        const string bankAccount = "123";
        const string bankCode = "1234";

        _freeWillClientMock
            .Setup(f => f.QueryCustomerBankAccountInfoAsync(It.IsAny<GetBankAccInfoRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetBankAccInfoResponse(
                "",
                "",
                "",
                ResultCode._000,
                "",
                1,
                new List<GetBankAccInfoResponseItem>
                {
                    new(
                        "7711496-8",
                        "",
                        "TRADE",
                        rptype: "R",
                        bankCode,
                        bankAccount,
                        "",
                        "",
                        "CH",
                        now.AddDays(-1).ToString(ResponseDateFormat, CultureInfo.InvariantCulture),
                        now.AddDays(1).ToString(ResponseDateFormat, CultureInfo.InvariantCulture),
                        custCode,
                        "",
                        "",
                        "",
                        "",
                        ""
                        ),
                    new(
                        "7711496-7",
                        "",
                        "MEDIA",
                        rptype: "R",
                        "not-expect-bank-code",
                        "not-expect-bank-account",
                        "",
                        "",
                        "",
                        now.AddDays(-1).ToString(ResponseDateFormat, CultureInfo.InvariantCulture),
                        now.AddDays(1).ToString(ResponseDateFormat, CultureInfo.InvariantCulture),
                        custCode,
                        "",
                        "",
                        "",
                        "",
                        ""
                    ),
                    new(
                        "7711496-7",
                        "",
                        "TRADE",
                        rptype: "R",
                        "not-expect-bank-code",
                        "not-expect-bank-account",
                        "",
                        "",
                        "",
                        now.AddDays(-1).ToString(ResponseDateFormat, CultureInfo.InvariantCulture),
                        now.AddDays(1).ToString(ResponseDateFormat, CultureInfo.InvariantCulture),
                        custCode,
                        "",
                        "",
                        "",
                        "",
                        ""
                    )
                }));

        // Act
        var response = await _customerService.GetCustomerBankAccount(
            custCode,
            Product.Derivatives,
            TransactionType.Deposit);

        Assert.NotNull(response);
        Assert.Equal(bankCode, response.BankCode);
        Assert.Equal(bankAccount, response.BankAccountNo);
    }
}