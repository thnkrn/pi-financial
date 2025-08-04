package interfaces

import (
	"context"

	"github.com/pi-financial/bond-srv/internal/domain/instrument"
	"github.com/shopspring/decimal"
)

type PortfolioV2Adapter interface {
	GetPositions(ctx context.Context, custCode string) (result map[instrument.Symbol]decimal.Decimal, err error)
}
