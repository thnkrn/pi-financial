package ports

import (
	"context"
	"time"

	"github.com/pi-financial/information-srv/internal/core/domain/exchangeRate"
)

type ExchangeRateRepository interface {
	GetExchangeRate(ctx context.Context, currency string, from time.Time, to time.Time) ([]exchangeRate.ExchangeRate, error)
	GetReferenceRate(ctx context.Context, from time.Time, to time.Time) ([]exchangeRate.ReferenceRate, error)
}

type ExchangeRateService interface {
	GetExchangeRate(ctx context.Context, currency string, from time.Time, to time.Time) ([]exchangeRate.ExchangeRate, error)
}
