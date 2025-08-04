using System.Net;
using Microsoft.Extensions.Logging;
using Moq;
using Pi.GlobalMarketData.Domain.Models.Request;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Helpers;
using Pi.GlobalMarketData.NonRealTimeDataHandler.constants;
using Pi.GlobalMarketData.NonRealTimeDataHandler.Helpers;

namespace Pi.GlobalMarketData.NonRealTimeDataHandler.Tests.Helpers;

public class MorningStarCenterDataHelperTest
{
    private readonly MorningStarCenterDataHelper _handler;
    private readonly Mock<IHttpRequestHelper> _client;
    private readonly MorningStarCenterApiRequest _request;
    private readonly string _email = "email";
    private readonly string _password = "password";
    private readonly string _accessCode = "accessCode";

    public MorningStarCenterDataHelperTest()
    {
        _request = new MorningStarCenterApiRequest
        {
            Identifier = "identifer",
            AccessCode = "accessCode",
            ResponseTypeJson = "json"
        };

        _client = new Mock<IHttpRequestHelper>();
        _handler = new MorningStarCenterDataHelper(_client.Object);
    }

    [Fact]
    public async Task Call_CreateAccessCode_ShouldRunCorrectly()
    {
        // Arrange
        var expectedUrl = MorningStarCenterEndpoints.createAccessCode;
        var expectedMethod = HttpMethod.Post;
        var expectedProtocol = SecurityProtocolType.Tls12;

        string sURL = string.Format(expectedUrl, _email, _password, Expiration.D90.Value);
        _client
            .Setup(x => x.RequestByUrl(sURL, expectedMethod, expectedProtocol))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        // Act
        await _handler.CreateAccessCode(_email, _password);

        // Assert
        _client.Verify(x => x.RequestByUrl(sURL, null, null), Times.Once);
    }

    [Fact]
    public async Task Call_QueryAccessCode_ShouldRunCorrectly()
    {
        // Arrange
        var expectedUrl = MorningStarCenterEndpoints.queryAccessCode;
        var expectedMethod = HttpMethod.Post;
        var expectedProtocol = SecurityProtocolType.Tls12;

        string sURL = string.Format(expectedUrl, _email, _password, _accessCode);
        _client
            .Setup(x => x.RequestByUrl(sURL, expectedMethod, expectedProtocol))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        // Act
        await _handler.QueryAccessCode(_email, _password, _accessCode);

        // Assert
        _client.Verify(x => x.RequestByUrl(sURL, null, null), Times.Once);
    }

    [Fact]
    public async Task Call_DeleteAccessCode_ShouldRunCorrectly()
    {
        // Arrange
        var expectedUrl = MorningStarCenterEndpoints.deleteAccessCode;
        var expectedMethod = HttpMethod.Post;
        var expectedProtocol = SecurityProtocolType.Tls12;

        string sURL = string.Format(expectedUrl, _email, _password, _accessCode);
        _client
            .Setup(x => x.RequestByUrl(sURL, expectedMethod, expectedProtocol))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        // Act
        await _handler.DeleteAccessCode(_email, _password, _accessCode);

        // Assert
        _client.Verify(x => x.RequestByUrl(sURL, null, null), Times.Once);
    }

    [Fact]
    public async Task Call_GetNetAssets_ShouldRunCorrectly()
    {
        // Arrange
        var expectedUrl = MorningStarCenterEndpoints.NetAssets;
        var expectedMethod = HttpMethod.Get;
        var expectedProtocol = SecurityProtocolType.Tls12;

        string sURL = string.Format(
            expectedUrl,
            _request.IdentifierType,
            _request.Identifier,
            _request.ResponseTypeJson,
            _request.AccessCode
        );
        _client
            .Setup(x => x.RequestByUrl(sURL, expectedMethod, expectedProtocol))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        // Act
        await _handler.GetNetAssets(_request);

        // Assert
        _client.Verify(x => x.RequestByUrl(sURL, null, null), Times.Once);
    }

