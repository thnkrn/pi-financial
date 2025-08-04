using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Pi.SetMarketData.Domain.ConstantConfigurations;
using Pi.SetMarketData.Domain.Entities;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;
using Pi.SetMarketData.NonRealTimeDataHandler.constants;
using Pi.SetMarketData.NonRealTimeDataHandler.interfaces;
using Pi.SetMarketData.NonRealTimeDataHandler.Services;

namespace Pi.SetMarketData.NonRealTimeDataHandler.Tests.Services;

public class MorningstarDataServiceTest
{
    private readonly MorningStarDataService _handler;
    private readonly Mock<IHttpClientFactory> _client;
    private readonly string _email = "email";
    private readonly string _password = "password";

    public MorningstarDataServiceTest()
    {
        _client = new Mock<IHttpClientFactory>();
        var mockConfiguration = new Mock<IConfiguration>();
        var mockConfigSection = new Mock<IConfigurationSection>();
        var mockMorningStarFlagService = new Mock<IMongoService<MorningStarFlag>>();
        var mockMorningStarStockService = new Mock<IMongoService<MorningStarStocks>>();
        var mockHostApplicationLifetime = new Mock<IHostApplicationLifetime>();

        var mockLogger = new Mock<ILogger<MorningStarDataService>>();

        _client.Setup(x => x.CreateClient(HttpClientKeys.MorningStar)).Returns(new HttpClient());

        mockConfigSection.Setup(x => x.Value).Returns(_email);
        mockConfiguration
            .Setup(x => x.GetSection(ConfigurationKeys.MorningStarEmail))
            .Returns(mockConfigSection.Object);

        mockConfigSection.Setup(x => x.Value).Returns(_password);
        mockConfiguration
            .Setup(x => x.GetSection(ConfigurationKeys.MorningStarPassword))
            .Returns(mockConfigSection.Object);

        _handler = new MorningStarDataService(
            _client.Object,
            mockConfiguration.Object,
            mockMorningStarFlagService.Object,
            mockMorningStarStockService.Object,
            mockHostApplicationLifetime.Object,
            mockLogger.Object
        );
    }

    [Fact]
    public async Task Call_StartAsync_ShouldRunCorrectly()
    {
        // Arrange
        var cancellationToken = new CancellationToken(false);

        // Act
        await _handler.StartAsync(cancellationToken);

        // Assert
        _client.Verify();
    }
}
