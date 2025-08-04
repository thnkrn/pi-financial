package handler

import (
	"errors"
	"strings"

	"github.com/pi-financial/pi-sso-v2/internal/log"
	"go.uber.org/zap"

	"github.com/dgrijalva/jwt-go"
	"github.com/labstack/echo/v4"
	"github.com/pi-financial/go-common/result"
	"github.com/pi-financial/pi-sso-v2/config"
	"github.com/pi-financial/pi-sso-v2/internal/domain/ssodb"
	"github.com/pi-financial/pi-sso-v2/internal/service/ssodb_service"
	"github.com/pi-financial/pi-sso-v2/internal/types"
	"github.com/pi-financial/pi-sso-v2/middleware"
)

type AccountHandler struct {
	AccountService ssodb_service.AccountService
	Cfg            config.Config
	PiMiddlewares  *middleware.PiMiddlewares
	logger         log.Logger
}

type CheckDuplicateRequest struct {
	Username    *string `json:"username" validate:"omitempty,email"`
	PhoneNumber *string `json:"phoneNumber" validate:"omitempty,phone"`
}

type CheckDuplicateResponse struct {
	Username    *bool `json:"username" validate:"omitempty"`
	PhoneNumber *bool `json:"phoneNumber" validate:"omitempty"`
}

type UnlockUserRequest struct {
	Username string `json:"username" validate:"required"`
}

type ErrorResponse struct {
	Message string `json:"message"`
	Error   string `json:"error"`
}

type SuccessResponse struct {
	Message string `json:"message"`
}

func NewAccountHandler(e *echo.Echo, cfg config.Config, accountService ssodb_service.AccountService, piMiddlewares *middleware.PiMiddlewares, logger log.Logger) {
	handler := &AccountHandler{
		AccountService: accountService,
		Cfg:            cfg,
		PiMiddlewares:  piMiddlewares,
		logger:         logger,
	}

	// Route สำหรับการดึงข้อมูลบัญชีทั้งหมด
	e.GET("/internal/accounts", handler.GetAccounts)

	// Route สำหรับการสร้างบัญชีใหม่
	e.POST("/internal/accounts/trading", handler.CreateAccountByTrading)
	e.POST("/internal/accounts/trading/sync", handler.AccountTradingSync)

	e.POST("/public/accounts/guest/register", handler.GuestRegister)

	e.POST("/internal/accounts/sendLinkAccount", handler.SendLinkAccount)
	e.GET("/internal/accounts/link/:id", handler.GetLinkAccount)
	e.POST("/secure/accounts/link", handler.LinkAccount, piMiddlewares.JWTAccessTokenMiddleware)

	e.POST("/public/rsa_decrypt", handler.RsaDecryption)
	e.POST("/public/rsa_encrypt", handler.RsaEncryption)
	e.POST("/public/hash:msg", handler.hashText)

	e.POST("/public/text_encrypt", handler.TextEncrypt)
	e.POST("/public/text_decrypt", handler.TextDecrypt)

	e.POST("/internal/accounts/check-duplicate", handler.checkDuplicateAccount)
	e.POST("/internal/accounts/guest/migration", handler.migrateGuestAccount)

	e.GET("/internal/accounts/checkEmailAccountByUserID/:userID", handler.checkEmailAccountByUserID)

	e.POST("/secure/accounts/check-synced-pin", handler.CheckSyncedPin, piMiddlewares.JWTAccessTokenMiddleware)
	e.POST("/internal/accounts/check-synced-pin", handler.CheckSyncedPinByInternal)
	e.GET("/internal/accounts/send-link-account/:custcode", handler.GetSendLinkAccountByCustcode)

	e.POST("/public/accounts/generateEmailOtpForSetup", handler.generateEmailOtpForSetup)
	e.POST("/public/accounts/verifyEmailOtpForSetup", handler.verifyEmailOtpForSetup)
	e.POST("/public/accounts/generatePhoneOtpForSetup", handler.generatePhoneOtpForSetup)
	e.POST("/public/accounts/verifyPhoneOtpForSetup", handler.verifyPhoneOtpForSetup)

	e.POST("/public/accounts/generateEmailOtpForForgotPassword", handler.generateEmailOtpForForgotPassword)
	e.POST("/public/accounts/generatePhoneOtpForForgotPassword", handler.generatePhoneOtpForForgotPassword)

	e.POST("/public/accounts/setupWithOTP", handler.setupWithOTP)

	e.GET("/internal/accounts/accountWithoutPin", handler.AccountWithoutPin)
	e.GET("/internal/accounts/accountWithoutPin/:userId", handler.AccountWithoutPinByUserId)

	e.GET("/secure/accounts/profile", handler.Profile, piMiddlewares.JWTAccessTokenMiddleware)

	e.PUT("/internal/accounts/changeUsername", handler.ChangeUsername)

}

