// using Moq;
// using Pi.Client.FundService.Api;
// using Pi.Client.FundService.Model;
// using Pi.Common.ExtensionMethods;
// using Pi.PortfolioService.Application.Services.Models.Portfolio;
// using Pi.PortfolioService.Infrastructure.Services;
//
// namespace Pi.PortfolioService.Infrastructure.Tests.Services;
//
// public class FundServiceTest
// {
//     private readonly Mock<IFundTradingApi> _fundTradingApi;
//     private readonly FundService _fundService;
//
//     public FundServiceTest()
//     {
//         _fundTradingApi = new Mock<IFundTradingApi>();
//         _fundService = new FundService(_fundTradingApi.Object);
//     }
//
//     [Fact]
//     public async Task GetPortfolioAccounts_ShouldReturnExpectedPortfolioAccounts()
//     {
//         // Arrange
//         var assets = new List<PiFinancialFundServiceAPIModelsInternalFundAssetResponse>()
//         {
//             FakeAsset("KF-OIL", "779911", "779911-M", marketValue: 100, costValue: 80),
//             FakeAsset("KF-CHINA", "779911", "779911-M", marketValue: 100, costValue: 120),
//             FakeAsset("KF-INDIA", "779911", "779911-1", marketValue: 100, costValue: 80),
//             FakeAsset("KF-OIL", "879911", "879911-M", marketValue: 100, costValue: 140),
//             FakeAsset("KF-OIL", "", "879912-M", marketValue: 300, costValue: 270),
//         };
//         _fundTradingApi.Setup(q => q.InternalAccountsAssetsGetAsync(It.IsAny<Guid>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
//             .ReturnsAsync(new PiFinancialFundServiceAPIModelsInternalFundAssetResponseListApiResponse() { Data = assets });
//
//         // Act
//         var actual = await _fundService.GetPortfolioAccounts(Guid.NewGuid());
//
//         // Assert
//         Assert.Equal(4, actual.Count);
//         Assert.True(actual.TrueForAll(q => q.AccountType == PortfolioAccountType.MutualFund.GetEnumDescription()));
//         Assert.True(actual.TrueForAll(q => !q.SblFlag));
//         Assert.True(actual.TrueForAll(q => q.CashBalance == 0m));
//         Assert.True(actual.TrueForAll(q => q.AccountId == ""));
//         Assert.Equal(new[] { "779911-M", "779911-1", "879911-M", "879912-M" }, actual.Select(q => q.AccountNoForDisplay));
//         Assert.Equal(new[] { "779911", "879911", "879912" }, actual.Select(q => q.CustCode).Distinct());
//         Assert.Equal(new[] { 200m, 100m, 100m, 300m }, actual.Select(q => q.TotalMarketValue));
//         Assert.Equal(new[] { 0m, 20m, -40m, 30m }, actual.Select(q => q.Upnl));
//     }
//
//     [Fact]
//     public async Task GetPortfolioAccounts_ShouldReturnEmptyPortfolioAccounts_When_ApiResponseEmpty()
//     {
//         // Arrange
//         _fundTradingApi.Setup(q => q.InternalAccountsAssetsGetAsync(It.IsAny<Guid>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
//             .ReturnsAsync(new PiFinancialFundServiceAPIModelsInternalFundAssetResponseListApiResponse(new List<PiFinancialFundServiceAPIModelsInternalFundAssetResponse>()));
//
//         // Act
//         var actual = await _fundService.GetPortfolioAccounts(Guid.NewGuid());
//
//         // Assert
//         Assert.Empty(actual);
//     }
//
//     [Fact]
//     public async Task GetPortfolioAccounts_ShouldReturnEmptyPortfolioAccounts_When_ApiError()
//     {
//         // Arrange
//         _fundTradingApi.Setup(q =>
//                 q.InternalAccountsAssetsGetAsync(It.IsAny<Guid>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
//             .ThrowsAsync(new Exception());
//
//         // Act
//         var actual = await _fundService.GetPortfolioAccounts(Guid.NewGuid());
//
//         // Assert
//         Assert.Empty(actual);
//     }
//
//     private static PiFinancialFundServiceAPIModelsInternalFundAssetResponse FakeAsset(
//         string fundCode,
//         string custCode,
//         string tradingAccountNo,
//         string unitHolderId = "18731",
//         DateTime asOfDate = default,
//         double marketValue = 0.0,
//         double costValue = 0.0)
//     {
//         return new PiFinancialFundServiceAPIModelsInternalFundAssetResponse(
//                 asOfDate,
//                 fundCode,
//                 tradingAccountNo,
//                 unitHolderId,
//                 custCode: custCode,
//                 marketValue: marketValue,
//                 costValue: costValue
//             );
//     }
// }
