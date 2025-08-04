package http

import (
	"github.com/labstack/echo/v4"
	"github.com/pi-financial/go-common/result"
	"github.com/pi-financial/portfolio-srv-v2/internal/core/model"
	"github.com/pi-financial/portfolio-srv-v2/internal/core/port"
)

type StructuredHandler struct {
	service port.StructuredService
}

func NewStructuredHandler(service port.StructuredService) *StructuredHandler {
	handler := &StructuredHandler{service: service}
	return handler
}

// GetStructuredProductSummaryByCustomerCode godoc
//
//	@Summary		Get structured product summary of customer code
//	@Description	Get structured product summary of customer code
//	@Tags			structured
//	@Accept			json
//	@Produce		json
//	@Param			customer-code	path		string	true	"Customer Code"
//	@Success		200				{object}	result.ResponseSuccess{data=[]model.StructuredProductSummary}
//	@Failure		400				{object}	result.ResponseError
//	@Failure		500				{object}	result.ResponseError
//	@Router			/internal/v1/structured-product-summary/{customer-code} [get]
func (h *StructuredHandler) GetStructuredProductSummaryByCustomerCode(ctx echo.Context) error {
	var response []model.StructuredProductSummary
	response, err := h.service.GetProductDailySummary(ctx.Request().Context(), ctx.Param("customer-code"))
	return result.HttpResult(ctx, response, err)
}

// GetStructuredProductOnshoreSummaryByCustomerCode godoc
//
//	@Summary		Get structured product onshore summary of customer code
//	@Description	Get structured product onshore summary of customer code
//	@Tags			structured
//	@Accept			json
//	@Produce		json
//	@Param			customer-code	path		string	true	"Customer Code"
//	@Success		200				{object}	result.ResponseSuccess{data=[]model.StructuredProductSummary}
//	@Failure		400				{object}	result.ResponseError
//	@Failure		500				{object}	result.ResponseError
//	@Router			/internal/v1/structured-product-onshore-summary/{customer-code} [get]
func (h *StructuredHandler) GetStructuredProductOnshoreSummaryByCustomerCode(ctx echo.Context) error {
	var response []model.StructuredProductSummary
	response, err := h.service.GetProductOnshoreDailySummary(ctx.Request().Context(), ctx.Param("customer-code"))
	return result.HttpResult(ctx, response, err)
}

// GetStructuredNoteCashMovementByCustomerCode godoc
//
//	@Summary		Get structured note cash movement of customer code
//	@Description	Get structured note cash movement of customer code
//	@Tags			structured
//	@Accept			json
//	@Produce		json
//	@Param			customer-code	path		string	true	"Customer Code"
//	@Success		200				{object}	result.ResponseSuccess{data=[]model.StructuredNote}
//	@Failure		400				{object}	result.ResponseError
//	@Failure		500				{object}	result.ResponseError
//	@Router			/internal/v1/structure-note-cash-movement/{customer-code} [get]
func (h *StructuredHandler) GetStructuredNoteCashMovementByCustomerCode(ctx echo.Context) error {
	var response []model.StructuredNote
	response, err := h.service.GetNoteCashMovement(ctx.Request().Context(), ctx.Param("customer-code"))
	return result.HttpResult(ctx, response, err)
}
