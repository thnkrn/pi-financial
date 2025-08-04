package port

import (
	"context"
	"time"

	"github.com/shopspring/decimal"

	"github.com/pi-financial/portfolio-srv-v2/internal/core/model"
	"github.com/pi-financial/user-srv-v2/client"
)

type AssetSummaryRepository interface {
	GetByCustomerCodeWithLatestDateKey(ctx context.Context, customerCode string) ([]model.AssetSummary, error)
}

type UserRepository interface {
	GetUserById(ctx context.Context, id string) (*client.DtoUserInfo, error)
}

type FundRepository interface {
	GetByCustomerCodeWithLatestDateKey(ctx context.Context, customerCode string) ([]model.FundSummary, error)
}

type BondRepository interface {
	GetByCustomerCodeWithLatestDateKey(ctx context.Context, customerCode string) ([]model.BondSummary, error)
}

type ThaiEquityRepository interface {
	GetByCustomerCode(ctx context.Context, customerCode string) ([]model.ThaiEquitySummary, error)
}

type BondOffshoreRepository interface {
	GetByCustomerCode(ctx context.Context, customerCode string) ([]model.BondOffshoreSummary, error)
}
type TfexDailySummaryRepository interface {
	GetByCustomerCode(ctx context.Context, customerCode string) ([]model.TfexDailySummary, error)
}

type TfexDailyRepository interface {
	GetByCustomerCodeWithLatestDateKey(ctx context.Context, customerCode string) ([]model.TfexDaily, error)
}

type CashRepository interface {
	GetByCustomerCodeWithLatestDateKey(ctx context.Context, customerCode string) ([]model.CashSummary, error)
}

type ExchangeRateRepository interface {
	GetByLatestDateKey(ctx context.Context, currency string, dateKey time.Time) (*decimal.Decimal, error)
}

type GeDepositWithdrawRepository interface {
	GetByCustomerCodeWithLatestDateKey(ctx context.Context, customerCode string) ([]model.GeDepositWithdrawSummary, error)
}

type GeDailyRepository interface {
	GetByCustomerCodeWithLatestDateKey(ctx context.Context, customerCode string) ([]model.GeSummary, error)
}

type GeDividendRepository interface {
	GetByCustomerCodeWithLatestDateKey(ctx context.Context, customerCode string) ([]model.GeDividendSummary, error)
}

type GeTradeRepository interface {
	GetByCustomerCodeWithLatestDateKey(ctx context.Context, customerCode string) ([]model.GeTradeSummary, error)
}

type StructuredProductDailyRepository interface {
	GetByCustomerCodeWithLatestDateKey(ctx context.Context, customerCode string) ([]model.StructuredProductSummary, error)
}

type StructuredProductOnshoreDailyRepository interface {
	GetByCustomerCodeWithLatestDateKey(ctx context.Context, customerCode string) ([]model.StructuredProductSummary, error)
}

type StructuredNoteCashMovementRepository interface {
	GetByCustomerCodeWithLatestDateKey(ctx context.Context, customerCode string) ([]model.StructuredNote, error)
}

type GeOtcRepository interface {
	GetByCustomerCodeWithLatestDateKey(ctx context.Context, customerCode string) ([]model.GeOtcSummary, error)
}
