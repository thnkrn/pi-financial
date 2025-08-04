// using Microsoft.Extensions.Logging.Abstractions;
// using Moq;
// using Pi.Common.ExtensionMethods;
// using Pi.Common.Features;
// using Pi.PortfolioService.Application.Models;
// using Pi.PortfolioService.Application.Services.Models.Portfolio;
// using Pi.PortfolioService.Application.Services.Models.StructureNote;
// using Pi.PortfolioService.Application.Queries;
// using Pi.PortfolioService.Application.Services;
//
// namespace Pi.PortfolioService.Application.Tests.Queries;
//
// public class PortfolioSummaryQueriesTests
// {
//     private readonly Mock<IFundService> _fundservice;
//     private readonly Mock<IPortfolioService> _portfolioService;
//     private readonly PortfolioSummaryQueries _portfolioSummaryQueries;
//     private readonly Mock<IStructureNoteService> _structureNoteService;
//     private readonly Mock<IFeatureService> _featureService;
//     private readonly Mock<ISetService> _setService;
//     private readonly Mock<IGeService> _geService;
//
//     public PortfolioSummaryQueriesTests()
//     {
//         _portfolioService = new Mock<IPortfolioService>();
//         _fundservice = new Mock<IFundService>();
//         _structureNoteService = new Mock<IStructureNoteService>();
//         _featureService = new Mock<IFeatureService>();
//         _setService = new Mock<ISetService>();
//         _geService = new Mock<IGeService>();
//
//         _portfolioSummaryQueries = new PortfolioSummaryQueries(
//             _portfolioService.Object,
//             _geService.Object,
//             _structureNoteService.Object,
//             _featureService.Object,
//             _fundservice.Object,
//             _setService.Object,
//             NullLogger<PortfolioSummaryQueries>.Instance
//         );
//     }
//
//     [Fact]
//     public async Task GetPortfolioSummaryAsync_ShouldReturnCombineData()
//     {
//         var portfolioMock = new PortfolioSummary(
//             DateTime.Now,
//             "",
//             0,
//             new List<PortfolioAccount>
//             {
//                 new(
//                     PortfolioAccountType.Cash.GetEnumDescription(),
//                     "54321",
//                     "000054321",
//                     "00005432-1",
//                     "005432100",
//                     false,
//                     100, // market
//                     100, // cash
//                     0, // upnl
//                     ""
//                 ),
//                 new(
//                     "Cash (to be remove as duplicated)",
//                     "54321",
//                     "000012345",
//                     "00001234-5",
//                     "001234500",
//                     false,
//                     0, // market
//                     0, // cash
//                     0, // upnl
//                     ""
//                 )
//             },
//             new List<PortfolioWalletCategorized>
//             {
//                 new(PortfolioAccountType.Cash.GetEnumDescription(), 100, 100, 0, 100)
//             },
//             new List<PortfolioErrorAccount>(),
//             new List<GeneralError>()
//         );
//
//         var snMock = new StructureNoteAccountSummary(
//             PortfolioAccountType.Offshore.ToString(),
//             "",
//             "000012345",
//             "001234500",
//             false,
//             100, // market
//             -50, // upnl
//             ""
//         );
//         var snErrorMock = new StructureNoteAccountSummary(
//             PortfolioAccountType.Offshore.ToString(),
//             "",
//             "000006789",
//             "000678900",
//             false,
//             0, // market
//             0, // upnl
//             "Test Error"
//         );
//
//         _portfolioService
//             .Setup(
//                 service =>
//                     service.GetByToken(
//                         It.IsAny<string>(),
//                         It.IsAny<Guid>(),
//                         It.IsAny<string>(),
//                         It.IsAny<CancellationToken>()
//                     )
//             )
//             .ReturnsAsync(portfolioMock);
//
//         _structureNoteService
//             .Setup(
//                 service =>
//                     service.GetStructureNotes(
//                         It.IsAny<string>(),
//                         It.IsAny<string>(),
//                         It.IsAny<CancellationToken>()
//                     )
//             )
//             .ReturnsAsync(new List<StructureNoteAccountSummary> { snMock, snErrorMock });
//         _fundservice.Setup(q => q.GetPortfolioAccounts(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
//             .ReturnsAsync(new List<PortfolioAccount>());
//
//         _featureService.Setup(q => q.IsOn(Features.SummaryPiApi)).Returns(true);
//         _featureService.Setup(q => q.IsOn(Features.RemoveDuplicateTradingAccount)).Returns(true);
//
//         var result =
//             await _portfolioSummaryQueries.GetPortfolioSummaryAsync("", Guid.NewGuid(), Guid.NewGuid().ToString(), "");
//
//         Assert.Equal(300, result.TotalValue);
//         Assert.Equal(200, result.TotalMarketValue);
//         Assert.Equal(250, result.TotalCostValue);
//         Assert.Equal(100, result.CashBalance);
//         Assert.Equal(-50, result.Upnl);
//         Assert.Equal(-20, result.UpnlPercentage);
//
//         Assert.Equal(3, result.PortfolioAccounts.Count());
//
//         Assert.Equal(2, result.PortfolioWalletCategorizeds.Count());
//         Assert.Equal(
//             new List<string> { "66.67", "33.33" },
//             result.PortfolioWalletCategorizeds.Select(wl => wl.AssetRatioInAllAsset.ToString("F2"))
//         );
//
//         Assert.Single(result.PortfolioErrorAccounts);
//     }
//
//     [Fact]
//     public async Task GetPortfolioSummaryAsync_Should_Not_Replace_PortfolioSummarySET_When_Sirius_SET_Accounts_Are_Empty()
//     {
//         // Arrange
//         var portfolioMock = new PortfolioSummary(
//             DateTime.Now,
//             "",
//             0,
//             new List<PortfolioAccount>
//             {
//                 FakesPortfolioAccount(PortfolioAccountType.MutualFund.GetEnumDescription()) with {AccountId = "111111", AccountNoForDisplay = "7800547-M", TotalMarketValue = 100m, CashBalance = 100m, Upnl = 10m},
//             },
//             new List<PortfolioWalletCategorized>(),
//             new List<PortfolioErrorAccount>(),
//             new List<GeneralError>()
//         );
//         var fundPortfolioAccounts = new List<PortfolioAccount>()
//         {
//             FakesPortfolioAccount(PortfolioAccountType.Cash.GetEnumDescription()) with {AccountId = "999999", AccountNoForDisplay = "08005471", TotalMarketValue = 350.77m, CashBalance = 0m, Upnl = 100.21m},
//             FakesPortfolioAccount(PortfolioAccountType.CashBalance.GetEnumDescription()) with {AccountId = "777777", AccountNoForDisplay = "98005498", TotalMarketValue = 550.55m, CashBalance = 0m, Upnl = -150.67m},
//             FakesPortfolioAccount(PortfolioAccountType.CreditBalance.GetEnumDescription()) with {AccountId = "888888", AccountNoForDisplay = "78005476", TotalMarketValue = 450m, CashBalance = 0m, Upnl = -100m},
//         };
//
//         _featureService.Setup(q => q.IsOn(Features.SummaryPiApi)).Returns(true);
//         _portfolioService
//             .Setup(
//                 service =>
//                     service.GetByToken(
//                         It.IsAny<string>(),
//                         It.IsAny<Guid>(),
//                         It.IsAny<string>(),
//                         It.IsAny<CancellationToken>()
//                     )
//             )
//             .ReturnsAsync(portfolioMock);
//         _fundservice.Setup(q => q.GetPortfolioAccounts(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
//             .ReturnsAsync(fundPortfolioAccounts);
//
//         // Act
//         var result = await _portfolioSummaryQueries.GetPortfolioSummaryAsync("", Guid.NewGuid(), Guid.NewGuid().ToString(), "");
//
//         // Assert
//         Assert.Single(result.PortfolioAccounts);
//         Assert.Equal(new List<string> { "111111" }, result.PortfolioAccounts.Select(q => q.AccountId).Order());
//         Assert.Equal(new List<string> { "7800547-M" }, result.PortfolioAccounts.Select(q => q.AccountNoForDisplay).Order());
//         Assert.Equal(new List<string> { PortfolioAccountType.MutualFund.GetEnumDescription() }, result.PortfolioAccounts.Select(q => q.AccountType).Order());
//         Assert.Equal(200m, result.TotalValue);
//         Assert.Equal(100m, result.TotalMarketValue);
//         Assert.Equal(90m, result.TotalCostValue);
//         Assert.Equal(100m, result.CashBalance);
//         Assert.Equal(10m, result.Upnl);
//         Assert.Equal(11.1111m, decimal.Round(result.UpnlPercentage, 4));
//         Assert.Equal(
//             new List<string> { "100.00" },
//             result.PortfolioWalletCategorizeds.Select(wl => wl.AssetRatioInAllAsset.ToString("F2"))
//         );
//         Assert.Equal(
//             new List<decimal> { 200m },
//             result.PortfolioWalletCategorizeds.Select(wl => wl.TotalValue)
//         );
//     }
//
//     [Fact]
//     public async Task GetPortfolioSummaryAsync_Should_Not_Replace_PortfolioSummaryFund_When_Sirius_MF_Is_Empty()
//     {
//         // Arrange
//         var portfolioMock = new PortfolioSummary(
//             DateTime.Now,
//             "",
//             0,
//             new List<PortfolioAccount>
//             {
//                 FakesPortfolioAccount(PortfolioAccountType.Cash.GetEnumDescription()) with {AccountId = "111111", AccountNoForDisplay = "78005478", TotalMarketValue = 100m, CashBalance = 100m, Upnl = 10m},
//             },
//             new List<PortfolioWalletCategorized>(),
//             new List<PortfolioErrorAccount>(),
//             new List<GeneralError>()
//         );
//         var fundPortfolioAccounts = new List<PortfolioAccount>()
//         {
//             FakesPortfolioAccount(PortfolioAccountType.MutualFund.GetEnumDescription()) with {AccountId = "999999", AccountNoForDisplay = "0800547M", TotalMarketValue = 350.77m, CashBalance = 0m, Upnl = 100.21m},
//             FakesPortfolioAccount(PortfolioAccountType.MutualFund.GetEnumDescription()) with {AccountId = "777777", AccountNoForDisplay = "98005491", TotalMarketValue = 550.55m, CashBalance = 0m, Upnl = -150.67m},
//             FakesPortfolioAccount(PortfolioAccountType.MutualFund.GetEnumDescription()) with {AccountId = "888888", AccountNoForDisplay = "7800547M", TotalMarketValue = 450m, CashBalance = 0m, Upnl = -100m},
//         };
//
//         _featureService.Setup(q => q.IsOn(Features.SummaryPiApi)).Returns(true);
//         _portfolioService
//             .Setup(
//                 service =>
//                     service.GetByToken(
//                         It.IsAny<string>(),
//                         It.IsAny<Guid>(),
//                         It.IsAny<string>(),
//                         It.IsAny<CancellationToken>()
//                     )
//             )
//             .ReturnsAsync(portfolioMock);
//         _fundservice.Setup(q => q.GetPortfolioAccounts(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
//             .ReturnsAsync(fundPortfolioAccounts);
//
//         // Act
//         var result = await _portfolioSummaryQueries.GetPortfolioSummaryAsync("", Guid.NewGuid(), Guid.NewGuid().ToString(), "");
//
//         // Assert
//         Assert.Single(result.PortfolioAccounts);
//         Assert.Equal(new List<string> { "111111" }, result.PortfolioAccounts.Select(q => q.AccountId).Order());
//         Assert.Equal(new List<string> { "78005478" }, result.PortfolioAccounts.Select(q => q.AccountNoForDisplay).Order());
//         Assert.Equal(new List<string> { PortfolioAccountType.Cash.GetEnumDescription() }, result.PortfolioAccounts.Select(q => q.AccountType).Order());
//         Assert.Equal(200m, result.TotalValue);
//         Assert.Equal(100m, result.TotalMarketValue);
//         Assert.Equal(90m, result.TotalCostValue);
//         Assert.Equal(100m, result.CashBalance);
//         Assert.Equal(10m, result.Upnl);
//         Assert.Equal(11.1111m, decimal.Round(result.UpnlPercentage, 4));
//         Assert.Equal(
//             new List<string> { "100.00" },
//             result.PortfolioWalletCategorizeds.Select(wl => wl.AssetRatioInAllAsset.ToString("F2"))
//         );
//         Assert.Equal(
//             new List<decimal> { 200m },
//             result.PortfolioWalletCategorizeds.Select(wl => wl.TotalValue)
//         );
//     }
//
//     [Fact]
//     public async Task GetPortfolioSummaryAsync_Should_ReplacePortfolioSummarySET_When_PiAccount_Matched_SiriusAccount()
//     {
//         // Arrange
//         var portfolioMock = new PortfolioSummary(
//             DateTime.Now,
//             "",
//             0,
//             new List<PortfolioAccount>
//             {
//                 FakesPortfolioAccount(PortfolioAccountType.MutualFund.GetEnumDescription()) with {AccountId = "111111", AccountNoForDisplay = "7800547M", TotalMarketValue = 100m, CashBalance = 100m, Upnl = 0m},
//                 FakesPortfolioAccount(PortfolioAccountType.Cash.GetEnumDescription()) with {AccountId = "123456", AccountNoForDisplay = "08005471", TotalMarketValue = 250m, CashBalance = 0m, Upnl = 100m},
//                 FakesPortfolioAccount(PortfolioAccountType.CashBalance.GetEnumDescription()) with {AccountId = "654321", AccountNoForDisplay = "98005498", TotalMarketValue = 450m, CashBalance = 0m, Upnl = -100m},
//                 FakesPortfolioAccount(PortfolioAccountType.CreditBalance.GetEnumDescription()) with {AccountId = "555555", AccountNoForDisplay = "88005496", TotalMarketValue = 450m, CashBalance = 0m, Upnl = -100m},
//                 FakesPortfolioAccount(PortfolioAccountType.CreditBalance.GetEnumDescription()) with {AccountId = "777777", AccountNoForDisplay = "78005496", TotalMarketValue = 450m, CashBalance = 0m, Upnl = -100m},
//             },
//             new List<PortfolioWalletCategorized>(),
//             new List<PortfolioErrorAccount>(),
//             new List<GeneralError>()
//         );
//         var fundPortfolioAccounts = new List<PortfolioAccount>()
//         {
//             FakesPortfolioAccount(PortfolioAccountType.Cash.GetEnumDescription()) with {AccountId = "", AccountNoForDisplay = "08005471", TotalMarketValue = 350.77m, CashBalance = 0m, Upnl = 100.21m},
//             FakesPortfolioAccount(PortfolioAccountType.CashBalance.GetEnumDescription()) with {AccountId = "", AccountNoForDisplay = "98005498", TotalMarketValue = 550.55m, CashBalance = 0m, Upnl = -150.67m},
//             FakesPortfolioAccount(PortfolioAccountType.CreditBalance.GetEnumDescription()) with {AccountId = "", AccountNoForDisplay = "88005496", TotalMarketValue = 450m, CashBalance = 0m, Upnl = -100m},
//             FakesPortfolioAccount(PortfolioAccountType.CreditBalance.GetEnumDescription()) with {AccountId = "", AccountNoForDisplay = "78005486", TotalMarketValue = 450m, CashBalance = 0m, Upnl = -100m},
//         };
//
//         _featureService.Setup(q => q.IsOn(Features.SetPiApi)).Returns(true);
//         _portfolioService
//             .Setup(
//                 service =>
//                     service.GetByToken(
//                         It.IsAny<string>(),
//                         It.IsAny<Guid>(),
//                         It.IsAny<string>(),
//                         It.IsAny<CancellationToken>()
//                     )
//             )
//             .ReturnsAsync(portfolioMock);
//         _setService.Setup(q => q.GetPortfolioAccounts(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
//             .ReturnsAsync(fundPortfolioAccounts);
//
//         // Act
//         var result = await _portfolioSummaryQueries.GetPortfolioSummaryAsync("", Guid.NewGuid(), Guid.NewGuid().ToString(), "");
//
//         // Assert
//         Assert.Equal(5, result.PortfolioAccounts.Count);
//         Assert.Equal(new List<string> { "111111", "123456", "555555", "654321", "777777" }, result.PortfolioAccounts.Select(q => q.AccountId).Order());
//         Assert.Equal(new List<string> { "08005471", "7800547M", "78005496", "88005496", "98005498" }, result.PortfolioAccounts.Select(q => q.AccountNoForDisplay).Order());
//         Assert.Equal(new List<string> { PortfolioAccountType.Cash.GetEnumDescription(), PortfolioAccountType.CashBalance.GetEnumDescription(), PortfolioAccountType.CreditBalance.GetEnumDescription(), PortfolioAccountType.CreditBalance.GetEnumDescription(), PortfolioAccountType.MutualFund.GetEnumDescription() }, result.PortfolioAccounts.Select(q => q.AccountType).Order());
//         Assert.Equal(2001.32m, result.TotalValue);
//         Assert.Equal(1901.32m, result.TotalMarketValue);
//         Assert.Equal(2151.78m, result.TotalCostValue);
//         Assert.Equal(100m, result.CashBalance);
//         Assert.Equal(-250.46m, result.Upnl);
//         Assert.Equal(-11.6397m, decimal.Round(result.UpnlPercentage, 4));
//         Assert.Equal(
//             new List<string> { "9.99", "44.97", "17.53", "27.51" },
//             result.PortfolioWalletCategorizeds.Select(wl => wl.AssetRatioInAllAsset.ToString("F2"))
//         );
//         Assert.Equal(
//             new List<decimal> { 200m, 900m, 350.77m, 550.55m },
//             result.PortfolioWalletCategorizeds.Select(wl => wl.TotalValue)
//         );
//     }
//
//     [Fact]
//     public async Task GetPortfolioSummaryAsync_Should_ReplacePortfolioSummaryFund_When_PiAccount_Matched_SiriusAccount()
//     {
//         // Arrange
//         var portfolioMock = new PortfolioSummary(
//             DateTime.Now,
//             "",
//             0,
//             new List<PortfolioAccount>
//             {
//                 FakesPortfolioAccount(PortfolioAccountType.Cash.GetEnumDescription()) with {AccountId = "111111", AccountNoForDisplay = "78005478", TotalMarketValue = 100m, CashBalance = 100m, Upnl = 0m},
//                 FakesPortfolioAccount(PortfolioAccountType.MutualFund.GetEnumDescription()) with {AccountId = "123456", AccountNoForDisplay = "0800547M", TotalMarketValue = 250m, CashBalance = 0m, Upnl = 100m},
//                 FakesPortfolioAccount(PortfolioAccountType.MutualFund.GetEnumDescription()) with {AccountId = "654321", AccountNoForDisplay = "98005491", TotalMarketValue = 450m, CashBalance = 0m, Upnl = -100m},
//                 FakesPortfolioAccount(PortfolioAccountType.MutualFund.GetEnumDescription()) with {AccountId = "555555", AccountNoForDisplay = "8800549M", TotalMarketValue = 450m, CashBalance = 0m, Upnl = -100m},
//             },
//             new List<PortfolioWalletCategorized>(),
//             new List<PortfolioErrorAccount>(),
//             new List<GeneralError>()
//         );
//         var fundPortfolioAccounts = new List<PortfolioAccount>()
//         {
//             FakesPortfolioAccount(PortfolioAccountType.MutualFund.GetEnumDescription()) with {AccountId = "999999", AccountNoForDisplay = "0800547M", TotalMarketValue = 350.77m, CashBalance = 0m, Upnl = 100.21m},
//             FakesPortfolioAccount(PortfolioAccountType.MutualFund.GetEnumDescription()) with {AccountId = "777777", AccountNoForDisplay = "98005491", TotalMarketValue = 550.55m, CashBalance = 0m, Upnl = -150.67m},
//             FakesPortfolioAccount(PortfolioAccountType.MutualFund.GetEnumDescription()) with {AccountId = "888888", AccountNoForDisplay = "7800547M", TotalMarketValue = 450m, CashBalance = 0m, Upnl = -100m},
//         };
//
//         _featureService.Setup(q => q.IsOn(Features.SummaryPiApi)).Returns(true);
//         _portfolioService
//             .Setup(
//                 service =>
//                     service.GetByToken(
//                         It.IsAny<string>(),
//                         It.IsAny<Guid>(),
//                         It.IsAny<string>(),
//                         It.IsAny<CancellationToken>()
//                     )
//             )
//             .ReturnsAsync(portfolioMock);
//         _fundservice.Setup(q => q.GetPortfolioAccounts(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
//             .ReturnsAsync(fundPortfolioAccounts);
//
//         // Act
//         var result = await _portfolioSummaryQueries.GetPortfolioSummaryAsync("", Guid.NewGuid(), Guid.NewGuid().ToString(), "");
//
//         // Assert
//         Assert.Equal(4, result.PortfolioAccounts.Count);
//         Assert.Equal(new List<string> { "111111", "123456", "555555", "654321" }, result.PortfolioAccounts.Select(q => q.AccountId).Order());
//         Assert.Equal(new List<string> { "0800547M", "78005478", "8800549M", "98005491" }, result.PortfolioAccounts.Select(q => q.AccountNoForDisplay).Order());
//         Assert.Equal(new List<string> { PortfolioAccountType.Cash.GetEnumDescription(), PortfolioAccountType.MutualFund.GetEnumDescription(), PortfolioAccountType.MutualFund.GetEnumDescription(), PortfolioAccountType.MutualFund.GetEnumDescription() }, result.PortfolioAccounts.Select(q => q.AccountType).Order());
//         Assert.Equal(1551.32m, result.TotalValue);
//         Assert.Equal(1451.32m, result.TotalMarketValue);
//         Assert.Equal(1601.78m, result.TotalCostValue);
//         Assert.Equal(100m, result.CashBalance);
//         Assert.Equal(-150.46m, result.Upnl);
//         Assert.Equal(-9.3933m, decimal.Round(result.UpnlPercentage, 4));
//         Assert.Equal(
//             new List<string> { "12.89", "87.11" },
//             result.PortfolioWalletCategorizeds.Select(wl => wl.AssetRatioInAllAsset.ToString("F2"))
//         );
//         Assert.Equal(
//             new List<decimal> { 200m, 1351.32m },
//             result.PortfolioWalletCategorizeds.Select(wl => wl.TotalValue)
//         );
//     }
//
//     [Fact]
//     public async Task GetPortfolioSummaryAsync_Should_Not_ReplacePortfolioSummarySET_When_Pi_SET_Accounts_Are_Empty()
//     {
//         // Arrange
//         var portfolioMock = new PortfolioSummary(
//             DateTime.Now,
//             "",
//             0,
//             new List<PortfolioAccount>
//             {
//                 FakesPortfolioAccount(PortfolioAccountType.MutualFund.GetEnumDescription()) with {AccountId = "111111", AccountNoForDisplay = "7800547M", TotalMarketValue = 100m, CashBalance = 100m, Upnl = 0m},
//                 FakesPortfolioAccount(PortfolioAccountType.Cash.GetEnumDescription()) with {AccountId = "123456", AccountNoForDisplay = "08005471", TotalMarketValue = 250m, CashBalance = 0m, Upnl = 100m},
//                 FakesPortfolioAccount(PortfolioAccountType.CashBalance.GetEnumDescription()) with {AccountId = "654321", AccountNoForDisplay = "98005498", TotalMarketValue = 450m, CashBalance = 100m, Upnl = -100m},
//                 FakesPortfolioAccount(PortfolioAccountType.CreditBalance.GetEnumDescription()) with {AccountId = "555555", AccountNoForDisplay = "88005496", TotalMarketValue = 450m, CashBalance = 100m, Upnl = -100m},
//                 FakesPortfolioAccount(PortfolioAccountType.CreditBalance.GetEnumDescription()) with {AccountId = "777777", AccountNoForDisplay = "78005496", TotalMarketValue = 450m, CashBalance = 100m, Upnl = -100m},
//             },
//             new List<PortfolioWalletCategorized>(),
//             new List<PortfolioErrorAccount>(),
//             new List<GeneralError>()
//         );
//
//         _featureService.Setup(q => q.IsOn(Features.SetPiApi)).Returns(true);
//         _portfolioService
//             .Setup(
//                 service =>
//                     service.GetByToken(
//                         It.IsAny<string>(),
//                         It.IsAny<Guid>(),
//                         It.IsAny<string>(),
//                         It.IsAny<CancellationToken>()
//                     )
//             )
//             .ReturnsAsync(portfolioMock);
//         _setService.Setup(q => q.GetPortfolioAccounts(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
//             .ReturnsAsync(new List<PortfolioAccount>());
//
//         // Act
//         var result = await _portfolioSummaryQueries.GetPortfolioSummaryAsync("", Guid.NewGuid(), Guid.NewGuid().ToString(), "");
//
//         // Assert
//         Assert.Equal(5, result.PortfolioAccounts.Count);
//         Assert.Equal(new List<string> { "111111", "123456", "555555", "654321", "777777" }, result.PortfolioAccounts.Select(q => q.AccountId).Order());
//         Assert.Equal(new List<string> { "08005471", "7800547M", "78005496", "88005496", "98005498" }, result.PortfolioAccounts.Select(q => q.AccountNoForDisplay).Order());
//         Assert.Equal(new List<string> { PortfolioAccountType.Cash.GetEnumDescription(), PortfolioAccountType.CashBalance.GetEnumDescription(), PortfolioAccountType.CreditBalance.GetEnumDescription(), PortfolioAccountType.CreditBalance.GetEnumDescription(), PortfolioAccountType.MutualFund.GetEnumDescription() }, result.PortfolioAccounts.Select(q => q.AccountType).Order());
//         Assert.Equal(2100m, result.TotalValue);
//         Assert.Equal(1700m, result.TotalMarketValue);
//         Assert.Equal(1900m, result.TotalCostValue);
//         Assert.Equal(400m, result.CashBalance);
//         Assert.Equal(-200m, result.Upnl);
//         Assert.Equal(-10.5263m, decimal.Round(result.UpnlPercentage, 4));
//         Assert.Equal(
//             new List<string> { "9.52", "11.90", "26.19", "52.38" },
//             result.PortfolioWalletCategorizeds.Select(wl => wl.AssetRatioInAllAsset.ToString("F2"))
//         );
//         Assert.Equal(
//             new List<decimal> { 200, 250, 550, 1100 },
//             result.PortfolioWalletCategorizeds.Select(wl => wl.TotalValue)
//         );
//     }
//
//     [Fact]
//     public async Task GetPortfolioSummaryAsync_Should_Not_ReplacePortfolioSummaryFund_When_Pi_Account_Is_Empty()
//     {
//         // Arrange
//         var portfolioMock = new PortfolioSummary(
//             DateTime.Now,
//             "",
//             0,
//             new List<PortfolioAccount>
//             {
//                 FakesPortfolioAccount(PortfolioAccountType.Cash.GetEnumDescription()) with {AccountId = "111111", AccountNoForDisplay = "78005478", TotalMarketValue = 100m, CashBalance = 100m, Upnl = 0m},
//                 FakesPortfolioAccount(PortfolioAccountType.MutualFund.GetEnumDescription()) with {AccountId = "123456", AccountNoForDisplay = "0800547M", TotalMarketValue = 250m, CashBalance = 0m, Upnl = 100m},
//                 FakesPortfolioAccount(PortfolioAccountType.MutualFund.GetEnumDescription()) with {AccountId = "654321", AccountNoForDisplay = "98005491", TotalMarketValue = 450m, CashBalance = 0m, Upnl = -100m},
//                 FakesPortfolioAccount(PortfolioAccountType.MutualFund.GetEnumDescription()) with {AccountId = "555555", AccountNoForDisplay = "8800549M", TotalMarketValue = 450m, CashBalance = 0m, Upnl = -100m},
//             },
//             new List<PortfolioWalletCategorized>(),
//             new List<PortfolioErrorAccount>(),
//             new List<GeneralError>()
//         );
//         var fundPortfolioAccounts = new List<PortfolioAccount>();
//
//         _featureService.Setup(q => q.IsOn(Features.SummaryPiApi)).Returns(true);
//         _portfolioService
//             .Setup(
//                 service =>
//                     service.GetByToken(
//                         It.IsAny<string>(),
//                         It.IsAny<Guid>(),
//                         It.IsAny<string>(),
//                         It.IsAny<CancellationToken>()
//                     )
//             )
//             .ReturnsAsync(portfolioMock);
//         _fundservice.Setup(q => q.GetPortfolioAccounts(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
//             .ReturnsAsync(fundPortfolioAccounts);
//
//         // Act
//         var result = await _portfolioSummaryQueries.GetPortfolioSummaryAsync("", Guid.NewGuid(), Guid.NewGuid().ToString(), "");
//
//         // Assert
//         Assert.Equal(4, result.PortfolioAccounts.Count);
//         Assert.Equal(new List<string> { "111111", "123456", "555555", "654321" }, result.PortfolioAccounts.Select(q => q.AccountId).Order());
//         Assert.Equal(new List<string> { "0800547M", "78005478", "8800549M", "98005491" }, result.PortfolioAccounts.Select(q => q.AccountNoForDisplay).Order());
//         Assert.Equal(new List<string> { PortfolioAccountType.Cash.GetEnumDescription(), PortfolioAccountType.MutualFund.GetEnumDescription(), PortfolioAccountType.MutualFund.GetEnumDescription(), PortfolioAccountType.MutualFund.GetEnumDescription() }, result.PortfolioAccounts.Select(q => q.AccountType).Order());
//         Assert.Equal(1350m, result.TotalValue);
//         Assert.Equal(1250m, result.TotalMarketValue);
//         Assert.Equal(1350m, result.TotalCostValue);
//         Assert.Equal(100m, result.CashBalance);
//         Assert.Equal(-100m, result.Upnl);
//         Assert.Equal(-7.4074m, decimal.Round(result.UpnlPercentage, 4));
//         Assert.Equal(
//             new List<string> { "14.81", "85.19" },
//             result.PortfolioWalletCategorizeds.Select(wl => wl.AssetRatioInAllAsset.ToString("F2"))
//         );
//         Assert.Equal(
//             new List<decimal> { 200m, 1150m },
//             result.PortfolioWalletCategorizeds.Select(wl => wl.TotalValue)
//         );
//     }
//
//     private static PortfolioAccount FakesPortfolioAccount(string accountType)
//     {
//         return new PortfolioAccount(accountType, "272112", "0800547-M", "0800547-M", "0800547", false, 34500.73m, 0m, 3555, "");
//     }
// }
