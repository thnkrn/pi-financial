using FluentAssertions;
using Pi.User.Application.Models;
using Moq;
using Pi.User.Application.Queries;
using Pi.User.Domain.AggregatesModel.TradingAccountAggregate;
using Pi.User.Domain.AggregatesModel.UserInfoAggregate;
using Microsoft.Extensions.Logging;
using Pi.User.Application.Services.Onboard;
using Pi.Common.Domain.AggregatesModel.ProductAggregate;
using Pi.Client.OnboardService.Model;
using Pi.User.Application.Services.SSO;

namespace Pi.User.Application.Tests.Queries;

public class UserTradingAccountQueriesTests
{
    private readonly UserTradingAccountQueries _userTradingAccountQueries;
    private readonly Mock<ILogger<UserTradingAccountQueries>> _mockLogger = new();
    private readonly Mock<IProductRepository> _mockProductRepository = new();
    private readonly Mock<ITradingAccountRepository> _mockTradingAccountRepository = new();
    private readonly Mock<IUserInfoRepository> _mockUserInfoRepository = new();
    private readonly Mock<IUserQueries> _mockUserQueries = new();
    private readonly Mock<IOnboardTradingAccountService> _mockOnboardTradingAccountService = new();
    private readonly Mock<ISsoService> _mockSsoService = new();

    private static readonly List<string> SAccountTypeCodes =
        ["BB", "CT", "LC", "CC", "XU", "DC", "CB", "CH", "TF", "UT", "LH"];

    public UserTradingAccountQueriesTests()
    {
        foreach (string accountTypeCode in SAccountTypeCodes)
        {
            _mockProductRepository
                .Setup(oats => oats.GetProductNameFromAccountTypeCode(accountTypeCode))
                .ReturnsAsync(GetProductNameFromAccountTypeCode(accountTypeCode));
        }

        _userTradingAccountQueries = new UserTradingAccountQueries(
            _mockUserInfoRepository.Object,
            _mockTradingAccountRepository.Object,
            _mockProductRepository.Object,
            _mockOnboardTradingAccountService.Object,
            _mockLogger.Object,
            _mockUserQueries.Object,
            _mockSsoService.Object);
    }

