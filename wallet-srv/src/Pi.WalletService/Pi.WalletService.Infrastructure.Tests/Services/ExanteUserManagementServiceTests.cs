using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Pi.Client.ExanteUserManagement.Api;
using Pi.Client.ExanteUserManagement.Client;
using Pi.Client.ExanteUserManagement.Model;
using Pi.WalletService.Application.Commands.GlobalWalletTransfer;
using Pi.WalletService.Infrastructure.Models;
using Pi.WalletService.Infrastructure.Services;
namespace Pi.WalletService.Infrastructure.Tests.Services;

public class ExanteUserManagementServiceTests
{
    // private readonly Mock<IExanteUserManagementApi> _exanteUserManagementApi;
    // private readonly Mock<IMemoryCache> _memoryCache;
    // private readonly ExanteUserManagementService _exanteUserManagementService;
    //
    // private const string Token = "TOKEN";
    //
    // public ExanteUserManagementServiceTests()
    // {
    //     _memoryCache = new Mock<IMemoryCache>();
    //     _exanteUserManagementApi = new Mock<IExanteUserManagementApi>();
    //
    //     var inMemorySettings = new Dictionary<string, string>
    //     {
    //         { "Exante:UserManagementUsername", "Username" },
    //         { "Exante:UserManagementPassword", "Password" },
    //     };
    //
    //     var configuration = new ConfigurationBuilder()
    //         .AddInMemoryCollection(inMemorySettings!)
    //         .Build();
    //
    //     _exanteUserManagementService = new ExanteUserManagementService(_exanteUserManagementApi.Object, configuration, _memoryCache);
    //
    //     _exanteUserManagementApi
    //         .Setup(api => api.Configuration.DefaultHeaders)
    //         .Returns(new Dictionary<string, string>());
    //
    //     _exanteUserManagementApi
    //         .Setup(
    //             api => api.ApiUserAuthPostAsync(
    //                 It.IsAny<LoginRequest>(),
    //                 It.IsAny<CancellationToken>()
    //             )
    //         )
    //         .ReturnsAsync(new LoginResponse(Token));
    // }
    //
    // [Fact]
    // public async void ExanteUserManagementService_Should_Use_Auth_Token_From_Cache_If_Exist()
    // {
    //     // Arrange
    //     var response = new TransferResponse(accountId: "Source", secondAccount: "Target", "Asset", 100, "SEQ");
    //     _exanteUserManagementApi
    //         .Setup(
    //             api => api.EnClientsareaAccountWithdrawalCreatePostAsync(
    //                 It.IsAny<TransferRequest>(),
    //                 It.IsAny<CancellationToken>()
    //             )
    //         )
    //         .ReturnsAsync(response);
    //
    //     // Act
    //     _memoryCache.Set(CacheKeys.ExanteUserManagementAuthToken, Token);
    //     var x = _memoryCache.Get(CacheKeys.ExanteUserManagementAuthToken);
    //     var result = await _exanteUserManagementService.TransferMoney("Source", "Target", "USD", 100);
    //     _memoryCache.Remove(CacheKeys.ExanteUserManagementAuthToken);
    //
    //     // Assert
    //     Assert.Equal(response.AccountId, result.SourceAccountId);
    //     Assert.Equal(response.SecondAccount, result.TargetAccountId);
    //     Assert.Equal(response.Asset, result.Asset);
    //     Assert.Equal(response.Amount, result.Amount);
    //     Assert.Equal(response.SequenceId, result.SequenceId);
    //     _exanteUserManagementApi.Verify(
    //         api => api.ApiUserAuthPostAsync(
    //             It.IsAny<LoginRequest>(),
    //             It.IsAny<CancellationToken>()
    //         ),
    //         Times.Never
    //     );
    // }
    //
    // [Fact]
    // public async void ExanteUserManagementService_Should_Throw_TransferInsufficientBalanceException_If_Error_Is_Amount_Related()
    // {
    //     // Arrange
    //     var response = new TransferResponse(accountId: "Source", secondAccount: "Target", "Asset", 100, "SEQ");
    //     _exanteUserManagementApi
    //         .Setup(
    //             api => api.EnClientsareaAccountWithdrawalCreatePostAsync(
    //                 It.IsAny<TransferRequest>(),
    //                 It.IsAny<CancellationToken>()
    //             )
    //         )
    //         .ThrowsAsync(new ApiException(400, "No money", "{\"success\":false,\"errors\":{\"amount\":\"Transfer has not been completed.\"}}"));
    //
    //     // Act
    //     var action = () => _exanteUserManagementService.TransferMoney("Source", "Target", "USD", 100);
    //
    //     // Assert
    //     await Assert.ThrowsAsync<TransferInsufficientBalanceException>(action);
    // }
    //
    // [Fact]
    // public async void ExanteUserManagementService_Should_Throw_Exception_When_Token_From_Login_Is_Invalid()
    // {
    //     // Arrange
    //     _exanteUserManagementApi
    //         .Setup(
    //             api => api.ApiUserAuthPostAsync(
    //                 It.IsAny<LoginRequest>(),
    //                 It.IsAny<CancellationToken>()
    //             )
    //         )
    //         .ReturnsAsync(new LoginResponse(""));
    //
    //     // Act
    //     var action = () => _exanteUserManagementService.TransferMoney("Source", "Target", "USD", 100);
    //
    //     // Assert
    //     await Assert.ThrowsAsync<ArgumentNullException>(action);
    // }
    //
    // [Fact]
    // public async void ExanteUserManagementService_Should_Clear_Auth_Cache_And_Auth_Again_When_Api_Returned_401()
    // {
    //     // Arrange
    //     _exanteUserManagementApi
    //         .Setup(
    //             api => api.EnClientsareaAccountWithdrawalCreatePostAsync(
    //                 It.IsAny<TransferRequest>(),
    //                 It.IsAny<CancellationToken>()
    //             )
    //         )
    //         .Throws(new ApiException(401, "Unauthorized"));
    //
    //     // Act
    //     var action = () => _exanteUserManagementService.TransferMoney("Source", "Target", "USD", 100);
    //
    //     // Assert
    //     await Assert.ThrowsAsync<ApiException>(action);
    //     _exanteUserManagementApi.Verify(
    //         api => api.ApiUserAuthPostAsync(It.IsAny<LoginRequest>(), It.IsAny<CancellationToken>()),
    //         Times.Exactly(2)
    //     );
    //     _exanteUserManagementApi.Verify(
    //         api => api.ApiUserAuthPostAsync(
    //             It.IsAny<LoginRequest>(),
    //             It.IsAny<CancellationToken>()
    //         ),
    //         Times.Exactly(2)
    //     );
    // }
}