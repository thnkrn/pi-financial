using FluentAssertions;
using Pi.Client.OnboardService.Api;
using Microsoft.Extensions.Logging;
using Moq;
using Pi.User.Infrastructure.Services;
using Pi.Client.OnboardService.Model;

namespace Pi.User.Infrastructure.Tests.Services;

public class OnboardTradingAccountServiceTests
{
    private readonly OnboardTradingAccountService _onboardTradingAccountService;
    private readonly Mock<ITradingAccountApi> _mockTradingAccountApi = new();

    public OnboardTradingAccountServiceTests()
    {
        _onboardTradingAccountService = new OnboardTradingAccountService(_mockTradingAccountApi.Object);
    }

    [Fact]
    public async Task GetTradingAccountListByCustomerCodeAsync_WhenTradingAccountsExistsForCustCode_ShouldReturnList()
    {
        // Arrange
        string customerCode = "custcode-a";
        bool withBankAccounts = false;
        bool withExternalAccounts = true;

        List<PiOnboardServiceApplicationQueriesTradingAccountCustomerTradingAccount> expectedTradingAccountList =
        [
            new PiOnboardServiceApplicationQueriesTradingAccountCustomerTradingAccount(
                Guid.NewGuid(), "acct-no-a", customerCode, "acct-type-a", "acct-type-code-a"),
            new PiOnboardServiceApplicationQueriesTradingAccountCustomerTradingAccount(
                Guid.NewGuid(), "acct-no-b", customerCode, "acct-type-b", "acct-type-code-b")
        ];

        var apiResponse =
            new PiOnboardServiceApplicationQueriesTradingAccountCustomerTradingAccountListApiResponse(
                expectedTradingAccountList);
        _mockTradingAccountApi
            .Setup(taa =>
                taa.InternalGetTradingAccountListByCustomerCodeV2Async(
                    customerCode, withBankAccounts, withExternalAccounts, It.IsAny<CancellationToken>()))
            .ReturnsAsync(apiResponse);

        // Act
        var actualTradingAccountList = await _onboardTradingAccountService.GetTradingAccountListByCustomerCodeAsync(
            customerCode, withBankAccounts, withExternalAccounts, CancellationToken.None);

        // Assert
        actualTradingAccountList.Should().BeEquivalentTo(expectedTradingAccountList);
    }

    [Fact]
    public async Task GetTradingAccountListByCustomerCodeAsync_WhenFailedToGetTradingAccounts_ShouldThrowError()
    {
        // Arrange
        string customerCode = "custcode-a";
        bool withBankAccounts = false;
        bool withExternalAccounts = true;
        string expectedErrorMessage = "Some error";

        _mockTradingAccountApi
            .Setup(taa =>
                taa.InternalGetTradingAccountListByCustomerCodeV2Async(
                    customerCode, withBankAccounts, withExternalAccounts, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception(expectedErrorMessage));

        // Act and Assert
        var actualException = await Assert.ThrowsAsync<Exception>(() => _onboardTradingAccountService.GetTradingAccountListByCustomerCodeAsync(
            customerCode, withBankAccounts, withExternalAccounts, CancellationToken.None));
        Assert.Equal(expectedErrorMessage, actualException.Message);
    }
}