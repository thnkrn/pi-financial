package handler

import (
	"errors"
	"strings"

	"github.com/go-playground/validator/v10"
	"github.com/pi-financial/go-common/errorx"
	"github.com/pi-financial/go-common/logger"
	constants "github.com/pi-financial/user-srv-v2/const"

	"github.com/labstack/echo/v4"
	"github.com/pi-financial/go-common/result"
	"github.com/pi-financial/user-srv-v2/internal/domain"
	"github.com/pi-financial/user-srv-v2/internal/dto"
	serviceinterface "github.com/pi-financial/user-srv-v2/internal/service/interfaces"
	"github.com/pi-financial/user-srv-v2/internal/utils"
)

type TradingAccountHandler struct {
	TradeAccountService serviceinterface.TradeAccountService
	Validator           *validator.Validate
	Log                 logger.Logger
}

func NewTradingAccountHandler(
	tradeAccountService serviceinterface.TradeAccountService,
	log logger.Logger) *TradingAccountHandler {
	return &TradingAccountHandler{
		TradeAccountService: tradeAccountService,
		Validator:           validator.New(),
		Log:                 log,
	}
}

// GetDepositWithdrawableTradingAccounts godoc
//
//	@Summary		Get user's deposit/withdrawal trading accounts.
//	@Description	Get user's deposit/withdrawal trading accounts.
//	@Tags			trading-account
//	@Accept			json
//	@Produce		json
//	@Param			user-id	header		string	true	"User ID"
//	@Success		200		{object}	result.ResponseSuccess{data=[]dto.DepositWithdrawTradingAccountResponse}
//	@Failure		400		{object}	result.ResponseError
//	@Failure		500		{object}	result.ResponseError
//	@Router			/secure/v1/trading-accounts/deposit-withdraw [get]
func (h *TradingAccountHandler) GetDepositWithdrawableTradingAccounts(c echo.Context) error {
	userId, err := utils.GetUserIdFromHeader(c)
	if err != nil {
		return result.ParamErrorResult(c, err)
	}

	tradingAccounts, err := h.TradeAccountService.GetDepositWithdrawableTradingAccounts(c.Request().Context(), userId.String())
	return result.HttpResult(c, tradingAccounts, err)
}

// GetTradingAccountByUserId godoc
//
//	@Summary		Get all trading accounts for a user
//	@Description	Get all trading accounts for a user
//	@Tags			trading-account
//	@Accept			json
//	@Produce		json
//	@Param			user-id	header		string	true	"User ID"
//	@Param			status	query		string	false	"N for normal, C for closed"
//	@Success		200		{object}	result.ResponseSuccess{data=[]dto.TradeAccountResponse}
//	@Failure		400		{object}	result.ResponseError
//	@Failure		500		{object}	result.ResponseError
//	@Router			/internal/v1/trading-accounts [get]
//	@Router			/secure/v1/trading-accounts [get]
func (h *TradingAccountHandler) GetTradingAccountByUserId(c echo.Context) error {
	userId, err := utils.GetUserIdFromHeader(c)
	if err != nil {
		return result.ParamErrorResult(c, err)
	}

	var req dto.GetTradingAccountByUserIdParams
	if err := c.Bind(&req); err != nil {
		return result.ParamErrorResult(c, err)
	}

	if err := h.Validator.Struct(req); err != nil {
		var validationErrors validator.ValidationErrors
		if errors.As(err, &validationErrors) {
			for _, e := range validationErrors {
				switch e.Field() {
				case "Status":
					return result.ParamErrorResult(c, errors.New("status must be either N or C"))
				default:
					return result.ParamErrorResult(c, errors.New("invalid parameter: "+e.Field()))
				}
			}
		}
		return result.ParamErrorResult(c, err)
	}

	account, err := h.TradeAccountService.GetTradingAccountByUserId(c.Request().Context(), userId.String(), strings.ToUpper(req.Status))
	if err != nil {
		return result.HttpResult(c, []dto.TradeAccountResponse{}, err)
	}

	return result.HttpResult(c, account, nil)
}

// CreateTradingAccount godoc
//
//	@Summary		Create or update trading account for a customer.
//	@Description	Create or update trading account for a customer.
//	@Tags			trading-account
//	@Accept			json
//	@Produce		json
//	@Param			customerCode	path		string								true	"Customer code"
//	@Param			body			body		[]dto.CreateTradingAccountRequest	true	"Create trading account request"
//	@Success		200				{object}	result.ResponseSuccess{data=nil}
//	@Failure		400				{object}	result.ResponseError
//	@Failure		500				{object}	result.ResponseError
//	@Router			/internal/v1/trading-accounts/{customerCode} [post]
func (h *TradingAccountHandler) CreateTradingAccount(c echo.Context) error {
	customerCode := c.Param("customerCode")
	if customerCode == "" {
		return result.ParamErrorResult(c, errors.New("customerCode is required"))
	}

	var req []dto.CreateTradingAccountRequest
	if err := c.Bind(&req); err != nil {
		return result.ParamErrorResult(c, err)
	}

	// Set default account status
	for i, r := range req {
		if strings.TrimSpace(string(r.AccountStatus)) == "" {
			req[i].AccountStatus = domain.NormalTradeAccountStatus
		}
	}

	// Wrap req in a wrapper to perform validation on all req in the list.
	reqWrapper := dto.CreateTradingAccountRequestValidationWrapper{
		TradingAccount: req,
	}

	if err := validator.New().Struct(reqWrapper); err != nil {
		return result.ParamErrorResult(c, err)
	}

	err := h.TradeAccountService.CreateTradingAccount(c.Request().Context(), customerCode, req)
	return result.HttpResult(c, nil, err)
}

// GetTradingAccountWithMarketingInfoByCustomerCodes godoc
//
//	@Summary		Get trading accounts with marketing information from a list of customer codes.
//	@Description	Get trading accounts with marketing information from a list of customer codes.
//	@Tags			trading-account
//	@Accept			json
//	@Produce		json
//	@Param			customerCodes	query		string	true	"Customer Codes"
//	@Success		200				{object}	result.ResponseSuccess{data=[]dto.TradingAccountsMarketingInfo}
//	@Failure		400				{object}	result.ResponseError
//	@Failure		500				{object}	result.ResponseError
//	@Router			/internal/v1/trading-accounts/marketing-infos [get]
func (h *TradingAccountHandler) GetTradingAccountWithMarketingInfoByCustomerCodes(c echo.Context) error {
	var params dto.GetTradingAccountsMarketingInfoParam
	if err := c.Bind(&params); err != nil {
		return result.ParamErrorResult(c, err)
	}

	customerCode := params.CustomerCodes
	if customerCode == "" {
		return result.ParamErrorResult(c, constants.ErrCustomerCodeRequired)
	}
	customerCodeList := strings.Split(customerCode, ",")
	for i := range customerCodeList {
		customerCodeList[i] = strings.TrimSpace(customerCodeList[i]) // Trim spaces from each part
	}
	tradingAccounts, err := h.TradeAccountService.GetTradingAccountWithMarketingInfoByCustomerCodes(c.Request().Context(), customerCodeList)
	if err != nil {
		// Log out the error chain to get the stack trace.
		h.Log.Error(err.Error())

		// Return custom error if it exists in the error chain.
		var customErr *errorx.ErrorMsg
		if errors.As(err, &customErr) {
			return result.ParamErrorResult(c, customErr)
		}

		return result.ParamErrorResult(c, err)
	}
	return result.HttpResult(c, tradingAccounts, err)
}
