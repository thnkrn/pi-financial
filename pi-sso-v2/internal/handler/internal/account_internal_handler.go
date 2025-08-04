package internal

import (
	"fmt"
	"html"
	"log"
	"net/url"
	"strconv"

	"github.com/labstack/echo/v4"
	"github.com/pi-financial/go-common/result"
	"github.com/pi-financial/pi-sso-v2/config"
	"github.com/pi-financial/pi-sso-v2/internal/service/ssodb_service"
	"github.com/pi-financial/pi-sso-v2/internal/types"
	"github.com/pi-financial/pi-sso-v2/middleware"
)

type InternalAccountHandler struct {
	AccountService ssodb_service.AccountService
	Cfg            config.Config
	PiMiddlewares  *middleware.PiMiddlewares
	AuthService    ssodb_service.AuthService
}

func NewInternalAccountHandler(e *echo.Echo, cfg config.Config, accountService ssodb_service.AccountService, piMiddlewares *middleware.PiMiddlewares, authService ssodb_service.AuthService) {
	handler := &InternalAccountHandler{
		AccountService: accountService,
		Cfg:            cfg,
		PiMiddlewares:  piMiddlewares,
		AuthService:    authService,
	}

	// Routing
	e.GET("/internal/accounts/userIdByCustomerCode/:customercode", handler.userIdByCustomerCode)
	// get info by customer code
	e.GET("/internal/accounts/customerInfoByCustomerCode/:customercode", handler.customerInfoByCustomerCode)
	e.GET("/internal/accounts/accountInfoByCustomerCode/:customercode", handler.accountInfoByCustomerCode)

	// update user info
	e.PUT("/internal/accounts/updateUserInfo", handler.updateUserInfo)

	// get account info all
	e.GET("/internal/accounts/accountInfoAll", handler.accountInfoLast50)
	// get account info by username
	e.GET("/internal/accounts/accountInfoByUsername/:username", handler.accountInfoByUsername)
	e.GET("/internal/accounts/accountInfoByUsernameOrPage", handler.accountInfoByUsernameOrPage)

	// force change password
	e.POST("/internal/accounts/forceChangePassword", handler.forceChangePassword)

	// force change pin
	e.POST("/internal/accounts/forceChangePin", handler.forceChangePin)

	// unlock pin
	e.POST("/internal/accounts/unlockPin", handler.unlockPin)
}

// userIdByCustomerCode godoc
// @Summary Get User ID by Customer Code
// @Description Retrieve user ID using customer code
// @Tags account
// @Accept  json
// @Produce  json
// @Param customercode path string true "Customer Code"
// @Success 200 {object} result.ResponseSuccess{data=types.UserIdByCustomerCodeResponse} "User ID by Customer Code"
// @Failure 400 {object} result.ResponseError "Bad Request"
// @Failure 404 {object} result.ResponseError "Not Found"
// @Failure 500 {object} result.ResponseError "Internal Server Error"
// @Router /internal/accounts/userIdByCustomerCode/{customercode} [get]
func (h *InternalAccountHandler) userIdByCustomerCode(c echo.Context) error {
	// ดึง customercode จาก path parameter
	customerCode := c.Param("customercode")
	if customerCode == "" {
		return result.ParamErrorResult(c, fmt.Errorf("customerCode is required"))
	}

	// เรียก service เพื่อค้นหา userId ตาม customercode
	response, err := h.AccountService.GetUserIdByCustomerCode(c.Request().Context(), customerCode)

	// ส่งผลลัพธ์ที่สำเร็จกลับไป
	return result.HttpResult(c, response, err)
}

// customerInfoByCustomerCode godoc
// @Summary Get Customer Info by Customer Code
// @Description Retrieve customer information using customer code
// @Tags account
// @Accept  json
// @Produce  json
// @Param customercode path string true "Customer Code"
// @Success 200 {object} result.ResponseSuccess{data=domain.User} "Customer Info by Customer Code"
// @Failure 400 {object} result.ResponseError "Bad Request"
// @Failure 404 {object} result.ResponseError "Not Found"
// @Failure 500 {object} result.ResponseError "Internal Server Error"
// @Router /internal/accounts/customerInfoByCustomerCode/{customercode} [get]
func (h *InternalAccountHandler) customerInfoByCustomerCode(c echo.Context) error {
	// ดึง customercode จาก path parameter
	customerCode := c.Param("customercode")
	if customerCode == "" {
		return result.ParamErrorResult(c, fmt.Errorf("customerCode is required"))
	}

	// Debug Logging
	log.Printf("Fetching customer info for customerCode: %v", customerCode)

	// เรียก service เพื่อดึงข้อมูล customerInfo
	response, err := h.AccountService.GetCustomerInfoByCustomerCode(c.Request().Context(), customerCode)
	// ส่งผลลัพธ์ที่สำเร็จกลับไป
	return result.HttpResult(c, response, err)
}

