package handler

import (
	"net/http"
	"strconv"
	"strings"

	"github.com/dgrijalva/jwt-go"
	"github.com/labstack/echo/v4"
	"github.com/pi-financial/go-common/result"
	"github.com/pi-financial/pi-sso-v2/config"
	constants "github.com/pi-financial/pi-sso-v2/const"
	"github.com/pi-financial/pi-sso-v2/internal/service/ssodb_service"
	"github.com/pi-financial/pi-sso-v2/internal/types"
	"github.com/pi-financial/pi-sso-v2/internal/util"
	"github.com/pi-financial/pi-sso-v2/middleware"
)

// Struct สำหรับรับข้อมูลที่ใช้ในการเปลี่ยนรหัสผ่าน
type ChangePasswordRequest struct {
	Username    string `json:"username" validate:"required"`
	OldPassword string `json:"encryptOldPassword"  validate:"required,password"`
	NewPassword string `json:"encryptNewPassword" validate:"required,password"`
}

type ResetPasswordRequest struct {
	Username string `json:"username" validate:"required,username"`
	IdCardNo string `json:"idCardNo"  validate:"required"`
	Birthday string `json:"birthday" validate:"required,birthday"`
}

type RequestResetPinRequest struct {
	Username string `json:"username" validate:"required,custcode"`
	IdCardNo string `json:"idCardNo"  validate:"required"`
	Birthday string `json:"birthday" validate:"required,birthday"`
}

type ResetPasswordGuestRequest struct {
	Username string `json:"username" validate:"required,email"`
}

type ResetPasswordBackOfficeRequest struct {
	Username string `json:"username" validate:"required"`
}

type ResetPinBackOfficeRequest struct {
	Username string `json:"username" validate:"required,custcode"`
}

type SetNewPasswordRequest struct {
	// Username    []string `json:"username" validate:"required"`
	Token       string `json:"token" validate:"required"`
	NewPassword string `json:"encryptNewPassword" validate:"required"`
}

type SetNewPinRequest struct {
	// Username []string `json:"username" validate:"required"`
	Token  string `json:"token" validate:"required"`
	NewPin string `json:"encryptNewPin" validate:"required"`
}

type ResetPinRequest struct {
	Password string   `json:"encryptPassword"  validate:"required,password"`
	Custcode []string `json:"custcode" validate:"required,dive,custcodes"`
	NewPin   string   `json:"encryptNewPin" validate:"required"`
}

type CreatePasswordRequest struct {
	Username    []string `json:"username" validate:"required"`
	NewPassword string   `json:"encryptNewPassword" validate:"required,password"`
}

type LinkAccountRequest struct {
	Custcode           *string `json:"custcode" validate:"omitempty,custcode"`
	SendLinkAccountId  *string `json:"sendLinkAccountId" validate:"omitempty"`
	EncryptNewPassword string  `json:"encryptNewPassword" validate:"required,password"`
}

type RsaRequest struct {
	Message string `json:"message" validate:"required"`
}

type VerifyPinRequest struct {
	Custcode string `json:"custcode" validate:"required,custcode"`
	Pin      string `json:"encryptPin" validate:"required"`
}

type LoginRequest struct {
	Username string `json:"username" validate:"required"`
	Password string `json:"encryptPassword" validate:"required"`
}

type EFinTradeLoginRequest struct {
	UserLogin string `json:"userlogin" validate:"required"`
	Password  string `json:"password" validate:"required"`
	Device    string `json:"device" validate:"required"`
	ClientID  string `json:"client_id" validate:"required"`
	IP        string `json:"ip" validate:"required"`
}

type AuthHandler struct {
	AuthService    ssodb_service.AuthService
	AccountService ssodb_service.AccountService
	Cfg            config.Config
	piMiddleware   middleware.PiMiddlewares
}

type VerifyTokenResponse struct {
	UserId string `json:"userId"`
}