// GetAccounts godoc
// @Summary Get all accounts
// @Description Get all accounts
// @Tags account
// @Accept  json
// @Produce  json
// @Param request query types.GetAccountsQuery true "Get Accounts Query"
// @Success 200 {object} result.ResponseSuccess{data=[]ssodb.Account} "Get all accounts"
// @Failure 500 {object} result.ResponseError "Failed to retrieve accounts"
// @Router /internal/accounts [get]
func (h *AccountHandler) GetAccounts(c echo.Context) error {
	var query types.GetAccountsQuery
	if err := c.Bind(&query); err != nil {
		return result.ParamErrorResult(c, err)
	}

	accounts, err := h.AccountService.GetAllAccounts(query)
	return result.HttpResult(c, accounts, err)
}

// CreateAccountByTrading - สร้างบัญชีใหม่จากข้อมูล Trading
func (h *AccountHandler) CreateAccountByTrading(c echo.Context) error {
	var req types.CreateAccountRequest

	// ตรวจสอบข้อมูลที่ส่งเข้ามา
	if err := c.Bind(&req); err != nil {
		return result.ParamErrorResult(c, err)
	}

	// เรียก service เพื่อสร้างบัญชี
	err := h.AccountService.CreateAccountByTradingV2(c.Request().Context(), req)

	return result.HttpResult(c, nil, err)

}

// GuestRegister godoc
// @Summary Register a new guest account
// @Description Create a new guest account with username, password, and phone number
// @Tags account
// @Accept  json
// @Produce  json
// @Param request body types.GuestRegisterRequest true "Guest Register Request Body"
// @Success 200 {object} result.ResponseSuccess{data=types.GuestRegisterResponse} "Guest registered successfully"
// @Failure 400 {object} result.ResponseError "Validation failed"
// @Failure 500 {object} result.ResponseError "Failed to register guest"
// @Router /public/accounts/guest/register [post]
func (h *AccountHandler) GuestRegister(c echo.Context) error {
	var req types.GuestRegisterRequest

	// ผูกข้อมูลที่ได้รับจาก request กับโครงสร้าง
	if err := c.Bind(&req); err != nil {
		return result.ParamErrorResult(c, err)
	}

	// ตรวจสอบความถูกต้องของข้อมูล
	if err := c.Validate(req); err != nil {
		return result.ValidationErrorResult(c, err)
	}

	// เรียกใช้ service เพื่อสร้างบัญชีใหม่
	response, err := h.AccountService.RegisterGuest(c.Request().Context(), req)
	return result.HttpResult(c, response, err)
}

// GetLinkAccount godoc
// @Summary Get Link an account to a member
// @Description Get Link the authenticated user's account to a member using id
// @Tags account
// @Accept  json
// @Produce  json
// @Param id path string true "Link Account ID"
// @Success 200 {object} result.ResponseSuccess{data=ssodb.SendLinkAccount} "Link Account To Member created successfully"
// @Failure 400 {object} result.ResponseError "Validation failed"
// @Router /internal/accounts/link/{id} [get]
func (h *AccountHandler) GetLinkAccount(c echo.Context) error {
	sendLinkAccountId := c.Param("id")

	// ตรวจสอบความถูกต้องของข้อมูล
	if sendLinkAccountId == "" {
		return result.ValidationErrorResult(c, errors.New("id is required"))
	}

	resp, err := h.AccountService.GetLinkAccount(sendLinkAccountId)

	return result.HttpResult(c, resp, err)
}

// LinkAccount godoc
// @Summary Link an account to a member
// @Description Link the authenticated user's account to a member using custcode
// @Tags account
// @Accept  json
// @Produce  json
// @Param Authorization header string true "Authorization"
// @Param request body LinkAccountRequest true "Link Account Request Body"
// @Success 200 {object} result.ResponseSuccess{data=ssodb.SendLinkAccount} "Link Account To Member created successfully"
// @Failure 400 {object} result.ResponseError "Validation failed"
// @Failure 401 {object} result.ResponseError "Unauthorized or Invalid user, please login again"
// @Security BearerAuth
// @Router /secure/accounts/link [post]
func (h *AccountHandler) LinkAccount(c echo.Context) error {
	// ดึง claims จาก context
	userClaims := c.Get("user").(jwt.MapClaims)

	// ดึง user_id จาก claims
	accountID, ok := userClaims["account_id"].(string)
	if !ok {
		return result.UnauthorizedErrorResult(c)
	}

	var req LinkAccountRequest
	if err := c.Bind(&req); err != nil {
		return result.ParamErrorResult(c, err)
	}

	// ตรวจสอบความถูกต้องของข้อมูล
	if err := c.Validate(req); err != nil {
		return result.ValidationErrorResult(c, err)
	}

	resp, err := h.AccountService.LinkAccount(c.Request().Context(), accountID, req.SendLinkAccountId, req.Custcode, req.EncryptNewPassword)

	return result.HttpResult(c, resp, err)
}