    [Fact]
    public async Task Call_GetCurrentPrice_ShouldRunCorrectly()
    {
        // Arrange
        var expectedUrl = MorningStarCenterEndpoints.CurrentPrice;
        var expectedMethod = HttpMethod.Get;
        var expectedProtocol = SecurityProtocolType.Tls12;

        string sURL = string.Format(
            expectedUrl,
            _request.IdentifierType,
            _request.Identifier,
            _request.ResponseTypeJson,
            _request.AccessCode
        );
        _client
            .Setup(x => x.RequestByUrl(sURL, expectedMethod, expectedProtocol))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        // Act
        await _handler.GetCurrentPrice(_request);

        // Assert
        _client.Verify(x => x.RequestByUrl(sURL, null, null), Times.Once);
    }

    [Fact]
    public async Task Call_GetFundShareClassBasicInfo_ShouldRunCorrectly()
    {
        // Arrange
        var expectedUrl = MorningStarCenterEndpoints.FundShareClassBasicInfo;
        var expectedMethod = HttpMethod.Get;
        var expectedProtocol = SecurityProtocolType.Tls12;

        string sURL = string.Format(
            expectedUrl,
            _request.IdentifierType,
            _request.Identifier,
            _request.ResponseTypeJson,
            _request.AccessCode
        );
        _client
            .Setup(x => x.RequestByUrl(sURL, expectedMethod, expectedProtocol))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        // Act
        await _handler.GetFundShareClassBasicInfo(_request);

        // Assert
        _client.Verify(x => x.RequestByUrl(sURL, null, null), Times.Once);
    }

    [Fact]
    public async Task Call_GetYields_ShouldRunCorrectly()
    {
        // Arrange
        var expectedUrl = MorningStarCenterEndpoints.Yields;
        var expectedMethod = HttpMethod.Get;
        var expectedProtocol = SecurityProtocolType.Tls12;

        string sURL = string.Format(
            expectedUrl,
            _request.IdentifierType,
            _request.Identifier,
            _request.ResponseTypeJson,
            _request.AccessCode
        );
        _client
            .Setup(x => x.RequestByUrl(sURL, expectedMethod, expectedProtocol))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        // Act
        await _handler.GetYields(_request);

        // Assert
        _client.Verify(x => x.RequestByUrl(sURL, null, null), Times.Once);
    }

    [Fact]
    public async Task Call_GetProspectusFees_ShouldRunCorrectly()
    {
        // Arrange
        var expectedUrl = MorningStarCenterEndpoints.ProspectusFees;
        var expectedMethod = HttpMethod.Get;
        var expectedProtocol = SecurityProtocolType.Tls12;

        string sURL = string.Format(
            expectedUrl,
            _request.IdentifierType,
            _request.Identifier,
            _request.ResponseTypeJson,
            _request.AccessCode
        );
        _client
            .Setup(x => x.RequestByUrl(sURL, expectedMethod, expectedProtocol))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        // Act
        await _handler.GetProspectusFees(_request);

        // Assert
        _client.Verify(x => x.RequestByUrl(sURL, null, null), Times.Once);
    }

    [Fact]
    public async Task Call_GetInvestmentCriteria_ShouldRunCorrectly()
    {
        // Arrange
        var expectedUrl = MorningStarCenterEndpoints.InvestmentCriteria;
        var expectedMethod = HttpMethod.Get;
        var expectedProtocol = SecurityProtocolType.Tls12;

        string sURL = string.Format(
            expectedUrl,
            _request.IdentifierType,
            _request.Identifier,
            _request.ResponseTypeJson,
            _request.AccessCode
        );
        _client
            .Setup(x => x.RequestByUrl(sURL, expectedMethod, expectedProtocol))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        // Act
        await _handler.GetInvestmentCriteria(_request);

        // Assert
        _client.Verify(x => x.RequestByUrl(sURL, null, null), Times.Once);
    }
}