func NewAuthHandler(e *echo.Echo, cfg config.Config, authService ssodb_service.AuthService, piMiddlewares *middleware.PiMiddlewares, accountService ssodb_service.AccountService) {
	handler := &AuthHandler{
		AuthService:    authService,
		AccountService: accountService,
		Cfg:            cfg,
		piMiddleware:   *piMiddlewares,
	}

	e.POST("/public/auth/login", handler.Login)
	e.POST("/public/auth/login/biometric", handler.LoginBiometric)
	e.POST("/secure/auth/login/biometric/register", handler.RegisterBiometric, piMiddlewares.JWTAccessTokenMiddleware)

	e.POST("/secure/auth/refresh-token", handler.RefreshToken, piMiddlewares.JWTRefreshTokenMiddleware)

	e.POST("/internal/auth/verify-pin", handler.VerifyPin, piMiddlewares.JWTAccessTokenMiddleware)

	e.POST("/internal/auth/change-password", handler.InternalChangePassword)
	e.POST("/internal/auth/changePasswordWithOtp", handler.InternalChangePasswordWithOtp)

	e.POST("/secure/auth/create-pin", handler.CreatePin, piMiddlewares.JWTAccessTokenMiddleware)

	e.POST("/secure/auth/change-password", handler.ChangePassword, piMiddlewares.JWTAccessTokenMiddleware)
	e.POST("/secure/auth/change-pin", handler.ChangePin, piMiddlewares.JWTAccessTokenMiddleware)

	e.POST("/secure/auth/reset-pin", handler.ResetPin, piMiddlewares.JWTAccessTokenMiddleware)

	e.POST("/internal/auth/request-reset-password/backOffice", handler.RequestPasswordBackOffice)
	e.POST("/public/auth/request-reset-password/guest", handler.RequestPasswordResetGuest)
	e.POST("/public/auth/request-reset-password", handler.RequestPasswordReset)
	e.POST("/public/auth/reset-new-password", handler.SetNewPassword)

	e.POST("/internal/auth/request-reset-pin/backOffice", handler.RequestPinBackOffice)
	e.POST("/public/auth/request-reset-pin", handler.RequestPinReset)
	e.POST("/public/auth/reset-new-pin", handler.SetNewPin)

	e.POST("/internal/auth/unlock-account", handler.UnlockAccount)
	e.GET("/internal/auth/verify/token", handler.VerifyToken, piMiddlewares.JWTAccessTokenMiddleware)

	// for old web trading
	e.POST("/secure/auth/otw/create-pin", handler.CreatePinOtw, piMiddlewares.JWTAccessTokenMiddleware)
	e.POST("/secure/auth/otw/change-password", handler.ChangePasswordOtw, piMiddlewares.JWTAccessTokenMiddleware)
	e.POST("/secure/auth/otw/change-pin", handler.ChangePinOtw, piMiddlewares.JWTAccessTokenMiddleware)
	e.POST("/secure/auth/otw/syncToken", handler.SyncTokenOtw, piMiddlewares.JWTAccessTokenMiddleware)
	e.GET("/internal/auth/otw/getSyncTokenById/:id", handler.GetSyncTokenById)
	e.GET("/internal/auth/otw/getLogSession/:id", handler.GetLogSessionById)
}

// Login godoc
// @Summary Login with username and password
// @Description Authenticate user with username and password and return JWT tokens
// @Tags auth
// @Accept  json
// @Produce  json
// @Param request body LoginRequest true "Login request body"
// @Success 200 {object} result.ResponseSuccess{data=types.LoginResponse} "Returns access and refresh tokens"
// @Failure 400 {object} result.ResponseError "Invalid request"
// @Failure 401 {object} result.ResponseError "Unauthorized"
// @Router /public/auth/login [post]
func (h *AuthHandler) Login(c echo.Context) error {
	var req LoginRequest
	if err := c.Bind(&req); err != nil {
		return result.ParamErrorResult(c, err)
	}

	// ตรวจสอบ username และ password ผ่าน service
	token, refreshToken, err := h.AuthService.Login(c.Request().Context(), strings.ToLower(req.Username), req.Password)
	response := util.GenerateLoginResponse(token, refreshToken)
	// ส่ง JWT token กลับ
	return result.HttpResult(c, &response, err)

}

// LoginBiometric godoc
// @Summary LoginBiometric with biometric and token
// @Description Authenticate user with biometric and return JWT tokens
// @Tags auth
// @Accept  json
// @Produce  json
// @Param request body types.LoginBiometricRequest true "Login request body"
// @Success 200 {object} result.ResponseSuccess{data=types.LoginResponse} "Returns access and refresh tokens"
// @Failure 400 {object} result.ResponseError "Invalid request"
// @Failure 401 {object} result.ResponseError "Unauthorized"
// @Router /public/auth/login/biometric [post]
func (h *AuthHandler) LoginBiometric(c echo.Context) error {
	var req types.LoginBiometricRequest
	if err := c.Bind(&req); err != nil {
		return result.ParamErrorResult(c, err)
	}

	token, refreshToken, err := h.AuthService.LoginBiometric(c.Request().Context(), req)

	// ส่ง JWT token กลับ
	return result.HttpResult(c, &types.LoginResponse{
		AccessToken:        *token,
		AccessTokenExpiry:  strconv.Itoa(h.Cfg.JwtExpiration) + "h",
		RefreshToken:       *refreshToken,
		RefreshTokenExpiry: strconv.Itoa(h.Cfg.RefreshExpiration) + "h",
	}, err)
}

// RegisterBiometric godoc
// @Summary RegisterBiometric with token
// @Description Register Biometric
// @Tags auth
// @Accept  json
// @Produce  json
// @Param Authorization header string true "Authorization"
// @Param deviceId header string true "deviceId"
// @Param request body types.RegisterBiometricRequest true "Register Biometric request body"
// @Success 200 {object} result.ResponseSuccess{data=types.RegisterBiometricResponse} "Returns biometric id"
// @Failure 400 {object} result.ResponseError "Invalid request, Invalid token, Decryption error"
// @Failure 401 {object} result.ResponseError "Unauthorized"
// @Security BearerAuth
// @Router /secure/auth/login/biometric/register [post]
func (h *AuthHandler) RegisterBiometric(c echo.Context) error {
	var req types.RegisterBiometricRequest

	// ดึง claims จาก context
	userClaims := c.Get("user").(jwt.MapClaims)
	userID := userClaims["user_id"].(string)
	accountID := userClaims["account_id"].(string)
	deviceId := c.Request().Header.Get("deviceId")

	if userID == "" || accountID == "" {
		return result.UnauthorizedErrorResult(c)
	}

	if err := c.Bind(&req); err != nil {
		return result.ParamErrorResult(c, err)
	}

	response := &types.RegisterBiometricResponse{}
	biometric, err := h.AuthService.RegisterBiometric(c.Request().Context(), req, deviceId, userID, accountID)
	if err == nil {
		response.Id = biometric.ID.String()
	}

	return result.HttpResult(c, response, err)
}