// checkDuplicateAccount godoc
// @Summary Check Email and PhoneNumber Duplication
// @Description use internally to check for existing username and mobile phone
// @Tags account
// @Accept  json
// @Produce  json
// @Param request body CheckDuplicateRequest true "Check Duplicate Request"
// @Success 200 {object} result.ResponseSuccess{data=CheckDuplicateResponse} "Check Duplicate"
// @Failure 400 {object} result.ResponseError "Validation failed"
// @Failure 500 {object} result.ResponseError "Internal Server Error"
// @Router /internal/accounts/check-duplicate [post]
func (h *AccountHandler) checkDuplicateAccount(c echo.Context) error {
	var req CheckDuplicateRequest

	if err := c.Bind(&req); err != nil {
		return result.ParamErrorResult(c, err)
	}

	// ตรวจสอบความถูกต้องของข้อมูล
	if err := c.Validate(req); err != nil {
		return result.ValidationErrorResult(c, err)
	}

	initialFalse := false
	response := &CheckDuplicateResponse{
		Username:    &initialFalse,
		PhoneNumber: &initialFalse,
	}

	// Check if `req.Username` is not nil
	if req.Username != nil {
		exists := h.AccountService.IsUsernameExisted(*req.Username)
		response.Username = &exists
	}

	// Check if `req.PhoneNumber` is not nil
	if req.PhoneNumber != nil {
		exists := h.AccountService.IsPhoneNumberExisted(c.Request().Context(), *req.PhoneNumber)
		response.PhoneNumber = &exists
	}

	return result.HttpResult(c, response, nil)
}

// migrateGuestAccount godoc
// @Summary Migrate Guest Account
// @Description use internally to migrate guest account
// @Tags account
// @Accept  json
// @Produce  json
// @Param request body types.MigrateGuestAccountRequest true "Migrate Guest Account Request"
// @Success 200 {object} result.ResponseSuccess{data=types.MigrateGuestAccountResponse} "Migrate Guest Account"
// @Failure 400 {object} result.ResponseError "Bad Request"
// @Failure 500 {object} result.ResponseError "Internal Server Error"
// @Router /internal/accounts/guest/migration [post]
func (h *AccountHandler) migrateGuestAccount(c echo.Context) error {
	var req types.MigrateGuestAccountRequest

	if err := c.Bind(&req); err != nil {
		return result.ParamErrorResult(c, err)
	}

	// ตรวจสอบความถูกต้องของข้อมูล
	if err := c.Validate(req); err != nil {
		return result.ValidationErrorResult(c, err)
	}

	response := &types.MigrateGuestAccountResponse{}
	account, err := h.AccountService.MigrateGuest(c.Request().Context(), req)
	if err == nil {
		response.AccountId = account.ID.String()
	}

	return result.HttpResult(c, response, err)
}

// CheckSyncedPin godoc
// @Summary Check Sync Pin
// @Description Check Sync Pin
// @Tags account
// @Accept  json
// @Produce  json
// @Param request body types.CheckSyncedPinRequest true "Check Sync Pin Request"
// @Success 200 {object} result.ResponseSuccess{data=types.CheckSyncedPinResponse} "Check Sync Pin"
// @Failure 400 {object} result.ResponseError "Bad Request"
// @Failure 500 {object} result.ResponseError "Internal Server Error"
// @Security BearerAuth
// @Router /secure/accounts/check-synced-pin [post]
func (h *AccountHandler) CheckSyncedPin(c echo.Context) error {

	// ดึง claims จาก context
	userClaims := c.Get("user").(jwt.MapClaims)

	// ดึง user_id จาก claims
	userId := userClaims["user_id"].(string)

	var req types.CheckSyncedPinRequest

	if err := c.Bind(&req); err != nil {
		return result.ParamErrorResult(c, err)
	}

	// ตรวจสอบความถูกต้องของข้อมูล
	if err := c.Validate(req); err != nil {
		return result.ValidationErrorResult(c, err)
	}
	response, err := h.AccountService.CheckSyncedPin(c.Request().Context(), req, userId)

	return result.HttpResult(c, response, err)

}

