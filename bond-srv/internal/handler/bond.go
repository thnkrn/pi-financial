package handler

import (
	"github.com/labstack/echo/v4"

	result "github.com/pi-financial/go-common/result"

	log "github.com/pi-financial/bond-srv/internal/driver/log"
	interfaces "github.com/pi-financial/bond-srv/internal/service/interfaces"
)

type BondHandler struct {
	marketDataService interfaces.MarketDataService
	logger            log.Logger
}

func NewBondHandler(marketDataService interfaces.MarketDataService, logger log.Logger) *BondHandler {
	return &BondHandler{
		marketDataService,
		logger,
	}
}

// BondList  godoc
//
//	@Summary		Bond List
//	@Description	List of Bond
//	@Tags			MarketData
//	@Accept			json
//	@Produce		json
//	@Success		200	{object}	result.ResponseSuccess{data=BondResponse}
//	@Failure		500	{object}	result.ResponseError
//	@Router			/internal/bonds/symbols [get]
func (b *BondHandler) GetBondList(c echo.Context) error {
	context := c.Request().Context()
	symbols, err := b.marketDataService.GetSymbols(context)
	if err != nil {
		return err
	}

	response := NewBondResponse(symbols)

	return result.HttpResult(c, response, nil)
}