// updateUserInfo godoc
// @Summary Update User Info
// @Description Update user information
// @Tags account
// @Accept  json
// @Produce  json
// @Param custcode body types.UpdateUserInfoRequest true "Customer Code"
// @Success 200 {object} result.ResponseSuccess "User Info Updated"
// @Failure 400 {object} result.ResponseError "Bad Request"
// @Failure 404 {object} result.ResponseError "Not Found"
// @Failure 500 {object} result.ResponseError "Internal Server Error"
// @Router /internal/accounts/updateUserInfo [put]
func (h *InternalAccountHandler) updateUserInfo(c echo.Context) error {
	var req types.UpdateUserInfoRequest

	// ผูกข้อมูลที่ได้รับจาก request กับโครงสร้าง
	if err := c.Bind(&req); err != nil {
		return result.ParamErrorResult(c, err)
	}

	// ตรวจสอบความถูกต้องของข้อมูล
	if err := c.Validate(req); err != nil {
		return result.ValidationErrorResult(c, err)
	}

	// เรียก service เพื่ออัพเดทข้อมูล user
	user, err := h.AccountService.UpdateUserInfo(c.Request().Context(), req.Custcode)
	// ส่งผลลัพธ์ที่สำเร็จกลับไป
	return result.HttpResult(c, user, err)
}

// accountInfoByCustomerCode godoc
// @Summary Get Account Info by Customer Code
// @Description Retrieve account information using customer code
// @Tags account
// @Accept  json
// @Produce  json
// @Param customercode path string true "Customer Code"
// @Success 200 {object} result.ResponseSuccess{data=domain.User} "Account Info by Customer Code"
// @Failure 400 {object} result.ResponseError "Bad Request"
// @Failure 404 {object} result.ResponseError "Not Found"
// @Failure 500 {object} result.ResponseError "Internal Server Error"
// @Router /internal/accounts/accountInfoByCustomerCode/{customercode} [get]
func (h *InternalAccountHandler) accountInfoByCustomerCode(c echo.Context) error {
	// ดึง customercode จาก path parameter
	customerCode := c.Param("customercode")
	if customerCode == "" {
		return result.ParamErrorResult(c, fmt.Errorf("customerCode is required"))
	}

	// Debug Logging
	log.Printf("Fetching account info for customerCode: %v", customerCode)

	// เรียก service เพื่อดึงข้อมูล account info
	response, err := h.AccountService.GetAccountInfoByCustomerCode(c.Request().Context(), customerCode)
	// ส่งผลลัพธ์ที่สำเร็จกลับไป
	return result.HttpResult(c, response, err)
}

// accountInfoAll godoc
// @Summary Get Account Info All
// @Description Retrieve all account information
// @Tags account
// @Accept  json
// @Produce  json
// @Success 200 {object} result.ResponseSuccess{data=[]types.AccountInfoResponse} "Account Info All"
// @Failure 400 {object} result.ResponseError "Bad Request"
// @Failure 404 {object} result.ResponseError "Not Found"
// @Failure 500 {object} result.ResponseError "Internal Server Error"
// @Router /internal/accounts/accountInfoAll [get]
func (h *InternalAccountHandler) accountInfoLast50(c echo.Context) error {
	// Debug Logging
	log.Printf("Fetching all account info")

	// เรียก service เพื่อดึงข้อมูล account info ทั้งหมด
	response, err := h.AccountService.GetAccountInfoForAdminLast50(c.Request().Context())
	// ส่งผลลัพธ์ที่สำเร็จกลับไป
	return result.HttpResult(c, response, err)
}

// accountInfoByUsername godoc
// @Summary Get Account Info by Username
// @Description Retrieve account information using username
// @Tags account
// @Accept  json
// @Produce  json
// @Param username path string true "Username"
// @Success 200 {object} result.ResponseSuccess{data=types.PaginatedResponse} "Account Info by Username"
// @Failure 400 {object} result.ResponseError "Bad Request"
// @Failure 404 {object} result.ResponseError "Not Found"
// @Failure 500 {object} result.ResponseError "Internal Server Error"
// @Router /internal/accounts/accountInfoByUsername/{username} [get]
func (h *InternalAccountHandler) accountInfoByUsername(c echo.Context) error {
	// ดึง username จาก path parameter และแปลงค่า URL-encoded กลับเป็น string ปกติ
	rawUsername := c.Param("username")
	username, err := url.QueryUnescape(rawUsername)
	if err != nil {
		return result.ParamErrorResult(c, fmt.Errorf("invalid username encoding"))
	}

	// แปลง HTML-encoded characters (เช่น &amp; -> &)
	username = html.UnescapeString(username)

	if username == "" {
		return result.ParamErrorResult(c, fmt.Errorf("username is required"))
	}

	// Debug Logging
	log.Printf("Fetching account info for username: %v", username)

	// เรียก service เพื่อดึงข้อมูล account info ด้วย username
	response, err := h.AccountService.GetAccountInfoByUsername(c.Request().Context(), username)
	// ส่งผลลัพธ์ที่สำเร็จกลับไป
	return result.HttpResult(c, response, err)
}