// CheckSyncedPinByInternal godoc
// @Summary Check Sync Pin
// @Description Check Sync Pin
// @Tags account
// @Accept  json
// @Produce  json
// @Param request body types.CheckSyncedPinRequest true "Check Sync Pin Request"
// @Success 200 {object} result.ResponseSuccess{data=types.CheckSyncedPinResponse} "Check Sync Pin"
// @Failure 400 {object} result.ResponseError "Bad Request"
// @Failure 500 {object} result.ResponseError "Internal Server Error"
// @Router /internal/accounts/check-synced-pin [post]
func (h *AccountHandler) CheckSyncedPinByInternal(c echo.Context) error {
	var req types.CheckSyncedPinRequest

	if err := c.Bind(&req); err != nil {
		return result.ParamErrorResult(c, err)
	}

	// ตรวจสอบความถูกต้องของข้อมูล
	if err := c.Validate(req); err != nil {
		return result.ValidationErrorResult(c, err)
	}
	response, err := h.AccountService.CheckSyncedPinByInternal(c.Request().Context(), req)

	return result.HttpResult(c, response, err)

}

// RsaDecryption godoc
// @Summary Decrypt a message using RSA
// @Description Decrypt the provided message using RSA encryption
// @Tags account
// @Accept  json
// @Produce  json
// @Param request body RsaRequest true "Rsa Request"
// @Success 200 {object} result.ResponseSuccess{data=string} "Decrypted"
// @Failure 400 {object} result.ResponseError "Validation failed"
// @Failure 500 {object} result.ResponseError "Internal Server Error"
// @Router /public/rsa_decrypt [post]
func (h *AccountHandler) RsaDecryption(c echo.Context) error {
	// ดึง claims จาก context
	var req RsaRequest
	if err := c.Bind(&req); err != nil {
		return result.ParamErrorResult(c, err)
	}

	// ตรวจสอบความถูกต้องของข้อมูล
	if err := c.Validate(req); err != nil {
		return result.ValidationErrorResult(c, err)
	}

	decrypted, err := h.AccountService.RsaDecryption(req.Message)
	return result.HttpResult(c, decrypted, err)
}

// RsaEncryption godoc
// @Summary Encrypt a message using RSA
// @Description Encrypt the provided message using RSA encryption
// @Tags account
// @Accept  json
// @Produce  json
// @Param request body RsaRequest true "Rsa Request"
// @Success 200 {object} result.ResponseSuccess{data=string} "Encrypted"
// @Failure 400 {object} result.ResponseError "Validation failed"
// @Failure 500 {object} result.ResponseError "Internal Server Error"
// @Router /public/rsa_encrypt [post]
func (h *AccountHandler) RsaEncryption(c echo.Context) error {

	// ดึง claims จาก context
	var req RsaRequest
	if err := c.Bind(&req); err != nil {
		return result.ParamErrorResult(c, err)
	}

	// ตรวจสอบความถูกต้องของข้อมูล
	if err := c.Validate(req); err != nil {
		return result.ValidationErrorResult(c, err)
	}

	encrypted, err := h.AccountService.RsaEncryption(req.Message)
	return result.HttpResult(c, encrypted, err)
}

// AccountTradingSync godoc
// @Summary Sync Account Trading
// @Description Sync Account Trading
// @Tags account
// @Accept  json
// @Produce  json
// @Param request body types.AccountTradingSyncRequest true "Account Trading Sync Request"
// @Success 200 {object} result.ResponseSuccess{data=nil} "Sync Account Trading"
// @Failure 400 {object} result.ResponseError "Bad Request"
// @Failure 500 {object} result.ResponseError "Internal Server Error"
// @Router /internal/accounts/trading/sync [post]
func (h *AccountHandler) AccountTradingSync(c echo.Context) error {
	var req types.AccountTradingSyncRequest

	// ตรวจสอบข้อมูลที่ส่งเข้ามา
	if err := c.Bind(&req); err != nil {
		return result.ParamErrorResult(c, err)
	}

	// เรียก service เพื่อสร้างบัญชี
	err := h.AccountService.AccountTradingSync(c.Request().Context(), req)

	return result.HttpResult(c, nil, err)

}

