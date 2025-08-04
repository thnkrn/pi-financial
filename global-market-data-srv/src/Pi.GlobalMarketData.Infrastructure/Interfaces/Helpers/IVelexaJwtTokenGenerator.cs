namespace Pi.GlobalMarketData.Infrastructure.Interfaces.Helpers;

public interface IVelexaHttpApiJwtTokenGenerator
{
    string GenerateJwtToken(int expireInSecond);
    bool IsTokenExpired(string token);
}