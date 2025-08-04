using Microsoft.Extensions.Logging;
using Moq;
using Pi.Client.WalletService.Api;
using Pi.Client.WalletService.Model;
using Pi.TfexService.Infrastructure.Services;

namespace Pi.TfexService.Infrastructure.Tests.Services;

public class WalletServiceTests
{
    private readonly Mock<IWalletApiAsync> _walletApi;
    private readonly Mock<ILogger<WalletService>> _logger;
    private readonly WalletService _walletService;

    public WalletServiceTests()
    {
        _walletApi = new Mock<IWalletApiAsync>();
        _logger = new Mock<ILogger<WalletService>>();

        _walletService = new WalletService(_walletApi.Object, _logger.Object);
    }

    [Fact]
    public async Task GetWalletBalance_Should_Return_Valid_Balance()
    {
        // Arrange
        var userId = "testUserId";
        var accountCode = "12345678";
        var expectedBalance = 1000.00m;
        var cancellationToken = new CancellationToken();

        _walletApi.Setup(x => x.InternalWalletProductWithdrawBalanceGetAsync(
                It.IsAny<string>(),
                It.IsAny<PiWalletServiceIntegrationEventsAggregatesModelProduct>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PiWalletServiceAPIModelsAvailableBalanceApiResponse() { Data = new PiWalletServiceAPIModelsAvailableBalance { Amount = expectedBalance } });

        // Act
        var result = await _walletService.GetWalletBalance(userId, accountCode, cancellationToken);

        // Assert
        Assert.Equal(expectedBalance, result);
    }

    [Fact]
    public async Task GetWalletBalance_Should_Throw_ArgumentException_When_AccountCode_Is_Invalid()
    {
        // Arrange
        var userId = "testUserId";
        var accountCode = "123"; // Invalid account code
        var cancellationToken = new CancellationToken();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _walletService.GetWalletBalance(userId, accountCode, cancellationToken));
    }
}