// SendLinkAccount godoc
// @Summary Send Link Account
// @Description Send Link Account
// @Tags account
// @Accept  json
// @Produce  json
// @Param request body types.SendLinkAccountRequest true "Send Link Account Request"
// @Success 200 {object} result.ResponseSuccess{data=nil} "Send Link Account"
// @Failure 400 {object} result.ResponseError "Bad Request"
// @Failure 500 {object} result.ResponseError "Internal Server Error"
// @Router /internal/accounts/sendLinkAccount [post]
func (h *AccountHandler) SendLinkAccount(c echo.Context) error {
	defer func() {
		if r := recover(); r != nil {
			h.logger.Error(c.Request().Context(), "accountHandler.SendLinkAccount Panic recovered", zap.Any("recovery", r))
		}
	}()
	var req types.SendLinkAccountRequest

	// ตรวจสอบข้อมูลที่ส่งเข้ามา
	if err := c.Bind(&req); err != nil {
		return result.ParamErrorResult(c, err)
	}

	// ตรวจสอบความถูกต้องของข้อมูล
	if err := c.Validate(req); err != nil {
		return result.ValidationErrorResult(c, err)
	}

	// เรียก service เพื่อสร้างบัญชี
	err := h.AccountService.SendAndResendLinkAccount(c.Request().Context(), req)

	return result.HttpResult(c, nil, err)

}

// GetSendLinkAccountByCustcode godoc
// @Summary Get Send Link Account By Custcode
// @Description Get Send Link Account By Custcode
// @Tags account
// @Accept  json
// @Produce  json
// @Param custcode path string true "Custcode"
// @Success 200 {object} result.ResponseSuccess{data=ssodb.SendLinkAccount} "Get Send Link Account By Custcode"
// @Failure 400 {object} result.ResponseError "Bad Request"
// @Failure 500 {object} result.ResponseError "Internal Server Error"
// @Router /internal/accounts/send-link-account/{custcode} [get]
func (h *AccountHandler) GetSendLinkAccountByCustcode(c echo.Context) error {
	custcode := c.Param("custcode")
	if custcode == "" {
		return result.ParamErrorResult(c, nil)
	}

	response, err := h.AccountService.GetSendLinkAccountByCustcode(c.Request().Context(), custcode)
	return result.HttpResult(c, response, err)
}

// generateEmailOtpForSetup godoc
// @Summary Generate OTP For Setup
// @Description Generate OTP For Setup
// @Tags account
// @Accept  json
// @Produce  json
// @Param request body types.GenerateEmailOtpForSetupRequest true "Generate OTP For Setup Request"
// @Success 200 {object} result.ResponseSuccess{data=types.GenerateEmailOtpForSetupResponse} "Generate OTP For Setup"
// @Failure 400 {object} result.ResponseError "Bad Request"
// @Failure 500 {object} result.ResponseError "Internal Server Error"
// @Router /public/accounts/generateEmailOtpForSetup [post]
func (h *AccountHandler) generateEmailOtpForSetup(c echo.Context) error {
	var req types.GenerateEmailOtpForSetupRequest

	// ตรวจสอบข้อมูลที่ส่งเข้ามา
	if err := c.Bind(&req); err != nil {
		return result.ParamErrorResult(c, err)
	}

	// ตรวจสอบความถูกต้องของข้อมูล
	if err := c.Validate(req); err != nil {
		return result.ValidationErrorResult(c, err)
	}

	response, err := h.AccountService.GenerateOtpToEmailForSetup(c.Request().Context(), strings.ToLower(req.Email))
	return result.HttpResult(c, response, err)
}

// verifyEmailOtpForSetup godoc
// @Summary Verify OTP For Setup
// @Description Verify OTP For Setup
// @Tags account
// @Accept  json
// @Produce  json
// @Param request body types.VerifyEmailOtpForSetupRequest true "Verify Email OTP For Setup Request"
// @Success 200 {object} result.ResponseSuccess{data=types.VerifyEmailOtpForSetupResponse} "Verify OTP For Setup"
// @Failure 400 {object} result.ResponseError "Bad Request"
// @Failure 500 {object} result.ResponseError "Internal Server Error"
// @Router /public/accounts/verifyEmailOtpForSetup [post]
func (h *AccountHandler) verifyEmailOtpForSetup(c echo.Context) error {
	var req types.VerifyEmailOtpForSetupRequest

	// ตรวจสอบข้อมูลที่ส่งเข้ามา
	if err := c.Bind(&req); err != nil {
		return result.ParamErrorResult(c, err)
	}

	// ตรวจสอบความถูกต้องของข้อมูล
	if err := c.Validate(req); err != nil {
		return result.ValidationErrorResult(c, err)
	}

	response, err := h.AccountService.VerifySetupWithOTP(c.Request().Context(), strings.ToLower(req.Email), req.RefCode, req.OtpCode)
	return result.HttpResult(c, response, err)
}

