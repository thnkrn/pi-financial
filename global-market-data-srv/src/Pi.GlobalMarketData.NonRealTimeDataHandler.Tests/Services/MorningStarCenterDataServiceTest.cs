// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.Logging;
// using Moq;
// using Pi.GlobalMarketData.Domain.ConstantConfigurations;
// using Pi.GlobalMarketData.Domain.Entities;
// using Pi.GlobalMarketData.Infrastructure.Interfaces.Mongo;
// using Pi.GlobalMarketData.NonRealTimeDataHandler.constants;
// using Pi.GlobalMarketData.NonRealTimeDataHandler.interfaces;
// using Pi.GlobalMarketData.NonRealTimeDataHandler.Services;
//
// namespace Pi.GlobalMarketData.NonRealTimeDataHandler.Tests.Services;
//
// public class MorningStarCenterDataServiceTest
// {
//     private readonly MorningStarCenterDataService _handler;
//     private readonly Mock<IHttpClientFactory> _client;
//     private readonly string? _accountCode = "";
//     private readonly string? _password = "";
//
//     public MorningStarCenterDataServiceTest()
//     {
//         _client = new Mock<IHttpClientFactory>();
//         var mockConfiguration = new Mock<IConfiguration>();
//         var mockConfigSection = new Mock<IConfigurationSection>();
//         var mockMorningStarCenterServiceHelper = new Mock<IMorningStarCenterServiceHelper>();
//         var mockMorningStarFlagService = new Mock<IMongoService<MorningStarFlag>>();
//
//         var mockLogger = new Mock<ILogger<MorningStarCenterDataService>>();
//
//         _client
//             .Setup(x => x.CreateClient(HttpClientKeys.MorningStarCenter))
//             .Returns(new HttpClient());
//
//         mockConfigSection.Setup(x => x.Value).Returns(_accountCode);
//         mockConfiguration
//             .Setup(x => x.GetSection(ConfigurationKeys.MorningStarCenterAccountCode))
//             .Returns(mockConfigSection.Object);
//
//         mockConfigSection.Setup(x => x.Value).Returns(_password);
//         mockConfiguration
//             .Setup(x => x.GetSection(ConfigurationKeys.MorningStarCenterPassword))
//             .Returns(mockConfigSection.Object);
//
//         _handler = new MorningStarCenterDataService(
//             _client.Object,
//             mockConfiguration.Object,
//             mockMorningStarCenterServiceHelper.Object,
//             mockMorningStarFlagService.Object,
//             mockLogger.Object
//         );
//     }
//
//     [Fact]
//     public async Task Call_StartAsync_ShouldRunCorrectly()
//     {
//         // Arrange
//         var cancellationToken = new CancellationToken(false);
//
//         // Act
//         await _handler.StartAsync(cancellationToken);
//
//         // Assert
//         _client.Verify();
//     }
// }
