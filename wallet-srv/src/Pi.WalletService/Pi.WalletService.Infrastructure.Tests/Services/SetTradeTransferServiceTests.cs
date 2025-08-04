using System.Net;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Pi.Financial.Client.Settrade.Api;
using Pi.Financial.Client.Settrade.Client;
using Pi.Financial.Client.Settrade.Model;
using Pi.WalletService.Domain.Exceptions;
using Pi.WalletService.Infrastructure.Models;
using Pi.WalletService.Infrastructure.Options;
using Pi.WalletService.Infrastructure.Services;

namespace Pi.WalletService.Infrastructure.Tests.Services;

public class SetTradeTransferServiceTests
{
    private readonly Mock<ISetTradeApi> _setTradeApi;
    private readonly IDistributedCache _cache;
    private readonly Mock<ILogger<SetTradeTransferService>> _logger;
    private readonly Mock<IOptionsSnapshot<SetTradeOptions>> _options;
    private readonly SetTradeTransferService _setTradeTransferService;

    public SetTradeTransferServiceTests()
    {
        _setTradeApi = new Mock<ISetTradeApi>();
        _logger = new Mock<ILogger<SetTradeTransferService>>();
        _options = new Mock<IOptionsSnapshot<SetTradeOptions>>();
        var cacheOptions = new OptionsWrapper<MemoryDistributedCacheOptions>(new MemoryDistributedCacheOptions());
        _cache = new MemoryDistributedCache(cacheOptions);
        var options = new SetTradeOptions()
        {
            ApiKey = "MOCK_API_KEY",
            Application = "MOCK_APPLICATION",
            AppSecret = "TU9DS19BUFBfU0VDUkVU",
            BrokerId = "MOCK_BROKER_ID",
            Host = "MOCK_HOST",
            TimeoutMs = 5000
        };

        _options
            .Setup(o => o.Value)
            .Returns(options);

        _setTradeTransferService = new SetTradeTransferService(
            _setTradeApi.Object,
            _options.Object,
            _cache,
            _logger.Object
        );
    }

    [Fact]
    public async void GetCurrentAccessToken_Should_Return_AccessToken_From_Cache_If_Exists()
    {
        // Arrange
        var mockAccessToken = "MOCK_ACCESS_TOKEN";
        await _cache.SetStringAsync(CacheKeys.SetTradeAccessToken, mockAccessToken);

        // Act
        var result = await _setTradeTransferService.GetAccessToken();

        // Assert
        Assert.Equal("MOCK_ACCESS_TOKEN", result);
    }