    [Fact]
    public async Task
        GetUserTradingAccountsWithExternalAccountsByUserId_WhenUserHaveMultipleCustCodesWithMultipleTradingAccountsWithMultipleExternalAccounts_ShouldListOfTradingAccountsGroupedByCustCode()
    {
        // Arrange
        var userId = Guid.NewGuid();
        string identificationNo = "0123456789012";
        string custCodeA = "custcode-a";
        string custCodeB = "custcode-b";

        // Mock user queries.
        var user = new Pi.User.Application.Models.User(userId, [], [], [], null, null, null, null, null, null, null,
            null, null, null, identificationNo, null, null);
        _mockUserQueries.Setup(uq => uq.GetUser(userId)).ReturnsAsync(user);

        // Mock api response.
        var externalProviderA = GetExternalAccountsForTradingAccountsApiResponse("ea_a", providerId: 0);
        var externalProviderB = GetExternalAccountsForTradingAccountsApiResponse("ea_b", providerId: 1);
        var externalProviderC = GetExternalAccountsForTradingAccountsApiResponse("ea_c", providerId: 2);

        var tradingAccountA = GetTradingAccountsForCustomerCodeApiResponse("ta_a",
            externalAccounts: [externalProviderA, externalProviderB]);
        var tradingAccountB = GetTradingAccountsForCustomerCodeApiResponse("ta_b",
            externalAccounts: [externalProviderA]);
        var tradingAccountC = GetTradingAccountsForCustomerCodeApiResponse("ta_c", externalAccounts: []);
        var tradingAccountD = GetTradingAccountsForCustomerCodeApiResponse("ta_d",
            externalAccounts: [externalProviderA, externalProviderB, externalProviderC]);

        var tradingAccountCustCodeA = GetTradingAccountsGroupedByCustomerCodeApiResponse(
            custCodeA, [tradingAccountA, tradingAccountB, tradingAccountC]);
        var tradingAccountCustCodeB = GetTradingAccountsGroupedByCustomerCodeApiResponse(custCodeB, [tradingAccountD]);

        List<PiOnboardServiceApplicationQueriesTradingAccountCustomerTradingAccountsByCustomerCode>
            tradingAccountsGroupedByCustCodeApiResp = [tradingAccountCustCodeA, tradingAccountCustCodeB];

        _mockOnboardTradingAccountService
            .Setup(otas =>
                otas.GetTradingAccountListGroupedByCustomerCodeByIdentificationNumberAsync(identificationNo,
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(tradingAccountsGroupedByCustCodeApiResp);

        // Expected for all customer codes.
        var expectedTradingAccountDetailsWithExternalAccountsA =
            GetUserTradingAccountInfoResponseFromOnboardTradingAccountApiResponse(tradingAccountCustCodeA);
        var expectedTradingAccountDetailsWithExternalAccountsB =
            GetUserTradingAccountInfoResponseFromOnboardTradingAccountApiResponse(tradingAccountCustCodeB);
        List<UserTradingAccountInfoWithExternalAccounts> expectedUserTradingAccountInfo =
        [
            expectedTradingAccountDetailsWithExternalAccountsA,
            expectedTradingAccountDetailsWithExternalAccountsB
        ];

        // Act
        var userTradingAccountInfo =
            await _userTradingAccountQueries.GetUserTradingAccountsWithExternalAccountsByUserId(
                userId, CancellationToken.None);

        // Assert
        userTradingAccountInfo.Should().BeEquivalentTo(expectedUserTradingAccountInfo);
    }

    [Fact]
    public async Task GetUserTradingAccountsWithExternalAccountsByUserId_FailedToGetUser_ShouldThrowError()
    {
        // Arrange
        var userId = Guid.NewGuid();
        string expectedErrorMessage = "Some error";

        _mockUserQueries.Setup(uq => uq.GetUser(It.IsAny<Guid>())).ThrowsAsync(new Exception(expectedErrorMessage));

        // Act and Assert
        var actualException = await Assert.ThrowsAsync<Exception>(() =>
            _userTradingAccountQueries.GetUserTradingAccountsWithExternalAccountsByUserId(
                userId, CancellationToken.None));
        Assert.Equal(expectedErrorMessage, actualException.Message);
    }

    [Fact]
    public async Task GetUserTradingAccountsWithExternalAccountsByUserId_UserCitizenIdNotFound_ShouldThrowError()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var user = new Pi.User.Application.Models.User(userId, [], [], [], null, null, null, null, null, null, null,
            null, null, null, null, null, null);
        _mockUserQueries.Setup(uq => uq.GetUser(userId)).ReturnsAsync(user);

        // Act and Assert
        var actualException = await Assert.ThrowsAsync<InvalidDataException>(() =>
            _userTradingAccountQueries.GetUserTradingAccountsWithExternalAccountsByUserId(
                userId, CancellationToken.None));
    }

    [Fact]
    public async Task
        GetUserTradingAccountsWithExternalAccountsByUserId_WhenFailedToGetTradingAccountsFromOnboardService_ShouldThrowError()
    {
        // Arrange
        var userId = Guid.NewGuid();
        string identificationNo = "0123456789012";
        string expectedErrorMessage = "Some error";

        var user = new Pi.User.Application.Models.User(userId, [], [], [], null, null, null, null, null, null, null,
            null, null, null, identificationNo, null, null);
        _mockUserQueries.Setup(uq => uq.GetUser(userId)).ReturnsAsync(user);

        _mockOnboardTradingAccountService
            .Setup(otas =>
                otas.GetTradingAccountListGroupedByCustomerCodeByIdentificationNumberAsync(identificationNo,
                    It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception(expectedErrorMessage));

        // Act and Assert
        var actualException = await Assert.ThrowsAsync<Exception>(() =>
            _userTradingAccountQueries.GetUserTradingAccountsWithExternalAccountsByUserId(
                userId, CancellationToken.None));
        Assert.Equal(expectedErrorMessage, actualException.Message);
    }

    [Fact]
    public async Task CheckHasPin_ReturnCustomerHasPinList_WhenDataIsValid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        const string identificationNo = "0123456789012";
        const string custCodeA = "custcode-a";
        const string custCodeB = "custcode-b";

        // Mock user queries.
        var user = new Pi.User.Application.Models.User(userId, [], [], [], null, null, null, null, null, null, null,
            null, null, null, identificationNo, null, null);
        _mockUserQueries.Setup(uq => uq.GetUser(userId)).ReturnsAsync(user);

        // Mock api response.
        var externalProviderA = GetExternalAccountsForTradingAccountsApiResponse("ea_a", providerId: 0);
        var externalProviderB = GetExternalAccountsForTradingAccountsApiResponse("ea_b", providerId: 1);
        var externalProviderC = GetExternalAccountsForTradingAccountsApiResponse("ea_c", providerId: 2);

        var tradingAccountA = GetTradingAccountsForCustomerCodeApiResponse("ta_a",
            externalAccounts: [externalProviderA, externalProviderB]);
        var tradingAccountB = GetTradingAccountsForCustomerCodeApiResponse("ta_b",
            externalAccounts: [externalProviderA]);
        var tradingAccountC = GetTradingAccountsForCustomerCodeApiResponse("ta_c", externalAccounts: []);
        var tradingAccountD = GetTradingAccountsForCustomerCodeApiResponse("ta_d",
            externalAccounts: [externalProviderA, externalProviderB, externalProviderC]);

        var tradingAccountCustCodeA = GetTradingAccountsGroupedByCustomerCodeApiResponse(
            custCodeA, [tradingAccountA, tradingAccountB, tradingAccountC]);
        var tradingAccountCustCodeB = GetTradingAccountsGroupedByCustomerCodeApiResponse(custCodeB, [tradingAccountD]);

        List<PiOnboardServiceApplicationQueriesTradingAccountCustomerTradingAccountsByCustomerCode>
            tradingAccountsGroupedByCustCodeApiResp = [tradingAccountCustCodeA, tradingAccountCustCodeB];

        _mockOnboardTradingAccountService
            .Setup(otas =>
                otas.GetTradingAccountListGroupedByCustomerCodeByIdentificationNumberAsync(identificationNo,
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(tradingAccountsGroupedByCustCodeApiResp);

        // Mock Sso
        _mockSsoService
            .Setup(sso => sso.CheckSyncedPin(custCodeA))
            .ReturnsAsync(true);
        _mockSsoService
            .Setup(sso => sso.CheckSyncedPin(custCodeB))
            .ReturnsAsync(false);

        List<CustomerCodeHasPin> expectedResult = [new CustomerCodeHasPin(custCodeA, true), new CustomerCodeHasPin(custCodeB, false)];

        //Act
        var result = await _userTradingAccountQueries.CheckHasPin(userId, CancellationToken.None);

        //Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task CheckHasPin_UserCitizenIdNotFound_ShouldThrowError()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var user = new Pi.User.Application.Models.User(userId, [], [], [], null, null, null, null, null, null, null,
            null, null, null, null, null, null);
        _mockUserQueries.Setup(uq => uq.GetUser(userId)).ReturnsAsync(user);

        // Act and Assert
        await Assert.ThrowsAsync<InvalidDataException>(() =>
            _userTradingAccountQueries.CheckHasPin(userId, CancellationToken.None));
    }

    private static PiOnboardServiceApplicationQueriesTradingAccountCustomerTradingAccountsByCustomerCode
        GetTradingAccountsGroupedByCustomerCodeApiResponse(string customerCode,
            List<
                    PiOnboardServiceApplicationQueriesTradingAccountCustomerTradingAccountsByCustomerCodeTradingAccountDetails>
                tradingAccounts)
    {
        return new PiOnboardServiceApplicationQueriesTradingAccountCustomerTradingAccountsByCustomerCode(customerCode,
            tradingAccounts);
    }

    private static
        PiOnboardServiceApplicationQueriesTradingAccountCustomerTradingAccountsByCustomerCodeTradingAccountDetails
        GetTradingAccountsForCustomerCodeApiResponse(string suffix, Guid? id = null, string? accountNo = null,
            string? accountType = null, string? accountTypeCode = null, string? exchangeMarketId = null,
            List<PiOnboardServiceApplicationQueriesTradingAccountCustomerTradingAccountsByCustomerCodeExternalAccounts>?
                externalAccounts = null)
    {
        Guid tradingAccountId = id ?? Guid.NewGuid();
        string tradingAccountNo = accountNo ?? $"account-no-{suffix}";
        string tradingAccountType = accountType ?? $"account-type-{suffix}";
        string tradingAccountTypeCode = accountTypeCode ?? $"account-type-code-{suffix}";
        string tradingAccountExchangeMarketId = exchangeMarketId ?? $"account-xchgmktid-{suffix}";
        List<PiOnboardServiceApplicationQueriesTradingAccountCustomerTradingAccountsByCustomerCodeExternalAccounts>
            tradingAccountExternalAccounts = externalAccounts ?? [];
        return new
            PiOnboardServiceApplicationQueriesTradingAccountCustomerTradingAccountsByCustomerCodeTradingAccountDetails(
                tradingAccountId, tradingAccountNo, tradingAccountType, tradingAccountTypeCode,
                tradingAccountExchangeMarketId, tradingAccountExternalAccounts);
    }

    private static PiOnboardServiceApplicationQueriesTradingAccountCustomerTradingAccountsByCustomerCodeExternalAccounts
        GetExternalAccountsForTradingAccountsApiResponse(string suffix, Guid? id = null, string? account = null,
            int? providerId = null)
    {
        var externalAccountId = id ?? Guid.NewGuid();
        string externalAccountName = account ?? $"account-{suffix}";
        int externalAccountProviderId = providerId ?? 0;
        return new
            PiOnboardServiceApplicationQueriesTradingAccountCustomerTradingAccountsByCustomerCodeExternalAccounts(
                externalAccountId, externalAccountName, externalAccountProviderId);
    }

    private static UserTradingAccountInfoWithExternalAccounts
        GetUserTradingAccountInfoResponseFromOnboardTradingAccountApiResponse(
            PiOnboardServiceApplicationQueriesTradingAccountCustomerTradingAccountsByCustomerCode
                onboardTradingAccountApiResponse)
    {
        return new UserTradingAccountInfoWithExternalAccounts(onboardTradingAccountApiResponse.CustCode,
            onboardTradingAccountApiResponse.TradingAccounts
                .Select(x =>
                    GetTradingAccountDetailsResponseFromOnboardTradingAccountApiResponse(x))
                .ToList()
        );
    }

    private static TradingAccountDetailsWithExternalAccounts
        GetTradingAccountDetailsResponseFromOnboardTradingAccountApiResponse(
            PiOnboardServiceApplicationQueriesTradingAccountCustomerTradingAccountsByCustomerCodeTradingAccountDetails
                onboardTradingAccountApiResponse)
    {
        return new TradingAccountDetailsWithExternalAccounts(
            onboardTradingAccountApiResponse.TradingAccountId,
            onboardTradingAccountApiResponse.TradingAccountNo,
            onboardTradingAccountApiResponse.AccountType,
            onboardTradingAccountApiResponse.AccountTypeCode,
            onboardTradingAccountApiResponse.ExchangeMarketId,
            GetProductNameFromAccountTypeCode(onboardTradingAccountApiResponse.AccountTypeCode),
            onboardTradingAccountApiResponse.ExternalAccounts
                .Select(x =>
                    new ExternalAccountDetails(x.Id, x.Account, x.ProviderId)).ToList()
        );
    }

    private static ProductName GetProductNameFromAccountTypeCode(string accountTypeCode)
    {
        switch (accountTypeCode)
        {
            case "BB":
                return ProductName.CreditBalanceSbl;
            case "CT":
                return ProductName.Crypto;
            case "LC":
                return ProductName.CashSbl;
            case "CC":
                return ProductName.Cash;
            case "XU":
                return ProductName.GlobalEquities;
            case "DC":
                return ProductName.Bond;
            case "CB":
                return ProductName.CreditBalance;
            case "CH":
                return ProductName.CashBalance;
            case "TF":
                return ProductName.Derivatives;
            case "UT":
                return ProductName.Funds;
            case "LH":
                return ProductName.CashBalanceSbl;
            default:
                return ProductName.Unknown;
        }
    }
}