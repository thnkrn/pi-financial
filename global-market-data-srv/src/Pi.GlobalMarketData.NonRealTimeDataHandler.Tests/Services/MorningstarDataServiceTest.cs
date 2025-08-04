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
// public class MorningstarDataServiceTest
// {
//     private readonly MorningStarDataService _handler;
//     private readonly Mock<IHttpClientFactory> _client;
//     private readonly string _email = "email";
//     private readonly string _password = "password";
//
//     public MorningstarDataServiceTest()
//     {
//         _client = new Mock<IHttpClientFactory>();
//         var mockConfiguration = new Mock<IConfiguration>();
//         var mockConfigSection = new Mock<IConfigurationSection>();
//         var mockMorningStarServiceHelper = new Mock<IMorningStarServiceHelper>();
//         var mockMorningStarFlagService = new Mock<IMongoService<MorningStarFlag>>();
//
//         var mockLogger = new Mock<ILogger<MorningStarDataService>>();
//
//         _client.Setup(x => x.CreateClient(HttpClientKeys.MorningStar)).Returns(new HttpClient());
//
//         mockConfigSection.Setup(x => x.Value).Returns(_email);
//         mockConfiguration
//             .Setup(x => x.GetSection(ConfigurationKeys.MorningStarEmail))
//             .Returns(mockConfigSection.Object);
//
//         mockConfigSection.Setup(x => x.Value).Returns(_password);
//         mockConfiguration
//             .Setup(x => x.GetSection(ConfigurationKeys.MorningStarPassword))
//             .Returns(mockConfigSection.Object);
//
//         _handler = new MorningStarDataService(
//             _client.Object,
//             mockConfiguration.Object,
//             mockMorningStarServiceHelper.Object,
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
