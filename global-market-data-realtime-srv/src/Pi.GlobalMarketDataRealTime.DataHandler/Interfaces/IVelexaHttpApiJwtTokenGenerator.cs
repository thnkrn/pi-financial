namespace Pi.GlobalMarketDataRealTime.DataHandler.Interfaces;

public interface IVelexaHttpApiJwtTokenGenerator
{
    string GenerateJwtToken(int expireInSecond);
    bool IsTokenExpired(string token);
}