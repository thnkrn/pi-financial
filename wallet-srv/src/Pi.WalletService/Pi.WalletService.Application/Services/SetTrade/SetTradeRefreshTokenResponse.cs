namespace Pi.WalletService.Application.Services.SetTrade;

public record SetTradeRefreshTokenResponse(string AccessToken, string RefreshToken, int ExpiresIn);