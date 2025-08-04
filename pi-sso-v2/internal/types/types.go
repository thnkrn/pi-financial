package types

import "time"

type ChangePinRequest struct {
	Custcode string `json:"custcode" validate:"required,custcode"`
	OldPin   string `json:"encryptOldPin" validate:"required"`
	NewPin   string `json:"encryptNewPin" validate:"required"`
}

type ChangePinOTWRequest struct {
	Custcode string `json:"custcode" validate:"required,custcode"`
	Password string `json:"password" validate:"required"`
	NewPin   string `json:"newpin" validate:"required"`
}

type InternalChangePasswordRequest struct {
	UserId      string   `json:"userId" validate:"required"`
	Username    []string `json:"username"`
	NewPassword string   `json:"encryptNewPassword" validate:"required"`
}

type TextEncryptRequest struct {
	Text string `json:"text" validate:"required"`
}

type TextEncryptResponse struct {
	Text string `json:"text" validate:"required"`
}

type MigrateGuestAccountRequest struct {
	Username string `json:"username" validate:"required,username"`
	Password string `json:"password" validate:"required"`
	UserId   string `json:"userId" validate:"required"`
}

type MigrateGuestAccountResponse struct {
	AccountId string `json:"accountId" validate:"required"`
}

type CheckSyncedPinResponse struct {
	Result bool `json:"result"`
}

type CreatePinRequest struct {
	Custcode []string `json:"custcode" validate:"custcodes,required"`
	NewPin   string   `json:"encryptNewPin" validate:"required"`
}

type UserIdResponse struct {
	Data string `json:"data"`
}

type CreateAccountRequest struct {
	Username string  `json:"username" validate:"required"`
	Password string  `json:"password" validate:"required"`
	Pin      *string `json:"pin"`
}
type AccountTradingSyncRequest struct {
	Username string  `json:"username" validate:"required"`
	Password *string `json:"password" validate:"omitempty"`
	Pin      *string `json:"pin" validate:"omitempty"`
}

type RegisterBiometricRequest struct {
	Token string `json:"token" validate:"required"`
}

type RegisterBiometricResponse struct {
	Id string `json:"id" validate:"required"`
}

type LoginBiometricRequest struct {
	Token    string `json:"token" validate:"required"`
	Username string `json:"username" validate:"required"`
}

type ForceChangePasswordRequest struct {
	Username    string `json:"username" validate:"required"`
	NewPassword string `json:"newPassword" validate:"required"`
}
type ForceChangePinRequest struct {
	Username string `json:"username" validate:"required"`
	NewPin   string `json:"newPin" validate:"required"`
}

type UnlockPinRequest struct {
	Username string `json:"username" validate:"required"`
}

type CheckSyncedPinRequest struct {
	Custcode string `json:"custcode" validate:"required,custcode"`
}

type GuestRegisterRequest struct {
	Username    string  `json:"username" validate:"required,email"`
	Password    string  `json:"encryptPassword" validate:"required"`
	PhoneNumber *string `json:"phoneNumber" validate:"omitempty,phone"`
}

type UpdateUserInfoRequest struct {
	Custcode string `json:"custcode" validate:"required"`
}

type GuestRegisterResponse struct {
	AccountId          string `json:"accountId" validate:"required"`
	UserId             string `json:"userId" validate:"required"`
	AccessToken        string `json:"accessToken" validate:"omitempty"`
	AccessTokenExpiry  string `json:"accessTokenExpiry" validate:"omitempty"`
	RefreshToken       string `json:"refreshToken" validate:"omitempty"`
	RefreshTokenExpiry string `json:"refreshTokenExpiry" validate:"omitempty"`
}

type GetAccountsQuery struct {
	Username string `query:"username"`
	UserId   string `query:"userId"`
}

type UserIdByCustomerCodeResponse struct {
	UserId string `json:"userId" validate:"required"`
}

type LoginWithOtpRequest struct {
	Username string `json:"username" validate:"required"`
	Password string `json:"encryptPassword" validate:"required"`
	DeviceId string `json:"deviceId" validate:"required"`
}

type EFinTradeLoginWithOtpRequest struct {
	UserLogin string `json:"userlogin" validate:"required"`
	Password  string `json:"password" validate:"required"`
	Device    string `json:"device" validate:"required"`
	ClientID  string `json:"client_id" validate:"required"`
	IP        string `json:"ip" validate:"required"`
}

type EFinTradeLoginWithOtpResponse struct {
	User           string `json:"user"`
	Account        string `json:"account"`
	Efin           string `json:"efin"`
	Autotrade      string `json:"autotrade"`
	PhoneNumber    string `json:"phone_number"`
	OtpGenerateKey string `json:"otp_generate_key"`
	RequireOtp     string `json:"require_otp"`
}

type GenerateOtpResponse struct {
	RefCode      string `json:"ref_code"`
	VerifyOtpKey string `json:"verify_otp_key"`
}

type GenerateOtpRequest struct {
	OtpGenerateKey string `json:"otp_generate_key" validate:"required"`
}

type VerifyOtpKeyRequest struct {
	VerifyOtpKey string `json:"verify_otp_key" validate:"required"`
	RefCode      string `json:"ref_code" validate:"required"`
	OtpCode      string `json:"otp_code" validate:"required"`
}