// setupWithOTP godoc
// @Summary Setup With OTP
// @Description Setup With OTP
// @Tags account
// @Accept  json
// @Produce  json
// @Param request body types.SetupWithOTPRequest true "Setup With OTP Request"
// @Success 200 {object} result.ResponseSuccess{data=types.LoginResponse} "Setup With OTP"
// @Failure 400 {object} result.ResponseError "Bad Request"
// @Failure 500 {object} result.ResponseError "Internal Server Error"
// @Router /public/accounts/setupWithOTP [post]
func (h *AccountHandler) setupWithOTP(c echo.Context) error {
	var req types.SetupWithOTPRequest

	// ตรวจสอบข้อมูลที่ส่งเข้ามา
	if err := c.Bind(&req); err != nil {
		return result.ParamErrorResult(c, err)
	}

	// ตรวจสอบความถูกต้องของข้อมูล
	if err := c.Validate(req); err != nil {
		return result.ValidationErrorResult(c, err)
	}

	response, err := h.AccountService.SetupWithOTP(c.Request().Context(), req.EmailRefID, strings.ToLower(req.Email), req.PhoneRefID, req.PhoneNumber, req.EncryptPassword)
	return result.HttpResult(c, response, err)
}

// generatePhoneOtpForSetup godoc
// @Summary Generate Phone OTP For Setup
// @Description Generate Phone OTP For Setup
// @Tags account
// @Accept  json
// @Produce  json
// @Param request body types.GeneratePhoneOtpForSetupRequest true "Generate Phone OTP For Setup Request"
// @Success 200 {object} result.ResponseSuccess{data=types.GeneratePhoneOtpForSetupResponse} "Generate Phone OTP For Setup"
// @Failure 400 {object} result.ResponseError "Bad Request"
// @Failure 500 {object} result.ResponseError "Internal Server Error"
// @Router /public/accounts/generatePhoneOtpForSetup [post]
func (h *AccountHandler) generatePhoneOtpForSetup(c echo.Context) error {
	var req types.GeneratePhoneOtpForSetupRequest

	// ตรวจสอบข้อมูลที่ส่งเข้ามา
	if err := c.Bind(&req); err != nil {
		return result.ParamErrorResult(c, err)
	}

	// ตรวจสอบความถูกต้องของข้อมูล
	if err := c.Validate(req); err != nil {
		return result.ValidationErrorResult(c, err)
	}

	response, err := h.AccountService.GenerateOtpToPhoneForSetup(c.Request().Context(), req.PhoneNumber, req.SendLinkAccountId)
	return result.HttpResult(c, response, err)
}

// generatePhoneOtpForForgotPassword godoc
// @Summary Generate Phone OTP For Forgot Password
// @Description Generate Phone OTP For Forgot Password
// @Tags account
// @Accept  json
// @Produce  json
// @Param request body types.GeneratePhoneOtpForSetupRequest true "Generate Phone OTP For Setup Request"
// @Success 200 {object} result.ResponseSuccess{data=types.GeneratePhoneOtpForSetupResponse} "Generate Phone OTP For Setup"
// @Failure 400 {object} result.ResponseError "Bad Request"
// @Failure 500 {object} result.ResponseError "Internal Server Error"
// @Router /public/accounts/generatePhoneOtpForForgotPassword [post]
func (h *AccountHandler) generatePhoneOtpForForgotPassword(c echo.Context) error {
	var req types.GeneratePhoneOtpForSetupRequest

	// ตรวจสอบข้อมูลที่ส่งเข้ามา
	if err := c.Bind(&req); err != nil {
		return result.ParamErrorResult(c, err)
	}

	// ตรวจสอบความถูกต้องของข้อมูล
	if err := c.Validate(req); err != nil {
		return result.ValidationErrorResult(c, err)
	}

	response, err := h.AccountService.GenerateOtpToPhoneForForgotPassword(c.Request().Context(), req.PhoneNumber, req.SendLinkAccountId)
	return result.HttpResult(c, response, err)
}

// verifyPhoneOtpForSetup godoc
// @Summary Verify Phone OTP For Setup
// @Description Verify Phone OTP For Setup
// @Tags account
// @Accept  json
// @Produce  json
// @Param request body types.VerifyPhoneOtpForSetupRequest true "Verify Phone OTP For Setup Request"
// @Success 200 {object} result.ResponseSuccess{data=types.VerifyPhoneOtpForSetupResponse} "Verify Phone OTP For Setup"
// @Failure 400 {object} result.ResponseError "Bad Request"
// @Failure 500 {object} result.ResponseError "Internal Server Error"
// @Router /public/accounts/verifyPhoneOtpForSetup [post]
func (h *AccountHandler) verifyPhoneOtpForSetup(c echo.Context) error {
	var req types.VerifyPhoneOtpForSetupRequest

	// ตรวจสอบข้อมูลที่ส่งเข้ามา
	if err := c.Bind(&req); err != nil {
		return result.ParamErrorResult(c, err)
	}

	// ตรวจสอบความถูกต้องของข้อมูล
	if err := c.Validate(req); err != nil {
		return result.ValidationErrorResult(c, err)
	}

	response, err := h.AccountService.VerifyPhoneOtpForSetup(c.Request().Context(), req.PhoneNumber, req.RefCode, req.OtpCode)
	return result.HttpResult(c, response, err)
}