// RefreshToken godoc
// @Summary Refresh JWT tokens
// @Description Generate new access and refresh tokens using existing refresh token
// @Tags auth
// @Accept  json
// @Produce  json
// @Success 200 {object} map[string]string "Returns new access and refresh tokens"
// @Failure 401 {object} map[string]string "Unauthorized"
// @Security BearerAuth
// @Router /secure/auth/refresh-token [post]
func (h *AuthHandler) RefreshToken(c echo.Context) error {
	userClaims := c.Get("user").(jwt.MapClaims)
	userID := userClaims["user_id"].(string)
	accountID := userClaims["account_id"].(string)

	token, refreshToken, err := h.AuthService.RefreshToken(accountID, &userID)
	if err != nil {
		return ErrorResponseDetail(c, http.StatusUnauthorized, "E107")
	}

	response := util.GenerateLoginResponse(token, refreshToken)

	// ส่ง JWT token กลับ
	return c.JSON(http.StatusOK, response)
}

// ChangePassword godoc
// @Summary Change user's password
// @Description Change the password for the authenticated user by providing old and new password
// @Tags auth
// @Accept  json
// @Produce  json
// @Param Authorization header string true "Authorization"
// @Param request body ChangePasswordRequest true "Change Password Request Body"
// @Success 200 {object} map[string]string "Password changed successfully"
// @Failure 400 {object} ErrorResponse "Validation failed"
// @Failure 401 {object} map[string]string "Unauthorized"
// @Security BearerAuth
// @Router /secure/auth/change-password [post]
func (h *AuthHandler) ChangePassword(c echo.Context) error {
	// ดึง claims จาก context
	userClaims := c.Get("user").(jwt.MapClaims)

	// ดึง user_id จาก claims
	userID := userClaims["user_id"].(string)

	//accID := userClaims["account_id"].(string)

	var req ChangePasswordRequest
	if err := c.Bind(&req); err != nil {
		return result.ParamErrorResult(c, err)
	}

	decryptNewPassword, err := util.RsaDecryption(req.NewPassword)
	if err != nil {
		return result.ParamErrorResult(c, err)
	}

	req.NewPassword = decryptNewPassword

	decryptOldPassword, err := util.RsaDecryption(req.OldPassword)
	if err != nil {
		return result.ParamErrorResult(c, err)
	}

	req.OldPassword = decryptOldPassword

	// ตรวจสอบความถูกต้องของข้อมูล
	if err := c.Validate(req); err != nil {
		return ErrorResponseDetail(c, http.StatusBadRequest, "E101")
	}

	// เรียกใช้ service เพื่อตรวจสอบรหัสผ่านเก่าและเปลี่ยนรหัสผ่านใหม่
	err = h.AccountService.ChangePassword(c.Request().Context(), userID, req.Username, req.OldPassword, req.NewPassword)
	if err != nil {
		return c.JSON(http.StatusUnauthorized, map[string]string{"error": err.Error()})
	}

	return c.JSON(http.StatusOK, SuccessResponseDetail(c))
}

// RequestPasswordReset godoc
// @Summary Request Reset Password Link
// @Description request reset password email link
// @Tags auth
// @Accept  json
// @Produce  json
// @Param request body ResetPasswordRequest true "Reset Password Request Body"
// @Success 200 {object} map[string]string "Password reset link has been sent"
// @Failure 400 {object} ErrorResponse "Validation failed"
// @Failure 401 {object} map[string]string "Unauthorized"
// @Router /public/auth/request-reset-password [post]
func (h *AuthHandler) RequestPasswordReset(c echo.Context) error {
	var req ResetPasswordRequest
	if err := c.Bind(&req); err != nil {
		return result.ParamErrorResult(c, err)
	}

	// ตรวจสอบความถูกต้องของข้อมูล
	if err := c.Validate(req); err != nil {
		return result.ValidationErrorResult(c, err)
	}

	// ตรวจสอบ Username, IdCardNo และ Birthday ในระบบ
	user, err := h.AccountService.VerifyUserDetails(c.Request().Context(), req.Username, req.IdCardNo, req.Birthday)
	if err != nil {
		return result.HttpResult(c, nil, err)
	}

	err = h.AccountService.SendResetPasswordLink(c.Request().Context(), *user, req.Username)
	return result.HttpResult(c, nil, err)
}

