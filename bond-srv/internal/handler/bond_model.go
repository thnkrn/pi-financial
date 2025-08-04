package handler

import (
	"time"

	"github.com/pi-financial/bond-srv/internal/domain/instrument"
	utils "github.com/pi-financial/bond-srv/utils"
	"github.com/samber/lo"
)

type BondResponse struct {
	AsOfDate   utils.DateTimeMS `json:"asOfDate"`
	Symbols    []string         `json:"symbols"`
	TotalCount int              `json:"totalCount"`
}

func NewBondResponse(symbols []instrument.Symbol) *BondResponse {
	return &BondResponse{
		utils.DateTimeMS(time.Now().UTC()),
		lo.Map(symbols, func(sym instrument.Symbol, _ int) string {
			return string(sym)
		}),
		len(symbols),
	}
}
