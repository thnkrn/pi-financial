namespace Pi.TfexService.Application.Services.SetTrade;

public record SetTradeAuthResponse(bool Success, string TokenType, string AccessToken, string RefreshToken, int ExpiresIn);