// SetNewPassword RequestResetNewPassword godoc
// @Summary Request Reset New Password
// @Description request ResetNewPassword
// @Tags auth
// @Accept  json
// @Produce  json
// @Param request body SetNewPasswordRequest true "Password reset successfully"
// @Success 200 {object} map[string]string ""Password reset successfully"
// @Failure 400 {object} ErrorResponse "Validation failed"
// @Failure 401 {object} map[string]string "Unauthorized"
// @Router /public/auth/reset-new-password [post]
func (h *AuthHandler) SetNewPassword(c echo.Context) error {

	var req SetNewPasswordRequest
	if err := c.Bind(&req); err != nil {
		return result.ParamErrorResult(c, err)
	}

	decryptedPassword, err := util.RsaDecryption(req.NewPassword)
	if err != nil {
		return result.ParamErrorResult(c, err)
	}

	req.NewPassword = decryptedPassword

	// ตรวจสอบความถูกต้องของข้อมูล
	if err := c.Validate(req); err != nil {
		return result.ValidationErrorResult(c, err)
	}

	// reset password
	err = h.AccountService.ResetPassword(c.Request().Context(), req.Token, req.NewPassword)
	return result.HttpResult(c, nil, err)
}

// SetNewPin godoc
// @Summary Set New Pin
// @Description Set New Pin
// @Tags auth
// @Accept  json
// @Produce  json
// @Param request body SetNewPinRequest true "Pin reset successfully"
// @Success 200 {object} map[string]string ""Pin reset successfully"
// @Failure 400 {object} ErrorResponse "Validation failed"
// @Failure 401 {object} map[string]string "Unauthorized"
// @Router /public/auth/reset-new-pin [post]
// ถ้าตรวจสอบสำเร็จ ส่งลิงก์รีเซ็ตรหัสผ่าน
func (h *AuthHandler) SetNewPin(c echo.Context) error {
	var req SetNewPinRequest
	if err := c.Bind(&req); err != nil {
		return result.ParamErrorResult(c, err)
	}

	decryptedPin, err := util.RsaDecryption(req.NewPin)
	if err != nil {
		return result.ParamErrorResult(c, err)
	}

	req.NewPin = decryptedPin

	// ตรวจสอบความถูกต้องของข้อมูล
	if err := c.Validate(req); err != nil {
		return result.ValidationErrorResult(c, err)
	}

	// reset ResetPin
	err = h.AccountService.ResetPinByRequest(c.Request().Context(), req.Token, req.NewPin)
	return result.HttpResult(c, nil, err)
}

// ResetPin godoc
// @Summary Reset user's PIN
// @Description Reset the PIN for the authenticated user by verifying the password and custcode
// @Tags auth
// @Accept  json
// @Produce  json
// @Param Authorization header string true "Authorization"
// @Param request body ResetPinRequest true "Reset Pin Request Body"
// @Success 200 {object} map[string]string "Pin reset successfully"
// @Failure 400 {object} ErrorResponse "Validation failed"
// @Failure 401 {object} map[string]string "Unauthorized"
// @Security BearerAuth
// @Router /secure/auth/reset-pin [post]
func (h *AuthHandler) ResetPin(c echo.Context) error {
	// ดึง claims จาก context
	userClaims := c.Get("user").(jwt.MapClaims)

	// ดึง user_id จาก claims
	accountId := userClaims["account_id"].(string)

	var req ResetPinRequest
	if err := c.Bind(&req); err != nil {
		return result.ParamErrorResult(c, err)
	}

	decryptNewPin, err := util.RsaDecryption(req.NewPin)
	if err != nil {
		return result.ParamErrorResult(c, err)
	}

	req.NewPin = decryptNewPin

	decryptPassword, err := util.RsaDecryption(req.Password)
	if err != nil {
		return result.ParamErrorResult(c, err)
	}

	isSequencePin := util.HasSequenceNumbers(req.NewPin, 6)
	if isSequencePin {
		return result.ParamErrorResult(c, constants.ErrPinSequence)
	}

	req.Password = decryptPassword

	// ตรวจสอบความถูกต้องของข้อมูล
	if err := c.Validate(req); err != nil {
		return result.ValidationErrorResult(c, err)
	}

	// เรียกใช้ service
	err = h.AccountService.ResetPinByList(c.Request().Context(), req.Custcode, accountId, req.Password, req.NewPin)
	return result.HttpResult(c, nil, err)
}

// InternalChangePassword godoc
// @Summary Change user's password
// @Description Change the password for the authenticated user by verifying the user ID and username
// @Tags auth
// @Accept  json
// @Produce  json
// @Param request body types.InternalChangePasswordRequest true "Change Password Request Body"
// @Success 200 {object} result.ResponseSuccess "Password changed successfully"
// @Failure 400 {object} result.ResponseError "Bad request"
// @Failure 401 {object} result.ResponseError "Unauthorized"
// @Security BearerAuth
// @Router /internal/auth/change-password [post]
func (h *AuthHandler) InternalChangePassword(c echo.Context) error {
	var req types.InternalChangePasswordRequest
	if err := c.Bind(&req); err != nil {
		return result.ParamErrorResult(c, err)
	}

	if err := c.Validate(req); err != nil {
		return result.ValidationErrorResult(c, err)
	}

	err := h.AccountService.ChangePasswordByUserId(c.Request().Context(), req)

	return result.HttpResult(c, nil, err)
}

