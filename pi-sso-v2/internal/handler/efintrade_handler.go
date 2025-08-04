package handler

import (
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

type EfinTradeHandler struct {
	AuthService    ssodb_service.AuthService
	AccountService ssodb_service.AccountService
	Cfg            config.Config
	piMiddleware   middleware.PiMiddlewares
}

func NewEfinTradeHandler(e *echo.Echo, cfg config.Config, authService ssodb_service.AuthService, piMiddlewares *middleware.PiMiddlewares, accountService ssodb_service.AccountService) {
	handler := &EfinTradeHandler{
		AuthService:    authService,
		AccountService: accountService,
		Cfg:            cfg,
		piMiddleware:   *piMiddlewares,
	}

	e.POST("/public/auth/eFinTrade/login", handler.Login)
	e.POST("/public/auth/eFinTrade/generateOtp", handler.GenerateOtp)
	e.POST("/public/auth/eFinTrade/verifyOtp", handler.VerifyOtp)
	e.POST("/public/auth/eFinTrade/verifyPin", handler.VerifyPin, piMiddlewares.JWTAccessTokenMiddleware)

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
func (h *EfinTradeHandler) Login(c echo.Context) error {
	var req types.EFinTradeLoginWithOtpRequest
	if err := c.Bind(&req); err != nil {
		return result.ParamErrorResult(c, err)
	}

	//req.ClientID ไม่เท่ากับ nil และต้องเป็น type uuid
	if req.ClientID != "" && !util.IsValidUUID(req.ClientID) {
		return result.ParamErrorResult(c, constants.ErrInvalidClientID)
	}
	// ตรวจสอบ username และ password ผ่าน service
	response, err := h.AuthService.LoginWithOTP(c.Request().Context(), strings.ToLower(req.UserLogin), req.Password, req.ClientID)

	// ส่ง JWT token กลับ
	return result.HttpResult(c, response, err)
}

// GenerateOtp godoc
// @Summary Generate OTP
// @Description Generate OTP for user
// @Tags auth
// @Accept  json
// @Produce  json
// @Param request body types.GenerateOtpRequest true "Generate OTP request body"
// @Success 200 {object} result.ResponseSuccess{data=types.GenerateOtpResponse} "Returns OTP"
// @Failure 400 {object} result.ResponseError "Invalid request"
// @Failure 401 {object} result.ResponseError "Unauthorized"
// @Router /public/auth/generateOtp [post]
func (h *EfinTradeHandler) GenerateOtp(c echo.Context) error {
	var req types.GenerateOtpRequest
	if err := c.Bind(&req); err != nil {
		return result.ParamErrorResult(c, err)
	}

	// สร้าง OTP และส่งไปยังผู้ใช้
	response, err := h.AuthService.GenerateOtp(c.Request().Context(), req.OtpGenerateKey)

	// ส่ง OTP กลับ
	return result.HttpResult(c, response, err)
}

// VerifyOtp godoc
// @Summary Verify OTP
// @Description Verify OTP for user
// @Tags auth
// @Accept  json
// @Produce  json
// @Param request body types.VerifyOtpKeyRequest true "Verify OTP request body"
// @Success 200 {object} result.ResponseSuccess{data=types.LoginResponse} "Returns OTP"
// @Failure 400 {object} result.ResponseError "Invalid request"
// @Failure 401 {object} result.ResponseError "Unauthorized"
// @Router /public/auth/verifyOtp [post]
func (h *EfinTradeHandler) VerifyOtp(c echo.Context) error {
	var req types.VerifyOtpKeyRequest
	if err := c.Bind(&req); err != nil {
		return result.ParamErrorResult(c, err)
	}

	token, refreshToken, err := h.AuthService.VerifyOtpKey(c.Request().Context(), req.VerifyOtpKey, req.OtpCode)

	// ส่ง JWT token กลับ
	return result.HttpResult(c, &types.LoginResponse{
		AccessToken:        token,
		AccessTokenExpiry:  strconv.Itoa(h.Cfg.JwtExpiration) + "h",
		RefreshToken:       refreshToken,
		RefreshTokenExpiry: strconv.Itoa(h.Cfg.RefreshExpiration) + "h",
	}, err)

}

// VerifyPin godoc
// @Summary Verify Pin
// @Description Verify Pin for user
// @Tags auth
// @Accept  json
// @Produce  json
// @Param request body types.VerifyOtpRequest true "Verify Pin request body"
// @Success 200 {object} result.ResponseSuccess "Returns OTP"
// @Failure 400 {object} result.ResponseError "Invalid request"
// @Failure 401 {object} result.ResponseError "Unauthorized"
// @Router /public/auth/verifyPin [post]
func (h *EfinTradeHandler) VerifyPin(c echo.Context) error {

	// ดึง claims จาก context
	userClaims := c.Get("user").(jwt.MapClaims)

	// ดึง user_id จาก claims
	accountID, ok := userClaims["account_id"].(string)
	if !ok {
		return constants.ErrUnauthorizedOrInvalidUser
	}

	var req types.VerifyOtpRequest
	if err := c.Bind(&req); err != nil {
		return result.ParamErrorResult(c, constants.ErrInvalidRequest)
	}

	// ตรวจสอบความถูกต้องของข้อมูล
	if err := c.Validate(req); err != nil {
		return result.ValidationErrorResult(c, constants.ErrInvalidRequest)
	}

	account, err := h.AccountService.AccountRepo.FindById(accountID)
	if err != nil {
		return result.ValidationErrorResult(c, constants.ErrUnauthorizedOrInvalidUser)
	}

	// เรียกใช้ service เพื่อตรวจสอบรหัสพิน
	err = h.AccountService.VerifyPin(c.Request().Context(), *account.UserID, account.Username, req.Pin)

	return result.HttpResult(c, nil, err)
}
