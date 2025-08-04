using System.Text.Json.Serialization;

namespace Pi.WalletService.Application.Services.SetTrade;

public record SetTradeLoginResponse(
        [property: JsonPropertyName("token_type")]
        string TokenType,
        [property: JsonPropertyName("access_token")]
        string AccessToken,
        [property: JsonPropertyName("refresh_token")]
        string RefreshToken,
        [property: JsonPropertyName("expires_in")]
        int ExpiresIn,
        [property: JsonPropertyName("broker_id")]
        string BrokerId,
        [property: JsonPropertyName("authenticated_userid")]
        string AuthUserId
    );

public record SetTradeAuthResponse(bool Success, string TokenType, string AccessToken, string RefreshToken, int ExpiresIn);