// accountInfoByUsernameOrPage godoc
// @Summary Get Account Info by Username or Paginated List
// @Description Retrieve account information using username or fetch paginated list if username is not provided
// @Tags account
// @Accept  json
// @Produce  json
// @Param username query string false "Username"
// @Param page query int false "Page number (default 1)"
// @Param pageSize query int false "Page size (default 10)"
// @Success 200 {object} result.ResponseSuccess{data=types.PaginatedResponse} "Account Info or Paginated List"
// @Failure 400 {object} result.ResponseError "Bad Request"
// @Failure 500 {object} result.ResponseError "Internal Server Error"
// @Router /internal/accounts/accountInfoByUsernameOrPage [get]
func (h *InternalAccountHandler) accountInfoByUsernameOrPage(c echo.Context) error {
	// ดึงค่าจาก query parameters
	username := c.QueryParam("username")
	page := c.QueryParam("page")
	pageSize := c.QueryParam("pageSize")

	// กำหนดค่า default ให้ page และ pageSize หากไม่ได้ส่งมา
	pageInt, err := strconv.Atoi(page)
	if err != nil || pageInt < 1 {
		pageInt = 1
	}

	pageSizeInt, err := strconv.Atoi(pageSize)
	if err != nil || pageSizeInt < 1 {
		pageSizeInt = 10
	}

	response, err := h.AccountService.GetAccountInfoByUsernameOrPage(c.Request().Context(), username, pageInt, pageSizeInt)
	return result.HttpResult(c, response, err)
}

// forceChangePassword godoc
// @Summary Force Change Password
// @Description Force change password for user
// @Tags account
// @Accept  json
// @Produce  json
// @Param custcode body types.ForceChangePasswordRequest true "Customer Code"
// @Success 200 {object} result.ResponseSuccess "Password Changed"
// @Failure 400 {object} result.ResponseError "Bad Request"
// @Failure 404 {object} result.ResponseError "Not Found"
// @Failure 500 {object} result.ResponseError "Internal Server Error"
// @Router /internal/accounts/forceChangePassword [post]
func (h *InternalAccountHandler) forceChangePassword(c echo.Context) error {
	var req types.ForceChangePasswordRequest

	// ผูกข้อมูลที่ได้รับจาก request กับโครงสร้าง
	if err := c.Bind(&req); err != nil {
		return result.ParamErrorResult(c, err)
	}

	// ตรวจสอบความถูกต้องของข้อมูล
	if err := c.Validate(req); err != nil {
		return result.ValidationErrorResult(c, err)
	}

	// เรียก service เพื่อเปลี่ยน password
	err := h.AccountService.ForceChangePassword(c.Request().Context(), req)
	// ส่งผลลัพธ์ที่สำเร็จกลับไป
	return result.HttpResult(c, nil, err)
}

// forceChangePin godoc
// @Summary Force Change Pin
// @Description Force change pin for user
// @Tags account
// @Accept  json
// @Produce  json
// @Param custcode body types.ForceChangePinRequest true "Customer Code"
// @Success 200 {object} result.ResponseSuccess "Pin Changed"
// @Failure 400 {object} result.ResponseError "Bad Request"
// @Failure 404 {object} result.ResponseError "Not Found"
// @Failure 500 {object} result.ResponseError "Internal Server Error"
// @Router /internal/accounts/forceChangePin [post]
func (h *InternalAccountHandler) forceChangePin(c echo.Context) error {
	var req types.ForceChangePinRequest

	// ผูกข้อมูลที่ได้รับจาก request กับโครงสร้าง
	if err := c.Bind(&req); err != nil {
		return result.ParamErrorResult(c, err)
	}

	// ตรวจสอบความถูกต้องของข้อมูล
	if err := c.Validate(req); err != nil {
		return result.ValidationErrorResult(c, err)
	}

	// เรียก service เพื่อเปลี่ยน pin
	err := h.AccountService.ForceChangePin(c.Request().Context(), req)
	// ส่งผลลัพธ์ที่สำเร็จกลับไป
	return result.HttpResult(c, nil, err)
}

// unlockPin godoc
// @Summary Unlock Pin
// @Description Unlock pin for user
// @Tags account
// @Accept  json
// @Produce  json
// @Param custcode body types.UnlockPinRequest true "Customer Code"
// @Success 200 {object} result.ResponseSuccess "Pin Unlocked"
// @Failure 400 {object} result.ResponseError "Bad Request"
// @Failure 404 {object} result.ResponseError "Not Found"
// @Failure 500 {object} result.ResponseError "Internal Server Error"
// @Router /internal/accounts/unlockPin [post]
func (h *InternalAccountHandler) unlockPin(c echo.Context) error {
	var req types.UnlockPinRequest

	// ผูกข้อมูลที่ได้รับจาก request กับโครงสร้าง
	if err := c.Bind(&req); err != nil {
		return result.ParamErrorResult(c, err)
	}

	// ตรวจสอบความถูกต้องของข้อมูล
	if err := c.Validate(req); err != nil {
		return result.ValidationErrorResult(c, err)
	}

	// เรียก service เพื่อปลดล็อค pin
	err := h.AuthService.UnlockPin(c.Request().Context(), req.Username)

	// ส่งผลลัพธ์ที่สำเร็จกลับไป
	return result.HttpResult(c, nil, err)
}
