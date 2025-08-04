package http

import (
	"github.com/labstack/echo/v4"
	"github.com/pi-financial/go-common/result"
	"github.com/pi-financial/portfolio-srv-v2/internal/core/model"
	"github.com/pi-financial/portfolio-srv-v2/internal/core/port"
)

type GeSummaryHandler struct {
	service port.GeSummaryService
}

func NewGeSummaryHandler(service port.GeSummaryService) *GeSummaryHandler {
	handler := &GeSummaryHandler{service: service}
	return handler
}

// GetGeSummaryByCustomerCode godoc
//
//	@Summary		Get ge summary of customer code
//	@Description	Get ge summary of customer code
//	@Tags			global equity
//	@Accept			json
//	@Produce		json
//	@Param			customer-code	path		string	true	"Customer Code"
//	@Success		200				{object}	result.ResponseSuccess{data=[]model.GeSummary}
//	@Failure		400				{object}	result.ResponseError
//	@Failure		500				{object}	result.ResponseError
//	@Router			/internal/v1/global-equity-summary/{customer-code} [get]
func (h *GeSummaryHandler) GetGeSummaryByCustomerCode(ctx echo.Context) error {
	var response []model.GeSummary
	response, err := h.service.GetGeSummary(ctx.Request().Context(), ctx.Param("customer-code"))
	return result.HttpResult(ctx, response, err)
}

// GetGeDepositWithdrawSummaryByCustomerCode godoc
//
//	@Summary		Get global equity deposit withdraw summary of customer code
//	@Description	Get global equity deposit withdraw summary of customer code
//	@Tags			global equity
//	@Accept			json
//	@Produce		json
//	@Param			customer-code	path		string	true	"Customer Code"
//	@Success		200				{object}	result.ResponseSuccess{data=[]model.GeDepositWithdrawSummary}
//	@Failure		400				{object}	result.ResponseError
//	@Failure		500				{object}	result.ResponseError
//	@Router			/internal/v1/global-equity-deposit-withdraw-summary/{customer-code} [get]
func (h *GeSummaryHandler) GetGeDepositWithdrawSummaryByCustomerCode(ctx echo.Context) error {
	var response []model.GeDepositWithdrawSummary
	response, err := h.service.GetGeDepositWithdrawSummary(ctx.Request().Context(), ctx.Param("customer-code"))
	return result.HttpResult(ctx, response, err)
}

// GetGeDividendSummaryByCustomerCode godoc
//
//	@Summary		Get ge dividend summary of customer code
//	@Description	Get ge dividend summary of customer code
//	@Tags			global equity
//	@Accept			json
//	@Produce		json
//	@Param			customer-code	path		string	true	"Customer Code"
//	@Success		200				{object}	result.ResponseSuccess{data=[]model.GeDividendSummary}
//	@Failure		400				{object}	result.ResponseError
//	@Failure		500				{object}	result.ResponseError
//	@Router			/internal/v1/global-equity-dividend-summary/{customer-code} [get]
func (h *GeSummaryHandler) GetGeDividendSummaryByCustomerCode(ctx echo.Context) error {
	var response []model.GeDividendSummary
	response, err := h.service.GetGeDividendSummary(ctx.Request().Context(), ctx.Param("customer-code"))
	return result.HttpResult(ctx, response, err)
}

// GetGeTradeSummaryByCustomerCode godoc
//
//	@Summary		Get ge trade summary of customer code
//	@Description	Get ge trade summary of customer code
//	@Tags			global equity
//	@Accept			json
//	@Produce		json
//	@Param			customer-code	path		string	true	"Customer Code"
//	@Success		200				{object}	result.ResponseSuccess{data=[]model.GeTradeSummary}
//	@Failure		400				{object}	result.ResponseError
//	@Failure		500				{object}	result.ResponseError
//	@Router			/internal/v1/global-equity-trade-summary/{customer-code} [get]
func (h *GeSummaryHandler) GetGeTradeSummaryByCustomerCode(ctx echo.Context) error {
	var response []model.GeTradeSummary
	response, err := h.service.GetGeTradeSummary(ctx.Request().Context(), ctx.Param("customer-code"))
	return result.HttpResult(ctx, response, err)
}

// GetGeOtcByCustomerCode godoc
//
//	@Summary		Get ge otc of customer code
//	@Description	Get ge otc of customer code
//	@Tags			global equity
//	@Accept			json
//	@Produce		json
//	@Param			customer-code	path		string	true	"Customer Code"
//	@Success		200				{object}	result.ResponseSuccess{data=[]model.GeOtcSummary}
//	@Failure		400				{object}	result.ResponseError
//	@Failure		500				{object}	result.ResponseError
//	@Router			/internal/v1/global-equity-otc-summary/{customer-code} [get]
func (h *GeSummaryHandler) GetGeOtcByCustomerCode(ctx echo.Context) error {
	var response []model.GeOtcSummary
	response, err := h.service.GetGeOtcSummary(ctx.Request().Context(), ctx.Param("customer-code"))
	return result.HttpResult(ctx, response, err)
}
