package handler

import (
	"errors"

	"github.com/labstack/echo/v4"

	result "github.com/pi-financial/go-common/result"

	constants "github.com/pi-financial/bond-srv/internal/constants"
	log "github.com/pi-financial/bond-srv/internal/driver/log"
	handlerError "github.com/pi-financial/bond-srv/internal/handler/error"
	middleware "github.com/pi-financial/bond-srv/internal/middleware"
	interfaces "github.com/pi-financial/bond-srv/internal/service/interfaces"
)

type AccountHandler struct {
	accountService interfaces.AccountService
	logger         log.Logger
}

func NewAccountHandler(accountService interfaces.AccountService, logger log.Logger) *AccountHandler {
	return &AccountHandler{
		accountService,
		logger,
	}
}

// AccountSummary  godoc
//
//	@Summary		Account Summary
//	@Description	Get account summary and position
//	@Tags			Accounts
//	@Accept			json
//	@Produce		json
//	@Param			user-id		header		string	true	"user-id"
//	@Param			accountId	query		string	true	"accountId"
//	@Success		200			{object}	result.ResponseSuccess{data=handler.AccountSummaryResponse}
//	@Failure		400			{object}	result.ResponseError
//	@Failure		500			{object}	result.ResponseError
//	@Router			/secure/accounts/summary [get]
func (a *AccountHandler) GetAccountSummary(c echo.Context) error {
	userId := c.Get(middleware.UserId).(string)

	accountId := c.QueryParam(constants.AccountId)
	if accountId == "" {
		return handlerError.NewBadRequestError(errors.New("the query parameter accountId is required"))
	}

	accountSum, err := a.accountService.GetAccountSummary(c.Request().Context(), userId, accountId)
	if err != nil {
		return err
	}

	accountSumRes := NewAccountSummaryResponse(accountSum)

	return result.HttpResult(c, accountSumRes, err)
}

// AccountOverview  godoc
//
//	@Summary		Account Overview
//	@Description	Get accounts overview
//	@Tags			Accounts
//	@Accept			json
//	@Produce		json
//	@Param			user-id	header		string	true	"user-id"
//	@Success		200		{object}	result.ResponseSuccess{data=handler.AccountOverviewResponse}
//	@Failure		400		{object}	result.ResponseError
//	@Failure		500		{object}	result.ResponseError
//	@Router			/secure/accounts/overview [get]
//	@Router			/internal/accounts/overview [get]
func (a *AccountHandler) GetAccountOverview(c echo.Context) error {
	userId := c.Get(middleware.UserId).(string)

	accountsOverview, err := a.accountService.GetAccountsOverview(c.Request().Context(), userId)
	if err != nil {
		return err
	}

	accountOverviewRes := NewAccountOverviewResponse(accountsOverview)

	return result.HttpResult(c, accountOverviewRes, nil)
}