// generateEmailOtpForForgotPassword godoc
// @Summary Generate OTP For Forgot Password
// @Description Generate OTP For Forgot Password
// @Tags account
// @Accept  json
// @Produce  json
// @Param request body types.GenerateEmailOtpForSetupRequest true "Generate OTP For Setup Request"
// @Success 200 {object} result.ResponseSuccess{data=types.GenerateEmailOtpForSetupResponse} "Generate OTP For Setup"
// @Failure 400 {object} result.ResponseError "Bad Request"
// @Failure 500 {object} result.ResponseError "Internal Server Error"
// @Router /public/accounts/generateEmailOtpForForgotPassword [post]
func (h *AccountHandler) generateEmailOtpForForgotPassword(c echo.Context) error {
	var req types.GenerateEmailOtpForSetupRequest

	// ตรวจสอบข้อมูลที่ส่งเข้ามา
	if err := c.Bind(&req); err != nil {
		return result.ParamErrorResult(c, err)
	}

	// ตรวจสอบความถูกต้องของข้อมูล
	if err := c.Validate(req); err != nil {
		return result.ValidationErrorResult(c, err)
	}

	response, err := h.AccountService.GenerateOtpToEmailForForgotPassword(c.Request().Context(), strings.ToLower(req.Email))
	return result.HttpResult(c, response, err)
}

// checkEmailAccountByUserID godoc
// @Summary Check Email Account By UserID
// @Description Check Email Account By UserID
// @Tags account
// @Accept  json
// @Produce  json
// @Param userID path string true "userID"
// @Success 200 {object} result.ResponseSuccess{data=types.CheckEmailAccountByUserIDResponse} "Check Email Account By UserID"
// @Failure 400 {object} result.ResponseError "Bad Request"
// @Failure 500 {object} result.ResponseError "Internal Server Error"
// @Router /internal/accounts/checkEmailAccountByUserID/{userID} [get]
func (h *AccountHandler) checkEmailAccountByUserID(c echo.Context) error {
	userID := c.Param("userID")
	if userID == "" {
		return result.ParamErrorResult(c, nil)
	}

	response, err := h.AccountService.FindEmailAccountByUserID(c.Request().Context(), userID)
	return result.HttpResult(c, response, err)
}

// AccountWithoutPin godoc
// @Summary List Accounts Without Pin
// @Description List Accounts Without Pin
// @Tags account
// @Accept  json
// @Produce  json
// @Success 200 {object} result.ResponseSuccess{data=types.UserList} "List Accounts Without Pin"
// @Failure 500 {object} result.ResponseError "Internal Server Error"
// @Router /internal/accounts/accountWithoutPin [get]
func (h *AccountHandler) AccountWithoutPin(c echo.Context) error {
	response, err := h.AccountService.AccountWithoutPin(c.Request().Context())
	return result.HttpResult(c, response, err)
}

// AccountWithoutPinByUserId godoc
// @Summary List Accounts Without Pin By UserID
// @Description List Accounts Without Pin By UserID
// @Tags account
// @Accept  json
// @Produce  json
// @Param userId path string true "userId"
// @Success 200 {object} result.ResponseSuccess{data=[]types.PinAccountInfoList} "List Accounts Without Pin By UserID"
// @Failure 400 {object} result.ResponseError "Bad Request"
// @Failure 500 {object} result.ResponseError "Internal Server Error"
// @Router /internal/accounts/accountWithoutPin/{userId} [get]
func (h *AccountHandler) AccountWithoutPinByUserId(c echo.Context) error {
	userID := c.Param("userId")
	if userID == "" {
		return result.ParamErrorResult(c, nil)
	}

	response, err := h.AccountService.AccountWithoutPinByUserId(c.Request().Context(), userID)
	return result.HttpResult(c, response, err)
}