// ChangePin godoc
// @Summary Change user's PIN
// @Description Change the PIN for the authenticated user by verifying the old PIN and custcode
// @Tags auth
// @Accept  json
// @Produce  json
// @Param Authorization header string true "Authorization"
// @Param request body types.ChangePinRequest true "Change Pin Request Body"
// @Success 200 {object} result.ResponseSuccess "Pin changed successfully"
// @Failure 400 {object} result.ResponseError "Bad request"
// @Failure 401 {object} result.ResponseError "Unauthorized"
// @Security BearerAuth
// @Router /secure/auth/change-pin [post]
func (h *AuthHandler) ChangePin(c echo.Context) error {
	// ดึง claims จาก context
	userClaims := c.Get("user").(jwt.MapClaims)

	// ดึง user_id จาก claims
	userID, ok := userClaims["user_id"].(string)
	if !ok {
		return result.ParamErrorResult(c, constants.ErrUnauthorizedOrInvalidUser)
	}

	var req types.ChangePinRequest
	if err := c.Bind(&req); err != nil {
		return result.ParamErrorResult(c, constants.ErrInvalidRequest)
	}

	// ตรวจสอบความถูกต้องของข้อมูล
	if err := c.Validate(req); err != nil {
		return result.ParamErrorResult(c, constants.ErrValidationFailed)
	}

	// เรียกใช้ service เพื่อตรวจสอบรหัสผ่านเก่าและเปลี่ยนรหัสผ่านใหม่
	err := h.AccountService.ChangePin(c.Request().Context(), userID, req)

	return result.HttpResult(c, nil, err)
}

// VerifyPin godoc
// @Summary Verify user's PIN
// @Description Verify the PIN for the authenticated user by verifying the user ID and custcode
// @Tags auth
// @Accept  json
// @Produce  json
// @Param Authorization header string true "Authorization"
// @Param request body VerifyPinRequest true "Verify Pin Request Body"
// @Success 200 {object} map[string]string "Pin verified successfully"
// @Failure 400 {object} ErrorResponse "Validation failed"
// @Failure 401 {object} map[string]string "Unauthorized"
// @Security BearerAuth
// @Router /internal/auth/verify-pin [post]
func (h *AuthHandler) VerifyPin(c echo.Context) error {

	// ดึง claims จาก context
	userClaims := c.Get("user").(jwt.MapClaims)

	// ดึง user_id จาก claims
	userID, ok := userClaims["user_id"].(string)
	if !ok {
		return result.ParamErrorResult(c, constants.ErrUnauthorizedOrInvalidUser)
	}

	var req VerifyPinRequest
	if err := c.Bind(&req); err != nil {
		return result.ParamErrorResult(c, constants.ErrInvalidRequest)
	}

	decryptPin, err := util.RsaDecryption(req.Pin)
	if err != nil {
		return result.ParamErrorResult(c, constants.ErrDecrypt)
	}

	req.Pin = decryptPin

	// ตรวจสอบความถูกต้องของข้อมูล
	if err := c.Validate(req); err != nil {
		return result.ParamErrorResult(c, constants.ErrValidationFailed)
	}

	// เรียกใช้ service เพื่อตรวจสอบรหัสพิน
	err = h.AccountService.VerifyPin(c.Request().Context(), userID, req.Custcode, req.Pin)

	return result.HttpResult(c, nil, err)
}

// CreatePin godoc
// @Summary Create a new PIN for user
// @Description Create a new PIN for the authenticated user by verifying the user ID and custcode
// @Tags auth
// @Accept  json
// @Produce  json
// @Param Authorization header string true "Authorization"
// @Param request body types.CreatePinRequest true "Create Pin Request Body"
// @Success 200 {object} map[string]string "Pin created successfully"
// @Failure 400 {object} ErrorResponse "Validation failed"
// @Failure 401 {object} map[string]string "Unauthorized"
// @Security BearerAuth
// @Router /secure/auth/create-pin [post]
func (h *AuthHandler) CreatePin(c echo.Context) error {
	userClaims := c.Get("user").(jwt.MapClaims)
	userID, ok := userClaims["user_id"].(string)
	if !ok {
		return result.ParamErrorResult(c, constants.ErrUnauthorizedOrInvalidUser)
	}

	var req types.CreatePinRequest
	if err := c.Bind(&req); err != nil {
		return result.ParamErrorResult(c, constants.ErrInvalidRequest)
	}

	decryptPin, err := util.RsaDecryption(req.NewPin)
	if err != nil {
		return result.ParamErrorResult(c, constants.ErrDecrypt)
	}
	req.NewPin = decryptPin

	if err := c.Validate(req); err != nil {
		return result.ParamErrorResult(c, constants.ErrValidationFailed)
	}

	//"req.NewPin: %v\n", req.NewPin)

	isSequencePin := util.HasSequenceNumbers(req.NewPin, 6)
	if isSequencePin {
		return result.ParamErrorResult(c, constants.ErrPinSequence)
	}

	err = h.AccountService.CreatePin(c.Request().Context(), userID, req)
	return result.HttpResult(c, nil, err)
}

