package http

import (
	"github.com/labstack/echo/v4"
	"github.com/pi-financial/go-common/result"
	"github.com/pi-financial/portfolio-srv-v2/internal/core/model"
	"github.com/pi-financial/portfolio-srv-v2/internal/core/port"
)

type FundSummaryHandler struct {
	service port.FundService
}

func NewFundSummaryHandler(service port.FundService) *FundSummaryHandler {
	handler := &FundSummaryHandler{service: service}
	return handler
}

// GetFundSummaryByCustomerCode godoc
//
//	@Summary		Get fund summary of customer code
//	@Description	Get fund summary of customer code
//	@Tags			fund
//	@Accept			json
//	@Produce		json
//	@Param			customer-code	path		string	true	"Customer Code"
//	@Success		200				{object}	result.ResponseSuccess{data=[]model.FundSummary}
//	@Failure		400				{object}	result.ResponseError
//	@Failure		500				{object}	result.ResponseError
//	@Router			/internal/v1/fund-summary/{customer-code} [get]
func (h *FundSummaryHandler) GetFundSummaryByCustomerCode(ctx echo.Context) error {
	var response []model.FundSummary
	response, err := h.service.GetFundSummary(ctx.Request().Context(), ctx.Param("customer-code"))
	return result.HttpResult(ctx, response, err)
}