// TextEncrypt godoc
// @Summary Encrypt Text
// @Description Encrypt Text
// @Tags account
// @Accept  json
// @Produce  json
// @Param request body types.TextEncryptRequest true "Text Encrypt Request"
// @Success 200 {object} result.ResponseSuccess{data=types.TextEncryptResponse} "Encrypt Text"
// @Failure 400 {object} result.ResponseError "Bad Request"
// @Failure 500 {object} result.ResponseError "Internal Server Error"
// @Router /public/text_encrypt [post]
func (h *AccountHandler) TextEncrypt(c echo.Context) error {
	var req types.TextEncryptRequest

	// ตรวจสอบข้อมูลที่ส่งเข้ามา
	if err := c.Bind(&req); err != nil {
		return result.ParamErrorResult(c, err)
	}

	// ตรวจสอบความถูกต้องของข้อมูล
	if err := c.Validate(req); err != nil {
		return result.ValidationErrorResult(c, err)
	}

	response, err := h.AccountService.TextEncrypt(c.Request().Context(), req.Text)
	return result.HttpResult(c, response, err)
}

// TextDecrypt godoc
// @Summary Decrypt Text
// @Description Decrypt Text
// @Tags account
// @Accept  json
// @Produce  json
// @Param request body types.TextEncryptRequest true "Text Decrypt Request"
// @Success 200 {object} result.ResponseSuccess{data=types.TextEncryptResponse} "Decrypt Text"
// @Failure 400 {object} result.ResponseError "Bad Request"
// @Failure 500 {object} result.ResponseError "Internal Server Error"
// @Router /public/text_decrypt [post]
func (h *AccountHandler) TextDecrypt(c echo.Context) error {
	var req types.TextEncryptRequest

	// ตรวจสอบข้อมูลที่ส่งเข้ามา
	if err := c.Bind(&req); err != nil {
		return result.ParamErrorResult(c, err)
	}

	// ตรวจสอบความถูกต้องของข้อมูล
	if err := c.Validate(req); err != nil {
		return result.ValidationErrorResult(c, err)
	}

	response, err := h.AccountService.TextDecrypt(c.Request().Context(), req.Text)
	return result.HttpResult(c, response, err)
}

// Profile godoc
// @Summary Get Profile
// @Description Get Profile
// @Tags account
// @Accept  json
// @Produce  json
// @Success 200 {object} result.ResponseSuccess{data=ssodb.Account} "Get Profile"
// @Failure 500 {object} result.ResponseError "Internal Server Error"
// @Security BearerAuth
// @Router /secure/accounts/profile [get]
func (h *AccountHandler) Profile(c echo.Context) error {
	// ดึง claims จาก context
	userClaims := c.Get("user").(jwt.MapClaims)

	// ดึง user_id จาก claims
	userId := userClaims["user_id"].(string)

	response, err := h.AccountService.GetProfile(c.Request().Context(), userId)
	return result.HttpResult(c, response, err)
}

// hashText godoc
// @Summary Hash Text
// @Description Hash Text
// @Tags account
// @Accept  json
// @Produce  json
// @Param request body types.HashTextRequest true "Hash Text Request"
// @Success 200 {object} result.ResponseSuccess{data=string} "Hash Text"
// @Failure 400 {object} result.ResponseError "Bad Request"
// @Failure 500 {object} result.ResponseError "Internal Server Error"
// @Router /public/hash:msg [post]
func (h *AccountHandler) hashText(c echo.Context) error {
	msg := c.Param("msg")

	// ตรวจสอบความถูกต้องของข้อมูล
	if msg == "" {
		return result.ValidationErrorResult(c, errors.New("id is required"))
	}

	hash := ssodb.HashUsername(msg)
	return result.HttpResult(c, hash, nil)
}

// ChangeUsername ChangeEmailUsername godoc
// @Summary Change Email Username
// @Description Change Email Username
// @Tags account
// @Accept  json
// @Produce  json
// @Param request body types.ChangeUsernameRequest true "Change Username Request"
// @Success 200 {object} result.ResponseSuccess{data=ssodb.Account} "Change Username"
// @Failure 400 {object} result.ResponseError "Bad Request"
// @Failure 500 {object} result.ResponseError "Internal Server Error"
// @Router /internal/accounts/changUsername [put]
func (h *AccountHandler) ChangeUsername(c echo.Context) error {
	var req types.ChangeUsernameRequest

	// ตรวจสอบข้อมูลที่ส่งเข้ามา
	if err := c.Bind(&req); err != nil {
		return result.ParamErrorResult(c, err)
	}

	// ตรวจสอบความถูกต้องของข้อมูล
	if err := c.Validate(req); err != nil {
		return result.ValidationErrorResult(c, err)
	}
	response, err := h.AccountService.ChangeUsername(c.Request().Context(), strings.ToLower(req.Username), strings.ToLower(req.NewUsername), req.UserId)
	return result.HttpResult(c, response, err)
}