// UnlockAccount godoc
// @Summary Unlock a locked account
// @Description Unlock a locked account by providing the username
// @Tags auth
// @Accept  json
// @Produce  json
// @Param request body UnlockUserRequest true "Unlock Account Request Body"
// @Success 200 {object} map[string]string "Account unlocked successfully"
// @Failure 400 {object} ErrorResponse "Validation failed"
// @Failure 401 {object} map[string]string "Unauthorized"
// @Router /internal/auth/unlock-account [post]
func (h *AuthHandler) UnlockAccount(c echo.Context) error {
	var req = new(UnlockUserRequest)

	// ผูกข้อมูลที่ได้รับจาก request กับโครงสร้าง
	if err := c.Bind(req); err != nil {
		return result.ParamErrorResult(c, constants.ErrInvalidRequest)
	}

	// ตรวจสอบความถูกต้องของข้อมูล
	if err := c.Validate(req); err != nil {
		return result.ParamErrorResult(c, constants.ErrValidationFailed)
	}

	// เรียกใช้ service
	err := h.AuthService.UnlockAccount(c.Request().Context(), req.Username)
	return result.HttpResult(c, nil, err)
}

// VerifyToken godoc
// @Summary Verify Access Token
// @Description Verify Access Token for the authenticated user
// @Tags auth
// @Accept  json
// @Produce  json
// @Param Authorization header string true "Authorization"
// @Success 200 {object} result.ResponseSuccess{data=VerifyTokenResponse} "token verified successfully"
// @Failure 400 {object} ErrorResponse "Validation failed"
// @Failure 401 {object} map[string]string "Unauthorized"
// @Security BearerAuth
// @Router /internal/auth/verify/token [get]
func (h *AuthHandler) VerifyToken(c echo.Context) error {
	// ดึง claims จาก context
	userClaims := c.Get("user").(jwt.MapClaims)

	// ดึง user_id จาก claims
	userID, ok := userClaims["user_id"].(string)

	if !ok {
		return result.ParamErrorResult(c, constants.ErrUnauthorizedOrInvalidUser)
	}

	return result.HttpResult(c, VerifyTokenResponse{UserId: userID}, nil)
}

// RequestPasswordResetGuest godoc
// @Summary Request Reset Password Link
// @Description request reset password email link
// @Tags auth
// @Accept  json
// @Produce  json
// @Param request body ResetPasswordRequest true "Reset Password Request Body"
// @Success 200 {object} map[string]string "Password reset link has been sent"
// @Failure 400 {object} ErrorResponse "Validation failed"
// @Failure 401 {object} map[string]string "Unauthorized"
// @Router /public/auth/request-reset-password/guest [post]
func (h *AuthHandler) RequestPasswordResetGuest(c echo.Context) error {
	var req ResetPasswordGuestRequest
	if err := c.Bind(&req); err != nil {
		return result.ParamErrorResult(c, err)
	}

	// ตรวจสอบความถูกต้องของข้อมูล
	if err := c.Validate(req); err != nil {
		return result.ValidationErrorResult(c, err)
	}

	// ตรวจสอบ email ในระบบ
	user, err := h.AccountService.VerifyUserUsernameDetails(req.Username)
	if err != nil {
		return result.HttpResult(c, user, err)
	}

	err = h.AccountService.SendResetPasswordLink(c.Request().Context(), *user, req.Username)
	return result.HttpResult(c, nil, err)
}

// RequestPasswordBackOffice godoc
// @Summary Request Reset Password Link
// @Description request reset password email link
// @Tags auth
// @Accept  json
// @Produce  json
// @Param request body ResetPasswordRequest true "Reset Password Request Body"
// @Success 200 {object} map[string]string "Password reset link has been sent"
// @Failure 400 {object} ErrorResponse "Validation failed"
// @Failure 401 {object} map[string]string "Unauthorized"
// @Router /internal/auth/request-reset-password/backOffice [post]
func (h *AuthHandler) RequestPasswordBackOffice(c echo.Context) error {
	var req ResetPasswordBackOfficeRequest
	if err := c.Bind(&req); err != nil {
		return result.ParamErrorResult(c, err)
	}

	// ตรวจสอบความถูกต้องของข้อมูล
	if err := c.Validate(req); err != nil {
		return result.ValidationErrorResult(c, err)
	}

	// ตรวจสอบ Username, IdCardNo และ Birthday ในระบบ
	user, err := h.AccountService.VerifyUserUsernameDetails(req.Username)
	if err != nil {
		return result.HttpResult(c, nil, err)
	}

	err = h.AccountService.SendResetPasswordLink(c.Request().Context(), *user, req.Username)
	return result.HttpResult(c, nil, err)
}

