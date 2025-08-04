package util

import (
	"os"
	"strconv"
	"time"

	"github.com/golang-jwt/jwt/v5"
	"github.com/google/uuid"
	"github.com/pi-financial/pi-sso-v2/config"
	"github.com/pi-financial/pi-sso-v2/internal/types"
)

func GenerateAccessToken(cfg config.Config, accountID uuid.UUID, userId *string) (string, error) {
	expiration := time.Duration(cfg.JwtExpiration) * time.Hour
	claims := jwt.MapClaims{
		"account_id":       accountID,
		"user_id":          userId,
		"created_at":       time.Now().Unix(),
		"exp":              time.Now().Add(expiration).Unix(),
		"is_refresh_token": false, // Access Token
	}

	token := jwt.NewWithClaims(jwt.SigningMethodHS256, claims)
	return token.SignedString([]byte(cfg.JwtSecret))
}

func GenerateRefreshToken(cfg config.Config, accountID uuid.UUID, userId *string) (string, error) {
	expiration := time.Duration(cfg.RefreshExpiration) * time.Hour
	claims := jwt.MapClaims{
		"account_id":       accountID,
		"user_id":          userId,
		"created_at":       time.Now().Unix(),
		"exp":              time.Now().Add(expiration).Unix(),
		"is_refresh_token": true,
	}

	token := jwt.NewWithClaims(jwt.SigningMethodHS256, claims)
	return token.SignedString([]byte(cfg.JwtSecret))
}

func GetExpireTime() *time.Time {
	resetPasswordExpired := os.Getenv("RESET_PASSWORD_EXPIRATION")
	minute, err := strconv.Atoi(resetPasswordExpired)
	if err != nil {
		return nil
	}
	// กำหนดวันหมดอายุของ token เป็น 30 นาที
	expiresAt := time.Now().Add(time.Duration(minute) * time.Minute)
	return &expiresAt
}

func GenerateLoginResponse(token, refreshToken string) types.LoginResponse {
	accessTokenExpiry := os.Getenv("JWT_EXPIRATION") + "h"
	refreshTokenExpiry := os.Getenv("RESET_PASSWORD_EXPIRATION") + "h"

	return types.LoginResponse{
		AccessToken:        token,
		AccessTokenExpiry:  accessTokenExpiry,
		RefreshToken:       refreshToken,
		RefreshTokenExpiry: refreshTokenExpiry,
	}
}

func GenerateGuestRegisterResponse(token, refreshToken, accountId, userId string) types.GuestRegisterResponse {
	accessTokenExpiry := os.Getenv("JWT_EXPIRATION") + "h"
	refreshTokenExpiry := os.Getenv("RESET_PASSWORD_EXPIRATION") + "h"

	return types.GuestRegisterResponse{
		AccountId:          accountId,
		UserId:             userId,
		AccessToken:        token,
		AccessTokenExpiry:  accessTokenExpiry,
		RefreshToken:       refreshToken,
		RefreshTokenExpiry: refreshTokenExpiry,
	}
}
