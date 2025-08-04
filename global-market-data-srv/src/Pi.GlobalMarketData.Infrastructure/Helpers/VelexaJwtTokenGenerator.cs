using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Helpers;

namespace Pi.GlobalMarketData.Infrastructure.Helpers;

public class VelexaHttpApiJwtTokenGenerator : IVelexaHttpApiJwtTokenGenerator
{
    private readonly string _applicationId;
    private readonly string _clientId;
    private readonly string _secretKey;

    public VelexaHttpApiJwtTokenGenerator(string secretKey, string clientId, string applicationId)
    {
        _secretKey = secretKey;
        _clientId = clientId;
        _applicationId = applicationId;
    }

    public string GenerateJwtToken(int expireInSecond)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var header = new JwtHeader(credentials);
        header["typ"] = "JWT";

        var issueAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var expireAt = issueAt + expireInSecond;

        // https://api-live.velexa.com/api-docs/#section/Authentication
        var payload = new JwtPayload
        {
            { "iss", _clientId },
            { "sub", _applicationId },
            { "iat", issueAt },
            { "exp", expireAt },
            {
                "aud",
                new List<string>
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
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

        if (jwtToken == null)
            throw new ArgumentException("Invalid JWT token");

        var expClaim = jwtToken.Claims.FirstOrDefault(claim =>
            claim.Type == JwtRegisteredClaimNames.Exp
        );

        if (expClaim == null)
            throw new ArgumentException("JWT token does not contain 'exp' claim");

        var expUnix = long.Parse(expClaim.Value);
        var expirationTime = DateTimeOffset.FromUnixTimeSeconds(expUnix).UtcDateTime;

        return expirationTime < DateTime.UtcNow;
    }
}