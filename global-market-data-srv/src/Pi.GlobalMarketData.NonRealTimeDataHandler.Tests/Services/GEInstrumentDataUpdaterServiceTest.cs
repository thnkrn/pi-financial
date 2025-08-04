using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Moq;
using Pi.GlobalMarketData.Domain.ConstantConfigurations;
using Pi.GlobalMarketData.Domain.Entities;
using Pi.GlobalMarketData.Domain.Models.Response;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Mongo;
using Pi.GlobalMarketData.NonRealTimeDataHandler.constants;
using Pi.GlobalMarketData.NonRealTimeDataHandler.interfaces;
using Pi.GlobalMarketData.NonRealTimeDataHandler.Services;

namespace Pi.GlobalMarketData.NonRealTimeDataHandler.Tests.Services;

public class GEInstrumentDataUpdaterServiceTest
{
    private readonly string _email = "email";
    private readonly string _password = "password";
    private readonly Mock<IHttpClientFactory> _client;
    private readonly GEInstrumentDataUpdaterService _handler;
    private readonly Mock<IGeDataRepository> _mockGERepository;

    public GEInstrumentDataUpdaterServiceTest()
    {
        _client = new Mock<IHttpClientFactory>();

        var mockConfiguration = new Mock<IConfiguration>();
        var mockConfigSection = new Mock<IConfigurationSection>();
        var mockFullStockExchangeSecurityEntity =
            new Mock<IMongoService<FullStockExchangeSecurityEntity>>();

        _mockGERepository = new Mock<IGeDataRepository>();
        var mockMorningStarFlagService = new Mock<IMongoService<MorningStarFlag>>();

        var mockLogger = new Mock<ILogger<GEInstrumentDataUpdaterService>>();

        _client.Setup(x => x.CreateClient(HttpClientKeys.MorningStar)).Returns(new HttpClient());

        mockConfigSection.Setup(x => x.Value).Returns(_email);
        mockConfiguration
            .Setup(x => x.GetSection(ConfigurationKeys.MorningStarEmail))
            .Returns(mockConfigSection.Object);

        mockConfigSection.Setup(x => x.Value).Returns(_password);
        mockConfiguration
            .Setup(x => x.GetSection(ConfigurationKeys.MorningStarPassword))
            .Returns(mockConfigSection.Object);

        _handler = new GEInstrumentDataUpdaterService(
            _client.Object,
            mockConfiguration.Object,
            _mockGERepository.Object,
            mockFullStockExchangeSecurityEntity.Object,
            mockMorningStarFlagService.Object,
            new ApplicationLifetime(new Logger<ApplicationLifetime>(new LoggerFactory())),
            mockLogger.Object
        );
    }

    [Fact]
    public async Task Call_StartAsync_ShouldRunCorrectly()
    {
        // Arrange
        var cancellationToken = new CancellationToken(false);
        _mockGERepository
            .Setup(x => x.GeVenueMapping.GetAllAsync())
            .ReturnsAsync(
                [
                    new GeVenueMapping
                    {
                        Id = "1",
                        Source = "source",
                        Exchange = "exchange",
                        Mic = "mic",
                        ExchangeIdMs = "exchangeIdMs",
                        VenueCode = "venueCode"
                    }
                ]
            );
        _mockGERepository
            .Setup(x => x.GeInstrument.GetAllAsync())
            .ReturnsAsync(
                [
                    new GeInstrument
                    {
                        Id = null,
                        Symbol = "symbol",
                        Exchange = "exchange",
                        Mic = "mic",
                        StandardTicker = "standardTicker",
                    }
                ]
            );

        // Act
        await _handler.StartAsync(cancellationToken);

        // Assert
        _client.Verify();
    }
}