// RequestPinReset godoc
// @Summary Request Reset Pin Link
// @Description request reset pin email link
// @Tags auth
// @Accept  json
// @Produce  json
// @Param request body ResetPasswordRequest true "Reset Password Request Body"
// @Success 200 {object} map[string]string "Pin reset link has been sent"
// @Failure 400 {object} ErrorResponse "Validation failed"
// @Failure 401 {object} map[string]string "Unauthorized"
// @Router /public/auth/request-reset-pin [post]
func (h *AuthHandler) RequestPinReset(c echo.Context) error {
	var req RequestResetPinRequest
	if err := c.Bind(&req); err != nil {
		return result.ParamErrorResult(c, err)
	}

	// ตรวจสอบความถูกต้องของข้อมูล
	if err := c.Validate(req); err != nil {
		return result.ValidationErrorResult(c, err)
	}

	// ตรวจสอบ Username, IdCardNo และ Birthday ในระบบ
	user, err := h.AccountService.VerifyUserDetails(c.Request().Context(), req.Username, req.IdCardNo, req.Birthday)
	if err != nil {
		return result.HttpResult(c, nil, err)
	}

	err = h.AccountService.SendResetPinLink(c.Request().Context(), *user, req.Username)
	return result.HttpResult(c, nil, err)
}

// RequestPinBackOffice godoc
// @Summary Request Reset Pin Link
// @Description request reset pin email link
// @Tags auth
// @Accept  json
// @Produce  json
// @Param request body ResetPasswordRequest true "Reset Password Request Body"
// @Success 200 {object} map[string]string "Pin reset link has been sent"
// @Failure 400 {object} ErrorResponse "Validation failed"
// @Failure 401 {object} map[string]string "Unauthorized"
// @Router /internal/auth/request-reset-pin/backOffice [post]
func (h *AuthHandler) RequestPinBackOffice(c echo.Context) error {
	var req ResetPinBackOfficeRequest
	if err := c.Bind(&req); err != nil {
		return result.ParamErrorResult(c, err)
	}

	// ตรวจสอบความถูกต้องของข้อมูล
	if err := c.Validate(req); err != nil {
		return result.ValidationErrorResult(c, err)
	}

	// ตรวจสอบ Username, IdCardNo และ Birthday ในระบบ
	user, err := h.AccountService.VerifyUserUsernameDetails(req.Username)
	if err != nil {
		return result.HttpResult(c, nil, err)
	}

	err = h.AccountService.SendResetPinLink(c.Request().Context(), *user, req.Username)
	return result.HttpResult(c, nil, err)
}

// InternalChangePasswordWithOtp godoc
// @Summary Change user's password with OTP
// @Description Change the password for the authenticated user by verifying the user ID and username
// @Tags auth
// @Accept  json
// @Produce  json
// @Param request body types.InternalChangePasswordRequestWithOtp true "Change Password Request Body"
// @Success 200 {object} result.ResponseSuccess "Password changed successfully"
// @Failure 400 {object} result.ResponseError "Bad request"
// @Failure 401 {object} result.ResponseError "Unauthorized"
// @Security BearerAuth
// @Router /internal/auth/changePasswordWithOtp [post]
func (h *AuthHandler) InternalChangePasswordWithOtp(c echo.Context) error {
	var req types.InternalChangePasswordRequestWithOtp
	if err := c.Bind(&req); err != nil {
		return result.ParamErrorResult(c, err)
	}

	if err := c.Validate(req); err != nil {
		return result.ValidationErrorResult(c, err)
	}
	decryptNewPassword, err := util.RsaDecryption(req.NewPassword)
	if err != nil {
		return result.ParamErrorResult(c, err)
	}

	err = h.AccountService.ChangePasswordByUserIdWithOtp(c.Request().Context(), req.EmailOtpRef, req.PhoneOtpRef, req.Username, decryptNewPassword)
	return result.HttpResult(c, nil, err)
}

// CreatePinOtw godoc
// @Summary Create a new PIN for user
// @Description Create a new PIN for the authenticated user by verifying the user ID and custcode
// @Tags auth
// @Accept  json
// @Produce  json
// @Param Authorization header string true "Authorization"
// @Param request body types.CreatePinRequest true "Create Pin Request Body"
// @Success 200 {object} map[string]string "Pin created successfully"
// @Failure 400 {object} ErrorResponse "Validation failed"
// @Failure 401 {object} map[string]string "Unauthorized"
// @Security BearerAuth
// @Router /secure/auth/otw/create-pin [post]
func (h *AuthHandler) CreatePinOtw(c echo.Context) error {
	userClaims := c.Get("user").(jwt.MapClaims)
	userID, ok := userClaims["user_id"].(string)
	if !ok {
		return result.ParamErrorResult(c, constants.ErrUnauthorizedOrInvalidUser)
	}

	var req types.CreatePinRequest
	if err := c.Bind(&req); err != nil {
		return result.ParamErrorResult(c, constants.ErrInvalidRequest)
	}

	if err := c.Validate(req); err != nil {
		return result.ParamErrorResult(c, constants.ErrValidationFailed)
	}

	err := h.AccountService.CreatePin(c.Request().Context(), userID, req)
	return result.HttpResult(c, nil, err)
}

