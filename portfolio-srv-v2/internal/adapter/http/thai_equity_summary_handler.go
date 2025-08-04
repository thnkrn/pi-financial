package http

import (
	"github.com/labstack/echo/v4"
	"github.com/pi-financial/go-common/result"
	"github.com/pi-financial/portfolio-srv-v2/internal/core/model"
	"github.com/pi-financial/portfolio-srv-v2/internal/core/port"
)

type ThaiEquitySummaryHandler struct {
	service port.ThaiEquityService
}

func NewThaiEquitySummaryHandler(service port.ThaiEquityService) *ThaiEquitySummaryHandler {
	handler := &ThaiEquitySummaryHandler{service: service}
	return handler
}

// GetThaiEquitySummaryByCustomerCode godoc
//
//	@Summary		Get thai equity summary of customer code
//	@Description	Get thai equity summary of customer code group by thai equity category
//	@Tags			thai equity
//	@Accept			json
//	@Produce		json
//	@Param			customer-code	path		string	true	"Customer Code"
//	@Success		200				{object}	result.ResponseSuccess{data=[]model.ThaiEquitySummary}
//	@Failure		400				{object}	result.ResponseError
//	@Failure		500				{object}	result.ResponseError
//	@Router			/internal/v1/thai-equity-summary/{customer-code} [get]
func (h *ThaiEquitySummaryHandler) GetThaiEquitySummaryByCustomerCode(ctx echo.Context) error {
	var response []model.ThaiEquitySummary
	response, err := h.service.GetThaiEquitySummary(ctx.Request().Context(), ctx.Param("customer-code"))
	return result.HttpResult(ctx, response, err)
}