type VerifyOtpRequest struct {
	Pin string `json:"pin" validate:"required"`
}

type SendLinkAccountRequest struct {
	Email    *string `json:"email" validate:"omitempty,email"`
	Custcode string  `json:"custcode" validate:"required,custcode"`
}

type AccountInfoResponse struct {
	ID                string  `json:"id"`
	Username          string  `json:"username"`
	IsSyncPassword    bool    `json:"isSyncPassword"`
	IsSyncPin         bool    `json:"isSyncPin"`
	LoginPwdFailCount int     `json:"loginPwdFailCount"`
	LoginPinFailCount int     `json:"loginPinFailCount"`
	IsLock            bool    `json:"isLock"`
	UpdatedAt         string  `json:"updatedAt"`
	CreatedAt         string  `json:"createdAt"`
	UserID            *string `json:"userId"`
	Email             *string `json:"email"`
	Mobile            *string `json:"mobile"`
	CardID            *string `json:"cardId"`
}

type PinAccountInfoList struct {
	Username string `json:"username"`
	IsSetPin bool   `json:"isSetPin"`
}

type UserList struct {
	Username []string `json:"username"`
}

type PaginatedResponse struct {
	CurrentPage     int                   `json:"currentPage"`
	PageSize        int                   `json:"pageSize"`
	HasNextPage     bool                  `json:"hasNextPage"`
	HasPreviousPage bool                  `json:"hasPreviousPage"`
	TotalPages      int                   `json:"totalPages"`
	Data            []AccountInfoResponse `json:"data"`
}

type GeneratePhoneOtpForSetupRequest struct {
	PhoneNumber       string  `json:"phoneNumber" validate:"required,phone"`
	SendLinkAccountId *string `json:"sendLinkAccountId" validate:"omitempty"`
}

type GenerateEmailOtpForSetupRequest struct {
	Email string `json:"email" validate:"required,email"`
}

type VerifyPhoneOtpForSetupRequest struct {
	PhoneNumber string `json:"phoneNumber" validate:"required,phone"`
	RefCode     string `json:"refCode" validate:"required"`
	OtpCode     string `json:"otpCode" validate:"required"`
}

type GenerateEmailOtpForSetupResponse struct {
	RefCode   string    `json:"refCode" validate:"required"`
	Email     string    `json:"email" validate:"required"`
	ExpiresAt time.Time `json:"expiresAt" validate:"required"`
}

type GeneratePhoneOtpForSetupResponse struct {
	RefCode     string    `json:"refCode" validate:"required"`
	PhoneNumber string    `json:"phoneNumber" validate:"required"`
	ExpiresAt   time.Time `json:"expiresAt" validate:"required"`
}

type VerifyPhoneOtpForSetupResponse struct {
	RefId string `json:"refId" validate:"required"`
}

type VerifyEmailOtpForSetupRequest struct {
	Email   string `json:"email" validate:"required"`
	RefCode string `json:"refCode" validate:"required"`
	OtpCode string `json:"otpCode" validate:"required"`
}

type VerifyEmailOtpForSetupResponse struct {
	RefId string `json:"refId" validate:"required"`
}

type SetupWithOTPRequest struct {
	EmailRefID      string `json:"emailRefId" validate:"required"`
	Email           string `json:"email" validate:"required"`
	PhoneRefID      string `json:"phoneRefId" validate:"required"`
	PhoneNumber     string `json:"phoneNumber" validate:"required"`
	EncryptPassword string `json:"encryptPassword" validate:"required"`
}

type LoginResponse struct {
	AccessToken        string `json:"accessToken" validate:"omitempty"`
	AccessTokenExpiry  string `json:"accessTokenExpiry" validate:"omitempty"`
	RefreshToken       string `json:"refreshToken" validate:"omitempty"`
	RefreshTokenExpiry string `json:"refreshTokenExpiry" validate:"omitempty"`
}

type ErrorResponse struct {
	Type    string `json:"type"`
	Title   string `json:"title"`
	Status  int    `json:"status"`
	Detail  string `json:"detail"`
	TraceID string `json:"traceId"`
}

type CheckEmailAccountByUserIDRequest struct {
	UserId string `json:"userId" validate:"required"`
}

type CheckEmailAccountByUserIDResponse struct {
	Email             string `json:"email" validate:"required"`
	IsUsernameExisted bool   `json:"isUsernameExisted" validate:"required"`
}

type InternalChangePasswordRequestWithOtp struct {
	EmailOtpRef *string `json:"emailOtpRef" validate:"omitempty"`
	PhoneOtpRef *string `json:"phoneOtpRef" validate:"omitempty"`
	Username    string  `json:"username" validate:"required"`
	NewPassword string  `json:"encryptNewPassword" validate:"required"`
}

type ChangeUsernameRequest struct {
	Username    string `json:"username" validate:"required,email"`
	NewUsername string `json:"newUsername" validate:"required,email"`
	UserId      string `json:"userId" validate:"required"`
}

type HashTextRequest struct {
	Msg string `json:"msg" validate:"required"`
}
