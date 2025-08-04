package handler

import (
	"errors"

	constants "github.com/pi-financial/user-srv-v2/const"

	"github.com/go-playground/validator/v10"
	"github.com/labstack/echo/v4"
	"github.com/pi-financial/go-common/errorx"
	"github.com/pi-financial/go-common/logger"
	"github.com/pi-financial/go-common/result"
	"github.com/pi-financial/user-srv-v2/internal/dto"
	serviceinterface "github.com/pi-financial/user-srv-v2/internal/service/interfaces"
	"github.com/pi-financial/user-srv-v2/internal/utils"
)

type BankAccountHandler struct {
	BankAccountService serviceinterface.BankAccountService
	Validator          *validator.Validate
	Log                logger.Logger
}

func NewBankAccountHandler(
	bankAccountService serviceinterface.BankAccountService,
	log logger.Logger) *BankAccountHandler {
	return &BankAccountHandler{
		BankAccountService: bankAccountService,
		Validator:          validator.New(),
		Log:                log,
	}
}

// GetBankAccountForDepositWithdraw godoc
//
//	@Summary		Get bank account for deposit/withdraw (currently used by the app, but will be deprecated soon in favor of /internal/v2/bank-account/deposit-withdraw)
//	@Description	Get bank account details for deposit or withdrawal purposes
//	@Tags			bank-account
//	@Accept			json
//	@Produce		json
//	@Param			accountId	query		string	true	"Account ID"
//	@Param			purpose		query		string	true	"Purpose (deposit/withdrawal)"
//	@Param			product		query		string	false	"Product"
//	@Success		200			{object}	result.ResponseSuccess{data=dto.DepositWithdrawBankAccountResponse}
//	@Failure		400			{object}	result.ResponseError
//	@Failure		500			{object}	result.ResponseError
//	@Router			/internal/v1/bank-account/deposit-withdraw [get]
func (h *BankAccountHandler) GetBankAccountForDepositWithdraw(c echo.Context) error {
	var params dto.GetDepositWithdrawBankAccountParam
	if err := c.Bind(&params); err != nil {
		return result.ParamErrorResult(c, err)
	}

	if err := validator.New().Struct(params); err != nil {
		var validationErrors validator.ValidationErrors
		if errors.As(err, &validationErrors) {
			for _, e := range validationErrors {
				switch e.Field() {
				case "AccountId":
					return result.ParamErrorResult(c, errors.New("account_id must be 7 or 10 characters long"))
				case "Purpose":
					return result.ParamErrorResult(c, errors.New("purpose must be either 'deposit' or 'withdrawal'"))
				default:
					return result.ParamErrorResult(c, errors.New("invalid parameter: "+e.Field()))
				}
			}
		}
		return result.ParamErrorResult(c, err)
	}

	isCustCode := len(params.AccountId) == 7

	if isCustCode {
		if params.Product == nil {
			return result.ParamErrorResult(c, errors.New("product is required"))
		}
		product := *params.Product
		responses, err := h.BankAccountService.GetBankAccountByCustomerCode(c.Request().Context(), params.AccountId, params.Purpose, product)
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
		return result.HttpResult(c, responses, nil)
	}

	responses, err := h.BankAccountService.GetBankAccountByAccountId(c.Request().Context(), params.AccountId, string(params.Purpose))
	if err != nil {
		return result.ParamErrorResult(c, err)
	}
	return result.HttpResult(c, responses, nil)
}

// GetBankAccountByUserId godoc
//
//	@Summary		Get all bank accounts for a user
//	@Description	Get all bank accounts for a user
//	@Tags			bank-account
//	@Accept			json
//	@Produce		json
//	@Param			user-id	header		string	true	"User ID"
//	@Success		200		{object}	result.ResponseSuccess{data=[]dto.BankAccountResponse}
//	@Failure		400		{object}	result.ResponseError
//	@Failure		500		{object}	result.ResponseError
//	@Router			/internal/v1/bank-accounts [get]
func (h *BankAccountHandler) GetBankAccountByUserId(c echo.Context) error {
	userId, err := utils.GetUserIdFromHeader(c)
	if err != nil {
		return result.ParamErrorResult(c, err)
	}

	account, err := h.BankAccountService.GetBankAccountByUserId(c.Request().Context(), userId.String())
	if len(account) == 0 {
		err = constants.ErrBankAccountNotFound
	}
	return result.HttpResult(c, account, err)
}

