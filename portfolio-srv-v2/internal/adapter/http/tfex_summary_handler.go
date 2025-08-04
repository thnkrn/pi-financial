package http

import (
	"github.com/labstack/echo/v4"
	"github.com/pi-financial/go-common/result"
	"github.com/pi-financial/portfolio-srv-v2/internal/core/model"
	"github.com/pi-financial/portfolio-srv-v2/internal/core/port"
)

type TfexSummaryHandler struct {
	service port.TfexSummaryService
}

func NewTfexSummaryHandler(service port.TfexSummaryService) *TfexSummaryHandler {
	handler := &TfexSummaryHandler{service: service}
	return handler
}

// GetTfexDailySummaryByCustomerCode godoc
//
//	@Summary		Get tfex daily summary of customer code
//	@Description	Get tfex daily summary of customer code
//	@Tags			tfex
//	@Accept			json
//	@Produce		json
//	@Param			customer-code	path		string	true	"Customer Code"
//	@Success		200				{object}	result.ResponseSuccess{data=[]model.TfexDailySummary}
//	@Failure		400				{object}	result.ResponseError
//	@Failure		500				{object}	result.ResponseError
//	@Router			/internal/v1/tfex-daily-summary/{customer-code} [get]
func (h *TfexSummaryHandler) GetTfexDailySummaryByCustomerCode(ctx echo.Context) error {
	var response []model.TfexDailySummary
	response, err := h.service.GetTfexDailySummary(ctx.Request().Context(), ctx.Param("customer-code"))
	return result.HttpResult(ctx, response, err)
}

// GetTfexDailyByCustomerCode godoc
//
//	@Summary		Get tfex daily of customer code
//	@Description	Get tfex daily of customer code
//	@Tags			tfex
//	@Accept			json
//	@Produce		json
//	@Param			customer-code	path		string	true	"Customer Code"
//	@Success		200				{object}	result.ResponseSuccess{data=[]model.TfexDaily}
//	@Failure		400				{object}	result.ResponseError
//	@Failure		500				{object}	result.ResponseError
//	@Router			/internal/v1/tfex-daily/{customer-code} [get]
func (h *TfexSummaryHandler) GetTfexDailyByCustomerCode(ctx echo.Context) error {
	var response []model.TfexDaily
	response, err := h.service.GetTfexDaily(ctx.Request().Context(), ctx.Param("customer-code"))
	return result.HttpResult(ctx, response, err)
}
