using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Pi.Financial.Client.Freewill.Model;
using Pi.WalletService.Application.Models;
using Pi.WalletService.Application.Options;
using Pi.WalletService.Application.Queries;
using Pi.WalletService.Application.Services;
using Pi.WalletService.Application.Services.Bank;
using Pi.WalletService.Application.Services.CustomerService;
using Pi.WalletService.Application.Services.GlobalEquities;
using Pi.WalletService.Application.Services.OnboardService;
using Pi.WalletService.Application.Services.SetTrade;
using Pi.WalletService.Application.Services.UserService;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Application.Tests.Queries;

public class WalletQueriesTest
{
    private readonly Mock<IGlobalTradeService> _globalTradeServiceMock;
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<IOnboardService> _onboardServiceMock;
    private readonly Mock<ICustomerService> _freewillCustomerServiceMock;
    private readonly Mock<IBankInfoService> _bankInfoServiceMock;
    private readonly Mock<ISetTradeService> _setTradeServiceMock;
    private readonly Mock<IValidationService> _validationService;
    private readonly IWalletQueries _walletQueries;

    public WalletQueriesTest()
    {
        _globalTradeServiceMock = new Mock<IGlobalTradeService>();
        _userServiceMock = new Mock<IUserService>();
        _onboardServiceMock = new Mock<IOnboardService>();
        _freewillCustomerServiceMock = new Mock<ICustomerService>();
        _bankInfoServiceMock = new Mock<IBankInfoService>();
        _setTradeServiceMock = new Mock<ISetTradeService>();
        _validationService = new Mock<IValidationService>();
        Mock<ILogger<WalletQueries>> logger = new();
        var featuresOptionsMock = new Mock<IOptionsSnapshot<FeaturesOptions>>();

        featuresOptionsMock.Setup(f => f.Value).Returns(new FeaturesOptions
        {
            AllowDepositWithdrawV2CustCode = new List<string>() { "7711496" },
            ShouldGetBankAccountFromOnboardService = true,
            OddDepositBankCode = new List<string>() { "002", "004", "006", "014" }
        });


        _walletQueries = new WalletQueries(
            _globalTradeServiceMock.Object,
            _userServiceMock.Object,
            _onboardServiceMock.Object,
            _freewillCustomerServiceMock.Object,
            _bankInfoServiceMock.Object,
            _setTradeServiceMock.Object,
            logger.Object,
            featuresOptionsMock.Object,
            _validationService.Object
        );
    }

    [Fact]
    public async Task WalletQueries_GetAvailableWithdrawalAmount_ShouldThrowExceptionWhenCustCodeInvalid()
    {
        var custCode = "12345";
        var user = new User(
            Guid.NewGuid(),
            new List<string>(),
            new List<string>(),
            "",
            "",
            "",
            "",
            "",
            "",
            ""
        );

        _userServiceMock.Setup(u => u.GetUserInfoById(It.IsAny<string>())).ReturnsAsync(user);

        await Assert.ThrowsAsync<InvalidDataException>(async () =>
            await _walletQueries.GetAvailableWithdrawalAmount("userId", custCode, Product.GlobalEquities));
    }

    [Fact]
    public async Task WalletQueries_GetAvailableWithdrawalAmount_ShouldThrowExceptionWhenAccountCodeNotFound()
    {
        var custCode = "12345";
        var user = new User(
            Guid.NewGuid(),
            new List<string>
            {
                custCode
            },
            new List<string>(),
            "",
            "",
            "",
            "",
            "",
            "",
            ""
        );

        _userServiceMock.Setup(u => u.GetUserInfoById(It.IsAny<string>())).ReturnsAsync(user);

        await Assert.ThrowsAsync<InvalidDataException>(async () =>
            await _walletQueries.GetAvailableWithdrawalAmount("userId", custCode, Product.GlobalEquities));
    }

    [Fact]
    public async Task WalletQueries_GetAvailableWithdrawalAmount_ShouldThrowExceptionWhenGEServiceFailed()
    {
        var custCode = "12345";
        var user = new User(
            Guid.NewGuid(),
            new List<string>
            {
                custCode
            },
            new List<string>
            {
                custCode + "2"
            },
            "",
            "",
            "",
            "",
            "globalAccount",
            "",
            ""
        );

        _userServiceMock.Setup(u => u.GetUserInfoById(It.IsAny<string>())).ReturnsAsync(user);
        _globalTradeServiceMock.Setup(u => u.GetAvailableWithdrawalAmount("globalAccount", It.IsAny<string>()))
            .ThrowsAsync(new Exception("GEServiceFailed"));

        await Assert.ThrowsAsync<Exception>(async () =>
            await _walletQueries.GetAvailableWithdrawalAmount("userId", custCode, Product.GlobalEquities));
    }

