package http

import (
	"github.com/labstack/echo/v4"
	"github.com/pi-financial/go-common/result"
	"github.com/pi-financial/portfolio-srv-v2/internal/core/model"
	"github.com/pi-financial/portfolio-srv-v2/internal/core/port"
)

type CashSummaryHandler struct {
	service port.CashService
}

func NewCashSummaryHandler(service port.CashService) *CashSummaryHandler {
	handler := &CashSummaryHandler{service: service}
	return handler
}

// GetCashSummaryByCustomerCode godoc
//
//	@Summary		Get cash summary of customer code
//	@Description	Get cash summary of customer code
//	@Tags			cash
//	@Accept			json
//	@Produce		json
//	@Param			customer-code	path		string	true	"Customer Code"
//	@Success		200				{object}	result.ResponseSuccess{data=[]model.CashSummary}
//	@Failure		400				{object}	result.ResponseError
//	@Failure		500				{object}	result.ResponseError
//	@Router			/internal/v1/cash-summary/{customer-code} [get]
func (h *CashSummaryHandler) GetCashSummaryByCustomerCode(ctx echo.Context) error {
	var response []model.CashSummary
	response, err := h.service.GetCashSummary(ctx.Request().Context(), ctx.Param("customer-code"))
	return result.HttpResult(ctx, response, err)
}