// CreateBankAccount godoc
//
//	@Summary		Create a bank account for a user
//	@Description	Create a bank account for a user
//	@Tags			bank-account
//	@Accept			json
//	@Produce		json
//	@Param			user-id	header		string					true	"User ID"
//	@Param			request	body		dto.BankAccountRequest	true	"BankAccountRequest request"
//	@Success		200		{object}	result.ResponseSuccess
//	@Failure		400		{object}	result.ResponseError
//	@Failure		500		{object}	result.ResponseError
//	@Router			/internal/v1/bank-account [post]
func (h *BankAccountHandler) CreateBankAccount(c echo.Context) error {
	userId, err := utils.GetUserIdFromHeader(c)
	if err != nil {
		return result.ParamErrorResult(c, err)
	}

	var req dto.BankAccountRequest
	if err := c.Bind(&req); err != nil {
		return result.ParamErrorResult(c, err)
	}
	if err := h.Validator.Struct(req); err != nil {
		return result.ParamErrorResult(c, err)
	}

	err = h.BankAccountService.UpSertBankAccountByBankAccountNo(c.Request().Context(), userId, &req)
	return result.HttpResult(c, nil, err)
}

// GetBankAccountsForDepositWithdraw godoc
//
//	@Summary		Get bank accounts details for deposit or withdrawal purposes
//	@Description	Get bank accounts details for deposit or withdrawal purposes
//	@Tags			bank-account
//	@Accept			json
//	@Produce		json
//	@Param			accountId	query		string	true	"Account ID. Must be either customer code (7 digits) or cash wallet id (10 digits)."
//	@Param			purpose		query		string	true	"Purpose (deposit/withdrawal)"
//	@Param			product		query		string	false	"Product. Optional if accountId is cash wallet id."
//	@Success		200			{object}	result.ResponseSuccess{data=[]dto.DepositWithdrawBankAccountResponse}
//	@Failure		400			{object}	result.ResponseError
//	@Failure		500			{object}	result.ResponseError
//	@Router			/internal/v2/bank-account/deposit-withdraw [get]
func (h *BankAccountHandler) GetBankAccountsForDepositWithdraw(c echo.Context) error {
	var params dto.GetDepositWithdrawBankAccountParam
	if err := c.Bind(&params); err != nil {
		return result.ParamErrorResult(c, err)
	}

	if err := validator.New().Struct(params); err != nil {
		var validationErrors validator.ValidationErrors
		if errors.As(err, &validationErrors) {
			for _, e := range validationErrors {
				switch e.Field() {
				case "AccountId":
					return result.ParamErrorResult(c, errors.New("account_id must be 7 or 10 characters long"))
				case "Purpose":
					return result.ParamErrorResult(c, errors.New("purpose must be either 'deposit' or 'withdrawal'"))
				default:
					return result.ParamErrorResult(c, errors.New("invalid parameter: "+e.Field()))
				}
			}
		}
		return result.ParamErrorResult(c, err)
	}

	isCustCode := len(params.AccountId) == 7

	if isCustCode {
		if params.Product == nil {
			return result.ParamErrorResult(c, errors.New("product is required"))
		}
		product := *params.Product
		responses, err := h.BankAccountService.GetBankAccountsByCustomerCode(c.Request().Context(), params.AccountId, params.Purpose, product)
		if err != nil {
			// Log the error chain to get the stack trace.
			h.Log.Error(err.Error())

			// Return custom error if it exists in the error chain.
			var customErr *errorx.ErrorMsg
			if errors.As(err, &customErr) {
				return result.ParamErrorResult(c, customErr)
			}

			return result.ParamErrorResult(c, err)
		}
		return result.HttpResult(c, responses, nil)
	}

	responses, err := h.BankAccountService.GetBankAccountsByAccountId(c.Request().Context(), params.AccountId, string(params.Purpose))
	if err != nil {
		return result.ParamErrorResult(c, err)
	}
	return result.HttpResult(c, responses, nil)
}
