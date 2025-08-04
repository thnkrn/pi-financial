using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Pi.GlobalMarketDataRealTime.DataHandler.Exceptions;
using Pi.GlobalMarketDataRealTime.DataHandler.Interfaces;

namespace Pi.GlobalMarketDataRealTime.DataHandler.Helpers;

public class VelexaHttpApiJwtTokenGenerator : IVelexaHttpApiJwtTokenGenerator
{
    private readonly string _applicationId;
    private readonly string _clientId;
    private readonly ILogger<VelexaHttpApiJwtTokenGenerator> _logger;
    private readonly string _secretKey;

    public VelexaHttpApiJwtTokenGenerator(string secretKey, string clientId, string applicationId,
        ILogger<VelexaHttpApiJwtTokenGenerator> logger)
    {
        if (string.IsNullOrWhiteSpace(secretKey))
            throw new ArgumentException("Secret key cannot be null or empty.", nameof(secretKey));
        if (string.IsNullOrWhiteSpace(clientId))
            throw new ArgumentException("Client ID cannot be null or empty.", nameof(clientId));
        if (string.IsNullOrWhiteSpace(applicationId))
            throw new ArgumentException("Application ID cannot be null or empty.", nameof(applicationId));

        _secretKey = secretKey;
        _clientId = clientId;
        _applicationId = applicationId;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    ///     Call to https://api-live.velexa.com/api-docs/#section/Authentication
    /// </summary>
    /// <param name="expireInSecond"></param>
    /// <returns></returns>
    public string GenerateJwtToken(int expireInSecond)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var header = new JwtHeader(credentials)
        {
            ["typ"] = "JWT"
        };

        var issueAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var expireAt = issueAt + expireInSecond;
        var payload = new JwtPayload
        {
            { "iss", _clientId },
            { "sub", _applicationId },
            { "iat", issueAt },
            { "exp", expireAt },
            {
                "aud", new List<string>
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

        var token = new JwtSecurityToken(header, payload);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public bool IsTokenExpired(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            if (tokenHandler.ReadToken(token) is not JwtSecurityToken jwtToken)
                throw new ArgumentException("Invalid JWT token");

            var expClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Exp);

            if (expClaim == null)
                throw new ArgumentException("JWT token does not contain 'exp' claim");

            var expUnix = long.Parse(expClaim.Value);
            var expirationTime = DateTimeOffset.FromUnixTimeSeconds(expUnix).UtcDateTime;

            return expirationTime < DateTime.UtcNow;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in IsTokenExpired: {Message}", ex.Message);
            throw new SubscriptionServiceException($"Error in IsTokenExpired: {ex.Message}", ex);
        }
    }
}