package http

import (
	"github.com/google/uuid"
	"github.com/labstack/echo/v4"
	"github.com/pi-financial/go-common/result"
	constants "github.com/pi-financial/portfolio-srv-v2/const"
	"github.com/pi-financial/portfolio-srv-v2/internal/core/model"
	"github.com/pi-financial/portfolio-srv-v2/internal/core/port"
)

type TotalPortfolioSummaryHandler struct {
	service port.PortfolioService
}

func NewTotalPortfolioSummaryHandler(service port.PortfolioService) *TotalPortfolioSummaryHandler {
	return &TotalPortfolioSummaryHandler{service: service}
}

// GetTotalPortfolioSummaryByCustomerCode godoc
//
//	@Summary		Get total portfolio summary of user-id
//	@Description	Get total portfolio summary of user-id
//	@Tags			Total Portfolio
//	@Accept			json
//	@Produce		json
//	@Param			user-id	path		string	true	"User Id"
//	@Success		200		{object}	result.ResponseSuccess{data=[]model.TotalPortfolioSummary}
//	@Failure		400		{object}	result.ResponseError
//	@Failure		500		{object}	result.ResponseError
//	@Router			/internal/v1/total-portfolio-summary/{user-id} [get]
func (handler *TotalPortfolioSummaryHandler) GetTotalPortfolioSummaryByCustomerCode(ctx echo.Context) error {
	userId, err := uuid.Parse(ctx.Param("user-id"))
	if err != nil {
		return result.ParamErrorResult(ctx, constants.ErrInvalidUUIDFormat)
	}

	var response []model.TotalPortfolioSummary
	response, err = handler.service.GetTotalPortfolioSummary(ctx.Request().Context(), userId.String())

	return result.HttpResult(ctx, response, err)
}
