package http

import (
	"github.com/labstack/echo/v4"
	"github.com/pi-financial/go-common/result"
	"github.com/pi-financial/portfolio-srv-v2/internal/core/model"
	"github.com/pi-financial/portfolio-srv-v2/internal/core/port"
)

type BondSummaryHandler struct {
	service port.BondService
}

func NewBondSummaryHandler(service port.BondService) *BondSummaryHandler {
	handler := &BondSummaryHandler{service: service}
	return handler
}

// GetBondSummaryByCustomerCode godoc
//
//	@Summary		Get bond summary of customer code
//	@Description	Get bond summary of customer code
//	@Tags			bond
//	@Accept			json
//	@Produce		json
//	@Param			customer-code	path		string	true	"Customer Code"
//	@Success		200				{object}	result.ResponseSuccess{data=[]model.BondSummary}
//	@Failure		400				{object}	result.ResponseError
//	@Failure		500				{object}	result.ResponseError
//	@Router			/internal/v1/bond-summary/{customer-code} [get]
func (h *BondSummaryHandler) GetBondSummaryByCustomerCode(ctx echo.Context) error {
	var response []model.BondSummary
	response, err := h.service.GetBondSummary(ctx.Request().Context(), ctx.Param("customer-code"))
	return result.HttpResult(ctx, response, err)
}

// GetBondOffshoreSummaryByCustomerCode godoc
//
//	@Summary		Get bond offshore summary of customer code
//	@Description	Get bond offshore summary of customer code
//	@Tags			bond
//	@Accept			json
//	@Produce		json
//	@Param			customer-code	path		string	true	"Customer Code"
//	@Success		200				{object}	result.ResponseSuccess{data=[]model.BondOffshoreSummary}
//	@Failure		400				{object}	result.ResponseError
//	@Failure		500				{object}	result.ResponseError
//	@Router			/internal/v1/bond-offshore-summary/{customer-code} [get]
func (h *BondSummaryHandler) GetBondOffshoreSummaryByCustomerCode(ctx echo.Context) error {
	var response []model.BondOffshoreSummary
	response, err := h.service.GetBondOffshoreSummary(ctx.Request().Context(), ctx.Param("customer-code"))
	return result.HttpResult(ctx, response, err)
}