// ChangePasswordOtw godoc
// @Summary Change user's password
// @Description Change the password for the authenticated user by verifying the user ID and username
// @Tags auth
// @Accept  json
// @Produce  json
// @Param request body types.InternalChangePasswordRequest true "Change Password Request Body"
// @Success 200 {object} result.ResponseSuccess "Password changed successfully"
// @Failure 400 {object} result.ResponseError "Bad request"
// @Failure 401 {object} result.ResponseError "Unauthorized"
// @Security BearerAuth
// @Router /secure/auth/otw/change-password [post]
func (h *AuthHandler) ChangePasswordOtw(c echo.Context) error {
	userClaims := c.Get("user").(jwt.MapClaims)

	// ดึง user_id จาก claims
	userID := userClaims["user_id"].(string)

	//accID := userClaims["account_id"].(string)

	var req ChangePasswordRequest
	if err := c.Bind(&req); err != nil {
		return result.ParamErrorResult(c, err)
	}

	// ตรวจสอบความถูกต้องของข้อมูล
	if err := c.Validate(req); err != nil {
		return result.ValidationErrorResult(c, err)
	}

	// เรียกใช้ service เพื่อตรวจสอบรหัสผ่านเก่าและเปลี่ยนรหัสผ่านใหม่
	err := h.AccountService.ChangePassword(c.Request().Context(), userID, req.Username, req.OldPassword, req.NewPassword)
	return result.HttpResult(c, nil, err)
}

// ChangePinOtw godoc
// @Summary Change user's PIN
// @Description Change the PIN for the authenticated user by verifying the old PIN and custcode
// @Tags auth
// @Accept  json
// @Produce  json
// @Param Authorization header string true "Authorization"
// @Param request body types.ChangePinRequest true "Change Pin Request Body"
// @Success 200 {object} result.ResponseSuccess "Pin changed successfully"
// @Failure 400 {object} result.ResponseError "Bad request"
// @Failure 401 {object} result.ResponseError "Unauthorized"
// @Security BearerAuth
// @Router /secure/auth/otw/change-pin [post]
func (h *AuthHandler) ChangePinOtw(c echo.Context) error {
	// ดึง claims จาก context
	userClaims := c.Get("user").(jwt.MapClaims)

	// ดึง user_id จาก claims
	userID, ok := userClaims["user_id"].(string)
	if !ok {
		return result.ParamErrorResult(c, constants.ErrUnauthorizedOrInvalidUser)
	}

	var req types.ChangePinOTWRequest
	if err := c.Bind(&req); err != nil {
		return result.ParamErrorResult(c, err)
	}

	isSequencePin := util.HasSequenceNumbers(req.NewPin, 6)
	if isSequencePin {
		return result.ParamErrorResult(c, constants.ErrPinSequence)
	}

	if err := c.Validate(req); err != nil {
		return result.ValidationErrorResult(c, err)
	}

	err := h.AccountService.ChangePinOtw(c.Request().Context(), userID, req)

	return result.HttpResult(c, nil, err)
}

// SyncTokenOtw godoc
// @Summary Sync Token
// @Description Sync Token for the authenticated user
// @Tags auth
// @Accept  json
// @Produce  json
// @Param Authorization header string true "Authorization"
// @Success 200 {object} result.ResponseSuccess{data=ssodb.SyncToken} "Token synced successfully"
// @Failure 400 {object} result.ResponseError "Bad request"
// @Failure 401 {object} result.ResponseError "Unauthorized"
// @Security BearerAuth
// @Router /secure/auth/otw/syncToken [post]
func (h *AuthHandler) SyncTokenOtw(c echo.Context) error {
	// ดึง claims จาก context
	userClaims := c.Get("user").(jwt.MapClaims)
	// ดึง user_id จาก claims
	userID, ok := userClaims["user_id"].(string)
	if !ok {
		return result.ParamErrorResult(c, constants.ErrUnauthorizedOrInvalidUser)
	}

	// ดีง acccount_id จาก claims
	accID, ok := userClaims["account_id"].(string)
	if !ok {
		return result.ParamErrorResult(c, constants.ErrUnauthorizedOrInvalidUser)
	}

	synctoken, err := h.AccountService.SyncTokenToOtw(c.Request().Context(), userID, accID)

	return result.HttpResult(c, synctoken, err)
}

// GetSyncTokenById godoc
// @Summary Get Sync Token By Id
// @Description Get Sync Token By Id
// @Tags auth
// @Accept  json
// @Produce  json
// @Param id path string true "id"
// @Success 200 {object} result.ResponseSuccess "Get Sync Token By Id"
// @Failure 400 {object} result.ResponseError "Bad request"
// @Failure 401 {object} result.ResponseError "Unauthorized"
// @Router /internal/auth/otw/getSyncTokenById/{id} [get]
func (h *AuthHandler) GetSyncTokenById(c echo.Context) error {
	id := c.Param("id")
	synctoken, err := h.AccountService.GetSyncToken(c.Request().Context(), id)

	return result.HttpResult(c, synctoken, err)
}

// GetLogSessionById godoc
// @Summary Get Log Session By Id
// @Description Get Log Session By Id
// @Tags auth
// @Accept  json
// @Produce  json
// @Param id path string true "id"
// @Success 200 {object} result.ResponseSuccess "Get Log Session By Id"
// @Failure 400 {object} result.ResponseError "Bad request"
// @Failure 401 {object} result.ResponseError "Unauthorized"
// @Router /internal/auth/otw/getLogSessionById/{id} [get]
func (h *AuthHandler) GetLogSessionById(c echo.Context) error {
	id := c.Param("id")
	logSession, err := h.AccountService.GetLogSession(c.Request().Context(), id)

	return result.HttpResult(c, logSession, err)
}
