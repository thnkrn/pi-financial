package handler

import (
	"errors"
	"strings"

	"github.com/go-playground/validator/v10"
	"github.com/labstack/echo/v4"
	"github.com/pi-financial/go-common/errorx"
	"github.com/pi-financial/go-common/logger"
	"github.com/pi-financial/go-common/result"
	"github.com/pi-financial/user-srv-v2/internal/domain"
	"github.com/pi-financial/user-srv-v2/internal/dto"
	serviceinterface "github.com/pi-financial/user-srv-v2/internal/service/interfaces"
	"github.com/pi-financial/user-srv-v2/internal/utils"
)

type UserAccountHandler struct {
	UserAccountService serviceinterface.UserAccountService
	Log                logger.Logger
}

func NewUserAccountHandler(
	userAccountService serviceinterface.UserAccountService,
	log logger.Logger) *UserAccountHandler {
	return &UserAccountHandler{
		UserAccountService: userAccountService,
		Log:                log,
	}
}

// LinkUserAccount godoc
//
//	@Summary		Link user account id with user id for a user account type.
//	@Description	Link (upsert) user account id with user id for a user account type. User account id can be either customer code or cash wallet id.
//	@Tags			user-account
//	@Accept			json
//	@Produce		json
//	@Param			user-id	header		string								true	"User ID"
//	@Param			request	body		dto.LinkUserAccountRequest			true	"Link User Account Request"
//	@Success		200		{object}	result.ResponseSuccess{data=nil}	"Linked User Account"
//	@Failure		400		{object}	result.ResponseError				"Validation failed"
//	@Failure		500		{object}	result.ResponseError				"Error linking user account"
//	@Router			/internal/v1/user-account [post]
func (h *UserAccountHandler) LinkUserAccount(c echo.Context) error {
	var req dto.LinkUserAccountRequest
	if err := c.Bind(&req); err != nil {
		return result.ParamErrorResult(c, err)
	}

	// Normalize account status case
	req.Status = (domain.UserAccountStatus)(strings.ToUpper(strings.TrimSpace(string(req.Status))))

	// Set default account status
	if strings.TrimSpace(string(req.Status)) == "" {
		req.Status = domain.NormalUserAccountStatus
	}

	if err := validator.New().Struct(req); err != nil {
		return result.ParamErrorResult(c, err)
	}

	userId, err := utils.GetUserIdFromHeader(c)
	if err != nil {
		return result.ParamErrorResult(c, err)
	}

	err = h.UserAccountService.LinkUserAccount(c.Request().Context(), userId, req)

	if err != nil {
		h.Log.Error(err.Error())

		var customErr *errorx.ErrorMsg
		if errors.As(err, &customErr) {
			err = customErr
		}
	}

	return result.HttpResult(c, nil, err)
}

// GetUserAccountsByUserId godoc
//
//	@Summary		Get user account details by user id.
//	@Description	Get user account details by user id.
//	@Tags			user-account
//	@Accept			json
//	@Produce		json
//	@Param			user-id	header		string													true	"User ID"
//	@Success		200		{object}	result.ResponseSuccess{data=[]dto.UserAccountResponse}	"User Account"
//	@Failure		400		{object}	result.ResponseError									"Validation failed"
//	@Failure		404		{object}	result.ResponseError									"User account not found"
//	@Router			/secure/v1/user-accounts [get]
func (h *UserAccountHandler) GetUserAccountsByUserId(c echo.Context) error {
	userId, err := utils.GetUserIdFromHeader(c)
	if err != nil {
		return result.ParamErrorResult(c, err)
	}

	userAccount, err := h.UserAccountService.GetUserAccountByUserId(c.Request().Context(), userId)
	if err != nil {
		h.Log.Error(err.Error())

		var customErr *errorx.ErrorMsg
		if errors.As(err, &customErr) {
			err = customErr
		}
	}

	return result.HttpResult(c, userAccount, err)
}

// GetUserAccountsByFilters godoc
//
//	@Summary		Get user accounts by filters.
//	@Description	Get user accounts by filters. User id and citizen id card must exist in user info.
//	@Tags			user-account
//	@Accept			json
//	@Produce		json
//	@Param			userId		query		string													false	"User ID"
//	@Param			citizenId	query		string													false	"Citizen ID"
//	@Success		200			{object}	result.ResponseSuccess{data=[]dto.UserAccountResponse}	"User Account"
//	@Failure		400			{object}	result.ResponseError									"Validation failed"
//	@Failure		404			{object}	result.ResponseError									"User account not found"
//	@Router			/internal/v1/user-accounts [get]
func (h *UserAccountHandler) GetUserAccountsByFilters(c echo.Context) error {
	var req dto.GetUserAccountsByFiltersParam
	if err := c.Bind(&req); err != nil {
		return result.ParamErrorResult(c, err)
	}

	if err := validator.New().Struct(req); err != nil {
		return result.ParamErrorResult(c, err)
	}

	userAccount, err := h.UserAccountService.GetUserAccountByUserIdAndCitizenId(
		c.Request().Context(), req.UserId, req.CitizenId)

	if err != nil {
		h.Log.Error(err.Error())

		var customErr *errorx.ErrorMsg
		if errors.As(err, &customErr) {
			err = customErr
		}
	}

	return result.HttpResult(c, userAccount, err)
}

// GetUserAccountByMarketingId godoc
//
//	@Summary		Get user account by marketing id.
//	@Description	Get user account by marketing id.
//	@Tags			user-account
//	@Accept			json
//	@Produce		json
//	@Param			marketingId	path		string	true	"Marketing ID"
//	@Success		200			{object}	result.ResponseSuccess{data=[]dto.GetUserAccountByMarketingIdResponse}	"User Account"
//	@Failure		400			{object}	result.ResponseError									"Validation failed"
//	@Failure		404			{object}	result.ResponseError									"User account not found"
//	@Router			/internal/v1/user-accounts/marketing/{marketingId} [get]
func (h *UserAccountHandler) GetUserAccountByMarketingId(c echo.Context) error {
	var req dto.GetUserAccountByMarketingIdParams
	if err := c.Bind(&req); err != nil {
		return result.ParamErrorResult(c, err)
	}

	userAccount, err := h.UserAccountService.GetUserAccountByMarketingId(c.Request().Context(), req.MarketingId)

	if err != nil {
		h.Log.Error(err.Error())
	}

	return result.HttpResult(c, userAccount, err)
}

// GetCustomerInfoByAccountId godoc
//
//	@Summary		Get customer info by account id.
//	@Description	Get customer info by account id.
//	@Tags			user-account
//	@Accept			json
//	@Produce		json
//	@Param			accountId	path		string	true	"Account ID"
//	@Success		200			{object}	result.ResponseSuccess{data=dto.GetCustomerInfoByAccountIdResponse}	"Customer Info"
//	@Failure		400			{object}	result.ResponseError									"Validation failed"
//	@Failure		404			{object}	result.ResponseError									"Customer info not found"
//	@Router			/internal/v1/user-accounts/customer-info/{accountId} [get]
func (h *UserAccountHandler) GetCustomerInfoByAccountId(c echo.Context) error {
	var req dto.GetCustomerInfoByAccountIdParams
	if err := c.Bind(&req); err != nil {
		return result.ParamErrorResult(c, err)
	}

	customerInfo, err := h.UserAccountService.GetCustomerInfoByAccountId(c.Request().Context(), req.AccountId)
	return result.HttpResult(c, customerInfo, err)
}
