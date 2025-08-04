package port

import (
	"context"

	"github.com/pi-financial/portfolio-srv-v2/internal/core/model"
)

type PortfolioService interface {
	GetTotalPortfolioSummary(ctx context.Context, customerCode string) ([]model.TotalPortfolioSummary, error)
}

type FundService interface {
	GetFundSummary(ctx context.Context, customerCode string) ([]model.FundSummary, error)
}

type BondService interface {
	GetBondSummary(ctx context.Context, customerCode string) ([]model.BondSummary, error)
	GetBondOffshoreSummary(ctx context.Context, customerCode string) ([]model.BondOffshoreSummary, error)
}

type ThaiEquityService interface {
	GetThaiEquitySummary(ctx context.Context, customerCode string) ([]model.ThaiEquitySummary, error)
}

type TfexSummaryService interface {
	GetTfexDailySummary(ctx context.Context, customerCode string) ([]model.TfexDailySummary, error)
	GetTfexDaily(ctx context.Context, customerCode string) ([]model.TfexDaily, error)
}

type CashService interface {
	GetCashSummary(ctx context.Context, customerCode string) ([]model.CashSummary, error)
}

type GeSummaryService interface {
	GetGeSummary(ctx context.Context, customerCode string) ([]model.GeSummary, error)
	GetGeDepositWithdrawSummary(ctx context.Context, customerCode string) ([]model.GeDepositWithdrawSummary, error)
	GetGeDividendSummary(ctx context.Context, customerCode string) ([]model.GeDividendSummary, error)
	GetGeTradeSummary(ctx context.Context, customerCode string) ([]model.GeTradeSummary, error)
	GetGeOtcSummary(ctx context.Context, customerCode string) ([]model.GeOtcSummary, error)
}

type StructuredService interface {
	GetProductDailySummary(ctx context.Context, customerCode string) ([]model.StructuredProductSummary, error)
	GetProductOnshoreDailySummary(ctx context.Context, customerCode string) ([]model.StructuredProductSummary, error)
	GetNoteCashMovement(ctx context.Context, customerCode string) ([]model.StructuredNote, error)
}