    [Fact]
    public async void GetCurrentAccessToken_Should_Return_AccessToken_From_API()
    {
        // Arrange
        await _cache.RemoveAsync(CacheKeys.SetTradeAccessToken);

        var authResponse = new SettradeAuthResponse
        {
            AccessToken = "MOCK_ACCESS_TOKEN",
            AuthenticatedUserid = "MOCK_USER_ID",
            BrokerId = "MOCK_BROKER_ID",
            ExpiresIn = 3600,
            RefreshToken = "MOCK_REFRESH_TOKEN",
            TokenType = "Bearer"
        };
        _setTradeApi
            .Setup(
                api => api.AuthLoginAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<SettradeAuthRequest>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(authResponse);

        // Act
        var result = await _setTradeTransferService.GetAccessToken();

        // Assert
        Assert.Equal("MOCK_ACCESS_TOKEN", result);
    }

    [Fact]
    public async void RenewCurrentAccessToken_Should_Generate_New_AccessToken()
    {
        // Arrange
        await _cache.RemoveAsync(CacheKeys.SetTradeAccessToken);

        var authResponse = new SettradeAuthResponse
        {
            AccessToken = "MOCK_ACCESS_TOKEN",
            AuthenticatedUserid = "MOCK_USER_ID",
            BrokerId = "MOCK_BROKER_ID",
            ExpiresIn = 3600,
            RefreshToken = "MOCK_REFRESH_TOKEN",
            TokenType = "Bearer"
        };
        _setTradeApi
            .Setup(
                api => api.AuthLoginAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<SettradeAuthRequest>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(authResponse);

        // Act
        var result = await _setTradeTransferService.GenerateAccessToken();

        // Assert
        Assert.Equal("MOCK_ACCESS_TOKEN", result);
    }

    [Fact]
    public async void CashDeposit_Should_Return_Error_When_API_Throw_Exception()
    {
        // Arrange
        var userId = "MOCK_USER_ID";
        var transactionId = "MOCK_TRANSACTION_ID";
        var accountNo = "MOCK_ACCOUNT_NO";
        var amount = 100.7676m;
        var roundAmount = 100.77m;
        await _cache.RemoveAsync(CacheKeys.SetTradeAccessToken);

        var authResponse = new SettradeAuthResponse
        {
            AccessToken = "MOCK_ACCESS_TOKEN",
            AuthenticatedUserid = "MOCK_USER_ID",
            BrokerId = "MOCK_BROKER_ID",
            ExpiresIn = 3600,
            RefreshToken = "MOCK_REFRESH_TOKEN",
            TokenType = "Bearer"
        };
        _setTradeApi
            .Setup(
                api => api.AuthLoginAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<SettradeAuthRequest>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(authResponse);

        var depositRequest = new CashDepositRequest(roundAmount);
        _setTradeApi
            .Setup(
                api => api.DepositCashWithHttpInfoAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    $"Bearer {authResponse.AccessToken}",
                    depositRequest,
                    It.IsAny<CancellationToken>()
                )
            )
            .ThrowsAsync(new Exception("DEPOSIT_ERROR"));

        // Act
        var action = () => _setTradeTransferService.CashDeposit(userId, transactionId, accountNo, amount);

        // Assert
        await Assert.ThrowsAsync<SetTradeDepositException>(action);
    }

    [Fact]
    public async void CashDeposit_Should_Failed_When_API_Return_Fail_Response()
    {
        // Arrange
        var userId = "MOCK_USER_ID";
        var transactionId = "MOCK_TRANSACTION_ID";
        var accountNo = "MOCK_ACCOUNT_NO";
        var amount = 100.3210m;
        var roundAmount = 100.32m;
        await _cache.RemoveAsync(CacheKeys.SetTradeAccessToken);

        var authResponse = new SettradeAuthResponse
        {
            AccessToken = "MOCK_ACCESS_TOKEN",
            AuthenticatedUserid = "MOCK_USER_ID",
            BrokerId = "MOCK_BROKER_ID",
            ExpiresIn = 3600,
            RefreshToken = "MOCK_REFRESH_TOKEN",
            TokenType = "Bearer"
        };
        _setTradeApi
            .Setup(
                api => api.AuthLoginAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<SettradeAuthRequest>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(authResponse);

        var depositRequest = new CashDepositRequest(roundAmount);
        _setTradeApi
            .Setup(
                api => api.DepositCashWithHttpInfoAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    $"Bearer {authResponse.AccessToken}",
                    depositRequest,
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(new ApiResponse<object>(HttpStatusCode.BadRequest, new
            {
                code = "FAIL",
                message = "DEPOSIT_FAIL"
            }));

        // Act
        var response = await _setTradeTransferService.CashDeposit(userId, transactionId, accountNo, amount);

        // Assert
        Assert.False(response.Success);
        Assert.Equal("BadRequest", response.ErrorCode);
    }

    [Fact]
    public async void CashDeposit_Should_Success()
    {
        // Arrange
        var userId = "MOCK_USER_ID";
        var transactionId = "MOCK_TRANSACTION_ID";
        var accountNo = "MOCK_ACCOUNT_NO";
        var amount = 100.3210m;
        var roundAmount = 100.32m;
        await _cache.RemoveAsync(CacheKeys.SetTradeAccessToken);

        var authResponse = new SettradeAuthResponse
        {
            AccessToken = "MOCK_ACCESS_TOKEN",
            AuthenticatedUserid = "MOCK_USER_ID",
            BrokerId = "MOCK_BROKER_ID",
            ExpiresIn = 3600,
            RefreshToken = "MOCK_REFRESH_TOKEN",
            TokenType = "Bearer"
        };
        _setTradeApi
            .Setup(
                api => api.AuthLoginAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<SettradeAuthRequest>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(authResponse);

        var depositRequest = new CashDepositRequest(roundAmount);
        _setTradeApi
            .Setup(
                api => api.DepositCashWithHttpInfoAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    $"Bearer {authResponse.AccessToken}",
                    depositRequest,
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(new ApiResponse<object>(HttpStatusCode.OK, new
            {
            }));

        // Act
        var response = await _setTradeTransferService.CashDeposit(userId, transactionId, accountNo, amount);

        // Assert
        Assert.True(response.Success);
    }

    [Fact]
    public async void CashWithdraw_Should_Failed_When_API_Failed()
    {
        // Arrange
        var userId = "MOCK_USER_ID";
        var transactionId = "MOCK_TRANSACTION_ID";
        var accountNo = "MOCK_ACCOUNT_NO";
        var amount = 100.3210m;
        await _cache.RemoveAsync(CacheKeys.SetTradeAccessToken);

        var authResponse = new SettradeAuthResponse
        {
            AccessToken = "MOCK_ACCESS_TOKEN",
            AuthenticatedUserid = "MOCK_USER_ID",
            BrokerId = "MOCK_BROKER_ID",
            ExpiresIn = 3600,
            RefreshToken = "MOCK_REFRESH_TOKEN",
            TokenType = "Bearer"
        };
        _setTradeApi
            .Setup(
                api => api.AuthLoginAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<SettradeAuthRequest>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(authResponse);

        _setTradeApi
            .Setup(
                api => api.GetAccountInfoAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    $"Bearer {authResponse.AccessToken}",
                    It.IsAny<CancellationToken>()
                )
            )
            .ThrowsAsync(new Exception("GET_ACCOUNT_INFO_ERROR"));

        // Act
        var action = () => _setTradeTransferService.CashWithdraw(userId, transactionId, accountNo, amount);

        // Assert
        await Assert.ThrowsAsync<SetTradeWithdrawException>(action);
    }

    [Fact]
    public async void CashWithdraw_Should_Failed_When_ExcessEquity_Less_Then_Withdraw_Amount()
    {
        // Arrange
        var userId = "MOCK_USER_ID";
        var transactionId = "MOCK_TRANSACTION_ID";
        var accountNo = "MOCK_ACCOUNT_NO";
        var amount = 100.3210m;
        await _cache.RemoveAsync(CacheKeys.SetTradeAccessToken);

        var authResponse = new SettradeAuthResponse
        {
            AccessToken = "MOCK_ACCESS_TOKEN",
            AuthenticatedUserid = "MOCK_USER_ID",
            BrokerId = "MOCK_BROKER_ID",
            ExpiresIn = 3600,
            RefreshToken = "MOCK_REFRESH_TOKEN",
            TokenType = "Bearer"
        };
        _setTradeApi
            .Setup(
                api => api.AuthLoginAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<SettradeAuthRequest>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(authResponse);

        var accountInfoResponse = new AccountInfoResponse()
        {
            CashBalance = 10.00m,
            CreditLine = 10.00m,
            ExcessEquity = 10.00m,
            DepositWithdrawal = 10.00m,
            Equity = 10.00m,
            LiquidationValue = 10.00m
        };

        _setTradeApi
            .Setup(
                api => api.GetAccountInfoAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    $"Bearer {authResponse.AccessToken}",
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(accountInfoResponse);

        // Act
        var response = await _setTradeTransferService.CashWithdraw(userId, transactionId, accountNo, amount);

        // Assert
        Assert.False(response.Success);
        Assert.Equal($"SetTrade account cash balance is less than the requested amount. Cash balance: {accountInfoResponse.ExcessEquity}", response.ErrorMessage);
    }

    [Fact]
    public async void CashWithdraw_Should_Failed_When_API_Return_Not_Success()
    {
        // Arrange
        var userId = "MOCK_USER_ID";
        var transactionId = "MOCK_TRANSACTION_ID";
        var accountNo = "MOCK_ACCOUNT_NO";
        var amount = 100.3210m;
        var roundAmount = 100.32m;
        await _cache.RemoveAsync(CacheKeys.SetTradeAccessToken);

        var authResponse = new SettradeAuthResponse
        {
            AccessToken = "MOCK_ACCESS_TOKEN",
            AuthenticatedUserid = "MOCK_USER_ID",
            BrokerId = "MOCK_BROKER_ID",
            ExpiresIn = 3600,
            RefreshToken = "MOCK_REFRESH_TOKEN",
            TokenType = "Bearer"
        };
        _setTradeApi
            .Setup(
                api => api.AuthLoginAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<SettradeAuthRequest>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(authResponse);

        var accountInfoResponse = new AccountInfoResponse()
        {
            CashBalance = 10.00m,
            CreditLine = 10.00m,
            ExcessEquity = 1000.00m,
            DepositWithdrawal = 10.00m,
            Equity = 10.00m,
            LiquidationValue = 10.00m
        };

        _setTradeApi
            .Setup(
                api => api.GetAccountInfoAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    $"Bearer {authResponse.AccessToken}",
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(accountInfoResponse);

        var cashWithdrawRequest = new CashWithdrawalRequest(roundAmount);


        _setTradeApi
            .Setup(
                api => api.WithdrawCashWithHttpInfoAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    $"Bearer {authResponse.AccessToken}",
                    cashWithdrawRequest,
                    It.IsAny<CancellationToken>()
                ))
            .ReturnsAsync(new ApiResponse<object>(HttpStatusCode.BadRequest, new
            {
                code = "FAIL",
                message = "WITHDRAW_FAIL"
            }));

        // Act
        var response = await _setTradeTransferService.CashWithdraw(userId, transactionId, accountNo, amount);

        // Assert
        Assert.False(response.Success);
        Assert.Equal("BadRequest", response.ErrorCode);
    }

    [Fact]
    public async void CashWithdraw_Should_Success()
    {
        // Arrange
        var userId = "MOCK_USER_ID";
        var transactionId = "MOCK_TRANSACTION_ID";
        var accountNo = "MOCK_ACCOUNT_NO";
        var amount = 100.3210m;
        var roundAmount = 100.32m;
        await _cache.RemoveAsync(CacheKeys.SetTradeAccessToken);

        var authResponse = new SettradeAuthResponse
        {
            AccessToken = "MOCK_ACCESS_TOKEN",
            AuthenticatedUserid = "MOCK_USER_ID",
            BrokerId = "MOCK_BROKER_ID",
            ExpiresIn = 3600,
            RefreshToken = "MOCK_REFRESH_TOKEN",
            TokenType = "Bearer"
        };
        _setTradeApi
            .Setup(
                api => api.AuthLoginAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<SettradeAuthRequest>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(authResponse);

        var accountInfoResponse = new AccountInfoResponse()
        {
            CashBalance = 10.00m,
            CreditLine = 10.00m,
            ExcessEquity = 1000.00m,
            DepositWithdrawal = 10.00m,
            Equity = 10.00m,
            LiquidationValue = 10.00m
        };

        _setTradeApi
            .Setup(
                api => api.GetAccountInfoAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    $"Bearer {authResponse.AccessToken}",
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(accountInfoResponse);

        var cashWithdrawRequest = new CashWithdrawalRequest(roundAmount);


        _setTradeApi
            .Setup(
                api => api.WithdrawCashWithHttpInfoAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    $"Bearer {authResponse.AccessToken}",
                    cashWithdrawRequest,
                    It.IsAny<CancellationToken>()
                ))
            .ReturnsAsync(new ApiResponse<object>(HttpStatusCode.OK, new
            {
            }));

        // Act
        var response = await _setTradeTransferService.CashWithdraw(userId, transactionId, accountNo, amount);

        // Assert
        Assert.True(response.Success);
    }

    [Fact]
    public async void GetWithdrawalBalance_Should_Failed_When_API_Throw_Exception()
    {
        // Arrange
        var accountNo = "MOCK_ACCOUNT_NO";
        await _cache.RemoveAsync(CacheKeys.SetTradeAccessToken);

        var authResponse = new SettradeAuthResponse
        {
            AccessToken = "MOCK_ACCESS_TOKEN",
            AuthenticatedUserid = "MOCK_USER_ID",
            BrokerId = "MOCK_BROKER_ID",
            ExpiresIn = 3600,
            RefreshToken = "MOCK_REFRESH_TOKEN",
            TokenType = "Bearer"
        };

        _setTradeApi
            .Setup(
                api => api.AuthLoginAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<SettradeAuthRequest>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(authResponse);

        _setTradeApi
            .Setup(
                api => api.GetAccountInfoAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    $"Bearer {authResponse.AccessToken}",
                    It.IsAny<CancellationToken>()
                )
            )
            .ThrowsAsync(new Exception("GET_ACCOUNT_INFO_ERROR"));

        // Act
        var action = () => _setTradeTransferService.GetWithdrawalBalance(accountNo);

        // Assert
        await Assert.ThrowsAsync<SetTradeAccountInfoException>(action);
    }

    [Fact]
    public async void GetWithdrawalBalance_Should_Success()
    {
        // Arrange
        var accountNo = "MOCK_ACCOUNT_NO";
        await _cache.RemoveAsync(CacheKeys.SetTradeAccessToken);

        var authResponse = new SettradeAuthResponse
        {
            AccessToken = "MOCK_ACCESS_TOKEN",
            AuthenticatedUserid = "MOCK_USER_ID",
            BrokerId = "MOCK_BROKER_ID",
            ExpiresIn = 3600,
            RefreshToken = "MOCK_REFRESH_TOKEN",
            TokenType = "Bearer"
        };

        _setTradeApi
            .Setup(
                api => api.AuthLoginAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<SettradeAuthRequest>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(authResponse);

        var accountInfoResponse = new AccountInfoResponse()
        {
            CashBalance = 10.00m,
            CreditLine = 10.00m,
            ExcessEquity = 1000.00m,
            DepositWithdrawal = 10.00m,
            Equity = 10.00m,
            LiquidationValue = 10.00m
        };

        _setTradeApi
            .Setup(
                api => api.GetAccountInfoAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    $"Bearer {authResponse.AccessToken}",
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(accountInfoResponse);

        // Act
        var resp = await _setTradeTransferService.GetWithdrawalBalance(accountNo);

        // Assert
        Assert.Equal(1000.00m, resp);
    }
}