    [Fact]
    public async Task WalletQueries_GetAvailableWithdrawalAmount_ShouldThrowExceptionWhenNonTFexServiceFailed()
    {
        var custCode = "12345";
        var user = new User(
            Guid.NewGuid(),
            new List<string>
            {
                custCode
            },
            new List<string>
            {
                custCode + "1"
            },
            "",
            "",
            "",
            "",
            "",
            "",
            ""
        );

        _userServiceMock.Setup(u => u.GetUserInfoById(It.IsAny<string>())).ReturnsAsync(user);
        _freewillCustomerServiceMock.Setup(u => u.QueryWithdrawalBalance(custCode + "1"))
            .ThrowsAsync(new Exception("NonTFexServiceFailed"));

        await Assert.ThrowsAsync<Exception>(async () =>
            await _walletQueries.GetAvailableWithdrawalAmount("userId", custCode, Product.Cash));
    }

    [Fact]
    public async Task WalletQueries_GetAvailableWithdrawalAmount_ShouldThrowExceptionWhenTFexServiceFailed()
    {
        var custCode = "12345";
        var user = new User(
            Guid.NewGuid(),
            new List<string>
            {
                custCode
            },
            new List<string>
            {
                custCode + "0"
            },
            "",
            "",
            "",
            "",
            "",
            "",
            ""
        );

        _userServiceMock.Setup(u => u.GetUserInfoById(It.IsAny<string>())).ReturnsAsync(user);
        _setTradeServiceMock.Setup(u => u.GetWithdrawalBalance(custCode + "0"))
            .ThrowsAsync(new Exception("NonTFexServiceFailed"));

        await Assert.ThrowsAsync<Exception>(async () =>
            await _walletQueries.GetAvailableWithdrawalAmount("userId", custCode, Product.Derivatives));
    }

    [Fact]
    public async Task WalletQueries_GetAvailableWithdrawalAmount_ShouldReturnSuccess_NonTFex()
    {
        var custCode = "12345";
        var user = new User(
            Guid.NewGuid(),
            new List<string>
            {
                custCode
            },
            new List<string>
            {
                custCode + "1"
            },
            "",
            "",
            "",
            "",
            "",
            "",
            ""
        );

        _userServiceMock.Setup(u => u.GetUserInfoById(It.IsAny<string>())).ReturnsAsync(user);
        _freewillCustomerServiceMock.Setup(u => u.QueryWithdrawalBalance(custCode + "1"))
            .ReturnsAsync(1999.99m);

        var response = await _walletQueries.GetAvailableWithdrawalAmount("userId", custCode, Product.Cash);

        Assert.Equal(1999.99m, response);
    }

    [Fact]
    public async Task WalletQueries_GetAvailableWithdrawalAmount_ShouldReturnSuccess_GE()
    {
        var custCode = "12345";
        var user = new User(
            Guid.NewGuid(),
            new List<string>
            {
                custCode
            },
            new List<string>
            {
                custCode + "2"
            },
            "",
            "",
            "",
            "",
            "globalAccount",
            "",
            ""
        );

        _userServiceMock.Setup(u => u.GetUserInfoById(It.IsAny<string>())).ReturnsAsync(user);
        _globalTradeServiceMock.Setup(u => u.GetAvailableWithdrawalAmount("globalAccount", It.IsAny<string>()))
            .ReturnsAsync(18888.99m);

        var response = await _walletQueries.GetAvailableWithdrawalAmount("userId", custCode, Product.GlobalEquities);

        Assert.Equal(18888.99m, response);
    }

    [Fact]
    public async Task WalletQueries_GetAvailableWithdrawalAmount_ShouldReturnSuccess_TFex()
    {
        var custCode = "12345";
        var user = new User(
            Guid.NewGuid(),
            new List<string>
            {
                custCode
            },
            new List<string>
            {
                custCode + "0"
            },
            "",
            "",
            "",
            "",
            "globalAccount",
            "",
            ""
        );

        _userServiceMock.Setup(u => u.GetUserInfoById(It.IsAny<string>())).ReturnsAsync(user);
        _setTradeServiceMock.Setup(u => u.GetWithdrawalBalance(custCode + "0"))
            .ReturnsAsync(199.99m);
        _freewillCustomerServiceMock.Setup(u => u.QueryWithdrawalBalance(custCode + "0"))
            .ReturnsAsync(199.99m);

        var response = await _walletQueries.GetAvailableWithdrawalAmount("userId", custCode, Product.Derivatives);

        Assert.Equal(199.99m, response);
    }

