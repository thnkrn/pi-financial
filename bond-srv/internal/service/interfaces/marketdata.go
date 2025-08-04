package interfaces

import (
	"context"
	"time"

	"github.com/pi-financial/bond-srv/internal/domain/instrument"
)

type MarketDataService interface {
	GetSymbols(ctx context.Context) ([]instrument.Symbol, error)
	GetBondHistories(ctx context.Context, dateTime time.Time, days int) ([]instrument.BondHistory, error)
}
