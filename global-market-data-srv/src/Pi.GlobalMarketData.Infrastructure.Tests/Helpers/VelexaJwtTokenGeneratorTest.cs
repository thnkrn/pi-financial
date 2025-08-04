using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Pi.GlobalMarketData.Infrastructure.Helpers;

namespace Pi.GlobalMarketData.Infrastructure.Tests.Helpers;

public class VelexaHttpApiJwtTokenGeneratorTests
{
    private readonly string _secretKey = "This is my 256 bits legnth key..";
    private readonly string _clientId = "mock-client-id";
    private readonly string _applicationId = "mock-application-id";

    [Fact]
    public void GenerateJwtToken_Should_Return_Valid_Token()
    {
        // Arrange
        var tokenGenerator = new VelexaHttpApiJwtTokenGenerator(
            _secretKey,
            _clientId,
            _applicationId
        );

        // Act
        var expireInSecond = 10;
        string token = tokenGenerator.GenerateJwtToken(expireInSecond);

        // Assert
        Assert.NotNull(token);
        Assert.False(string.IsNullOrEmpty(token));

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

        Assert.NotNull(jwtToken);
        Assert.Equal("JWT", jwtToken.Header["typ"]);
        Assert.Equal("HS256", jwtToken.Header["alg"]);
        Assert.Equal(_clientId, jwtToken.Payload["iss"]);
        Assert.Equal(_applicationId, jwtToken.Payload["sub"]);
    }

    [Fact]
    public void IsTokenExpired_Should_Return_False_For_Valid_Token()
    {
        // Arrange
        var tokenGenerator = new VelexaHttpApiJwtTokenGenerator(
            _secretKey,
            _clientId,
            _applicationId
        );
        var expireInSecond = 10;
        string token = tokenGenerator.GenerateJwtToken(expireInSecond);

        // Act
        bool isExpired = tokenGenerator.IsTokenExpired(token);

        // Assert
        Assert.False(isExpired);
    }

    [Fact]
    public void IsTokenExpired_Should_Return_True_For_Expired_Token()
    {
        // Arrange
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var header = new JwtHeader(credentials);
        header["typ"] = "JWT";

        // Set the expiration time to a past date to simulate an expired token
        var payload = new JwtPayload
        {
            { "iss", _clientId },
            { "sub", _applicationId },
            { "iat", 1722509037 },
            { "exp", DateTimeOffset.UtcNow.AddSeconds(-1).ToUnixTimeSeconds() }, // Expired
            {
                "aud",
                new string[]
                {
                    "ohlc",
                    "crossrates",
                    "symbols",
                    "change",
                    "feed",
                    "orders",
                    "summary",
                    "transactions",
                    "accounts"
                }
            }
        };

        var expiredToken = new JwtSecurityToken(header, payload);
        var tokenHandler = new JwtSecurityTokenHandler();
        string token = tokenHandler.WriteToken(expiredToken);

        var tokenGenerator = new VelexaHttpApiJwtTokenGenerator(
            _secretKey,
            _clientId,
            _applicationId
        );

        // Act
        bool isExpired = tokenGenerator.IsTokenExpired(token);

        // Assert
        Assert.True(isExpired);
    }
}