    [Theory]
    [InlineData(199.99, 150.00, 150.00)]
    [InlineData(199.99, 200.00, 199.99)]
    public async Task WalletQueries_GetAvailableWithdrawalAmount_ShouldReturnLowerValue_TFex(decimal freewillValue, decimal settTradeValue, decimal expected)
    {
        var custCode = "12345";
        var user = new User(
            Guid.NewGuid(),
            new List<string>
            {
                custCode
            },
            new List<string>
            {
                custCode + "0"
            },
            "",
            "",
            "",
            "",
            "globalAccount",
            "",
            ""
        );

        _userServiceMock.Setup(u => u.GetUserInfoById(It.IsAny<string>())).ReturnsAsync(user);
        _setTradeServiceMock.Setup(u => u.GetWithdrawalBalance(custCode + "0"))
            .ReturnsAsync(settTradeValue);
        _freewillCustomerServiceMock.Setup(u => u.QueryWithdrawalBalance(custCode + "0"))
            .ReturnsAsync(freewillValue);

        var response = await _walletQueries.GetAvailableWithdrawalAmount("userId", custCode, Product.Derivatives);

        Assert.Equal(expected, response);
    }

    [Fact]
    public async Task WalletQueries_VerifyUserBalance_ShouldReturnCorrectResponse()
    {
        var user = new User(
            Guid.NewGuid(),
            new List<string>
            {
                "7711496"
            },
            new List<string>
            {
                "77114962"
            },
            "",
            "",
            "",
            "",
            "",
            "",
            ""
        );

        _userServiceMock
            .Setup(u => u.GetUserInfoById(It.IsAny<string>()))
            .ReturnsAsync(user);
        _globalTradeServiceMock
            .Setup(g => g.GetAvailableWithdrawalAmount(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(10);

        try
        {
            await _walletQueries.VerifyUserGeBalance("userId", "7711496", "THB", 10);
        }
        catch (Exception e)
        {
            Assert.Fail($"VerifyUserBalance failed: {e}");
        }
    }

    [Fact]
    public async Task WalletQueries_VerifyUserBalance_ShouldThrowExceptionWhenAccountCodeNotFound()
    {
        var user = new User(
            Guid.NewGuid(),
            new List<string>(),
            new List<string>(),
            "",
            "",
            "",
            "",
            "",
            "",
            ""
        );

        _userServiceMock.Setup(u => u.GetUserInfoById(It.IsAny<string>())).ReturnsAsync(user);

        await Assert.ThrowsAsync<InvalidDataException>(async () =>
            await _walletQueries.VerifyUserGeBalance("userId", "77114962", "THB", 10));
    }

    [Fact]
    public async Task WalletQueries_VerifyUserBalance_ShouldThrowExceptionWhenExanteAccountNotFound()
    {
        var user = new User(
            Guid.NewGuid(),
            new List<string>(),
            new List<string>
            {
                "77114962"
            },
            "",
            "",
            "",
            "",
            "",
            "",
            ""
        );

        AccountSummaryResponse? accountSummary = null;
        _userServiceMock.Setup(u => u.GetUserInfoById(It.IsAny<string>())).ReturnsAsync(user);
        _globalTradeServiceMock.Setup(g => g.GetAccountSummary(It.IsAny<string>(), It.IsAny<string>()))!
            .ReturnsAsync(accountSummary);

        await Assert.ThrowsAsync<InvalidDataException>(async () =>
            await _walletQueries.VerifyUserGeBalance("userId", "77114962", "THB", 10));
    }

    [Fact]
    public async Task WalletQueries_VerifyUserBalance_ShouldThrowExceptionWhenBalanceIsInsufficient()
    {
        var user = new User(
            Guid.NewGuid(),
            new List<string>(),
            new List<string>
            {
                "77114962"
            },
            "",
            "",
            "",
            "",
            "",
            "",
            ""
        );

        var accountSummary = new AccountSummaryResponse("", "", 123, "10", new List<Position>());
        _userServiceMock.Setup(u => u.GetUserInfoById(It.IsAny<string>())).ReturnsAsync(user);
        _globalTradeServiceMock.Setup(g => g.GetAccountSummary(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(accountSummary);

        await Assert.ThrowsAsync<InvalidDataException>(async () =>
            await _walletQueries.VerifyUserGeBalance("userId", "77114962", "THB", 100));
    }

    //============================================================

    [Fact]
    public async Task WalletQueries_GetBankAccount_ShouldThrowWhenCustCodeNotMatchedWithUser()
    {
        var user = new User(
            Guid.NewGuid(),
            new List<string>(),
            new List<string>
            {
                "77114962"
            },
            "",
            "",
            "",
            "",
            "",
            "",
            ""
        );

        _userServiceMock.Setup(u => u.GetUserInfoById(It.IsAny<string>())).ReturnsAsync(user);

        await Assert.ThrowsAsync<InvalidDataException>(async () => await _walletQueries.GetBankAccount("userId", "7711496", Product.GlobalEquities, TransactionType.Deposit));
    }

    [Fact]
    public async Task WalletQueries_GetBankAccount_ShouldReturnCorrectResponseWhenGetBankAccountFromFreewill()
    {
        var user = new User(
            Guid.NewGuid(),
            new List<string>
            {
                "7711496"
            },
            new List<string>
            {
                "77114962"
            },
            "",
            "",
            "",
            "",
            "",
            "",
            ""
        );
        var bankAccountFromFreewill = new BankAccount("bankCode", "bankAccountNo", "bankBranchCode");
        var bankInfo = new BankInfo("Elton John", "John", "123", "456");

        _userServiceMock.Setup(u => u.GetUserInfoById(It.IsAny<string>())).ReturnsAsync(user);
        _freewillCustomerServiceMock.Setup(c => c.GetCustomerBankAccount(It.IsAny<string>(), It.IsAny<Product>(), TransactionType.Withdraw)).ReturnsAsync(bankAccountFromFreewill);
        _bankInfoServiceMock.Setup(b => b.GetByBankCode(It.IsAny<string>())).ReturnsAsync(bankInfo);

        var response = await _walletQueries.GetBankAccount("userId", "7711496", Product.GlobalEquities, TransactionType.Withdraw);

        Assert.Equal(bankAccountFromFreewill.BankAccountNo, response.AccountNo);
        Assert.Equal(bankInfo.Name, response.Name);
        Assert.Equal(bankInfo.ShortName.ToUpperInvariant(), response.ShortName);
        Assert.Equal(bankInfo.Code, response.Code);
        Assert.Equal(bankInfo.IconUrl, response.IconUrl);
    }

    [Fact]
    public async Task WalletQueries_GetBankAccount_ShouldReturnCorrectResponseWhenGetBankAccountFromUser()
    {
        var user = new User(
            Guid.NewGuid(),
            new List<string>
            {
                "7711496"
            },
            new List<string>
            {
                "77114962"
            },
            "",
            "",
            "",
            "",
            "",
            "",
            ""
        );
        var bankAccountFromUser = new BankAccount("bankCode", "bankAccountNo", "bankBranchCode");
        var bankInfo = new BankInfo("Elton John", "John", "123", "456");

        _userServiceMock.Setup(u => u.GetUserInfoById(It.IsAny<string>())).ReturnsAsync(user);
        // _userServiceMock.Setup(u => u.GetBankAccount(It.IsAny<string>())).ReturnsAsync(bankAccountFromUser);
        _onboardServiceMock.Setup(o => o.GetBankAccount(It.IsAny<string>(), It.IsAny<Product>(), It.IsAny<TransactionType>())).ReturnsAsync(bankAccountFromUser);
        _bankInfoServiceMock.Setup(b => b.GetByBankCode(It.IsAny<string>())).ReturnsAsync(bankInfo);

        var response = await _walletQueries.GetBankAccount("userId", "7711496", Product.GlobalEquities, TransactionType.Withdraw);

        Assert.Equal(bankAccountFromUser.BankAccountNo, response.AccountNo);
        Assert.Equal(bankInfo.Name, response.Name);
        Assert.Equal(bankInfo.ShortName.ToUpperInvariant(), response.ShortName);
        Assert.Equal(bankInfo.Code, response.Code);
        Assert.Equal(bankInfo.IconUrl, response.IconUrl);
    }

    [Fact]
    public async Task WalletQueries_GetBankAccount_ShouldReturnNullWhenBankAccountFromFreewillNotFound()
    {
        var user = new User(
            Guid.NewGuid(),
            new List<string>
            {
                "7711496"
            },
            new List<string>
            {
                "77114962"
            },
            "",
            "",
            "",
            "",
            "",
            "",
            ""
        );

        var bankInfo = new BankInfo("Elton John", "John", "123", "456");

        _userServiceMock.Setup(u => u.GetUserInfoById(It.IsAny<string>())).ReturnsAsync(user);
        _freewillCustomerServiceMock.Setup(c => c.GetCustomerBankAccount(It.IsAny<string>(), It.IsAny<Product>(), TransactionType.Withdraw)).ReturnsAsync((BankAccount?)null);
        _bankInfoServiceMock.Setup(b => b.GetByBankCode(It.IsAny<string>())).ReturnsAsync(bankInfo);

        await Assert.ThrowsAsync<NoBankAccountFoundException>(async () => await _walletQueries.GetBankAccount("userId", "7711496", Product.GlobalEquities, TransactionType.Withdraw));
    }

    [Fact]
    public async Task WalletQueries_GetBankAccount_ShouldThrowExceptionWhenAccountCodeNotFound()
    {
        var user = new User(
            Guid.NewGuid(),
            new List<string>(),
            new List<string>
            {
                "123"
            },
            "",
            "",
            "",
            "",
            "",
            "",
            ""
        );

        _userServiceMock.Setup(u => u.GetUserInfoById(It.IsAny<string>())).ReturnsAsync(user);

        await Assert.ThrowsAsync<InvalidDataException>(async () => await _walletQueries.GetBankAccount("userId", "7711496", Product.GlobalEquities, TransactionType.Withdraw));
    }

    [Fact]
    public async Task WalletQueries_GetBankAccount_ShouldThrowExceptionWhenCustomerBankAccountNotFound()
    {
        var user = new User(
            Guid.NewGuid(),
            new List<string>(),
            new List<string>
            {
                "77114962"
            },
            "",
            "",
            "",
            "",
            "",
            "",
            ""
        );
        BankAccount? bankAccount = null;

        _userServiceMock.Setup(u => u.GetUserInfoById(It.IsAny<string>())).ReturnsAsync(user);
        _freewillCustomerServiceMock.Setup(c => c.GetCustomerBankAccount(It.IsAny<string>(), TransactionType.Withdraw)).ReturnsAsync(bankAccount);

        await Assert.ThrowsAsync<InvalidDataException>(async () => await _walletQueries.GetBankAccount("userId", "7711496", Product.GlobalEquities, TransactionType.Withdraw));
    }

    [Fact]
    public async Task WalletQueries_GetBankAccount_ShouldThrowExceptionWhenBankInfoNotFound()
    {
        var user = new User(
            Guid.NewGuid(),
            new List<string>(),
            new List<string>
            {
                "77114962"
            },
            "",
            "",
            "",
            "",
            "",
            "",
            ""
        );

        var bankAccountFromUser = new BankAccount("bankCode", "bankAccountNo", "bankBranchCode");

        _userServiceMock.Setup(u => u.GetUserInfoById(It.IsAny<string>())).ReturnsAsync(user);
        _userServiceMock.Setup(u => u.GetBankAccount(It.IsAny<string>())).ReturnsAsync(bankAccountFromUser);
        _bankInfoServiceMock.Setup(b => b.GetByBankCode(It.IsAny<string>())).ReturnsAsync((BankInfo?)null);

        await Assert.ThrowsAsync<InvalidDataException>(async () => await _walletQueries.GetBankAccount("userId", "7711496", Product.GlobalEquities, TransactionType.Withdraw));
    }

    [Fact]
    public async Task WalletQueries_GetGeDepositLimit_ShouldThrowWhenCustCodeNotMatchedWithUser()
    {
        var user = new User(
            Guid.NewGuid(),
            new List<string>(),
            new List<string>
            {
                "77114962"
            },
            "",
            "",
            "",
            "",
            "",
            "",
            ""
        );

        _userServiceMock.Setup(u => u.GetUserInfoById(It.IsAny<string>())).ReturnsAsync(user);

        await Assert.ThrowsAsync<InvalidDataException>(async () => await _walletQueries.GetGeDepositLimit("userId", "77114962", "THB"));
    }

    [Fact]
    public async Task WalletQueries_GetGeDepositLimit_ShouldReturnCorrectResponse()
    {
        var user = new User(
            Guid.NewGuid(),
            new List<string>
            {
                "7711496"
            },
            new List<string>
            {
                "77114962"
            },
            "",
            "",
            "",
            "",
            "",
            "",
            ""
        );
        var accountSummary = new AccountSummaryResponse("", "", 123, "10",
            new List<Position>
            {
                new("THB", decimal.One, 10, 1, 10),
                new("USD", decimal.One, 35, 1, 350)
            });

        _userServiceMock.Setup(u => u.GetUserInfoById(It.IsAny<string>())).ReturnsAsync(user);
        _userServiceMock.Setup(u => u.GetCreditLimit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Product>())).ReturnsAsync(10);
        _globalTradeServiceMock.Setup(g => g.GetAccountSummary(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(accountSummary);

        var response = await _walletQueries.GetGeDepositLimit("userId", "7711496", "THB");

        Assert.Equal(10, response.AccountLimit);
        Assert.Equal(10, response.UnusedCash);
        Assert.Equal(0, response.DepositLimit);
    }

    [Fact]
    public async Task WalletQueries_GetAtsRegistrationStatus_ShouldReturnCorrectStatus()
    {
        var queryAtsResponse = new ICustomerService.QueryAtsResponse(
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            ResultCode._000,
            ""
        );

        var user = new User(
            Guid.NewGuid(),
            new List<string>(),
            new List<string>
            {
                "77114968"
            },
            "",
            "",
            "",
            "",
            "",
            "",
            ""
        );

        _userServiceMock.Setup(u => u.GetUserInfoById(It.IsAny<string>())).ReturnsAsync(user);
        _freewillCustomerServiceMock
            .Setup(f => f.QueryAts(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(queryAtsResponse);

        var response = await _walletQueries.GetAtsRegistrationStatus("");
        Assert.True(response);
    }

    [Fact]
    public async Task WalletQueries_GetAtsRegistrationStatus_ShouldReturnTrueWhenTradingAccountIsNull()
    {
        var user = new User(
            Guid.NewGuid(),
            new List<string>(),
            new List<string>(),
            "",
            "",
            "",
            "",
            "",
            "",
            ""
        );

        _userServiceMock.Setup(u => u.GetUserInfoById(It.IsAny<string>())).ReturnsAsync(user);

        var response = await _walletQueries.GetAtsRegistrationStatus("");
        Assert.True(response);
    }

    [Theory]
    [InlineData("014", Product.Cash, true, false)]
    [InlineData("014", Product.Cash, false, true)]
    [InlineData("002", Product.GlobalEquities, true, true)]
    [InlineData("004", Product.GlobalEquities, false, true)]
    [InlineData("011", Product.GlobalEquities, true, false)]
    [InlineData("025", Product.GlobalEquities, false, false)]
    public async Task WalletQueries_CheckAtsAvailable_ShouldReturnCorrectResponse(string bankCode, Product product, bool isOutsideWorkingHour, bool expected)
    {
        var user = new User(
            Guid.NewGuid(),
            new List<string>
            {
                "0800280"
            },
            new List<string>
            {
                "08002802"
            },
            "",
            "",
            "",
            "",
            "",
            "",
            ""
        );
        var bankAccountFromUser = new BankAccount("bankCode", "bankAccountNo", "bankBranchCode");
        var bankInfo = new BankInfo("Elton John", "John", bankCode, "456");

        if (product != Product.GlobalEquities)
        {
            ValidationResult validateResult;
            _validationService.Setup(u => u.IsOutsideWorkingHour(product, Channel.ATS, It.IsAny<DateTime>(), out validateResult))
                .Returns(isOutsideWorkingHour);
        }
        else
        {
            _userServiceMock.Setup(u => u.GetUserInfoById(It.IsAny<string>())).ReturnsAsync(user);
            _onboardServiceMock.Setup(o => o.GetBankAccount(It.IsAny<string>(), It.IsAny<Product>(), It.IsAny<TransactionType>())).ReturnsAsync(bankAccountFromUser);
            _bankInfoServiceMock.Setup(b => b.GetByBankCode(It.IsAny<string>())).ReturnsAsync(bankInfo);
        }

        var response = await _walletQueries.CheckAtsAvailable("userId", "0800280", product);
        Assert.Equal(expected, response);
    }

}