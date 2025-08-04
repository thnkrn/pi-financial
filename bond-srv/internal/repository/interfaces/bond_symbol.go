package interfaces

import (
	"context"
	"github.com/pi-financial/bond-srv/internal/domain/instrument"
)

type BondRepository interface {
	GetAllSymbols(ctx context.Context) ([]instrument.Symbol, error)
}
