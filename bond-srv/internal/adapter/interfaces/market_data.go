package interfaces

import (
	"context"
	"time"

	"github.com/pi-financial/bond-srv/internal/domain/instrument"
)

type MarketDataAdapter interface {
	GetSymbols(ctx context.Context, from time.Time, days int) (map[instrument.Symbol]instrument.Symbol, error)
	GetBondsHistories(ctx context.Context, from time.Time, days int) ([]instrument.BondHistory, error)
}
