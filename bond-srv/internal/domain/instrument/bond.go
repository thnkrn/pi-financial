package instrument

import (
	"github.com/shopspring/decimal"
	"time"
)

type Symbol string

type BondHistory struct {
	AsOf  time.Time
	Bonds map[Symbol]BondInstrument
}

type BondInstrument struct {
	Maturity              time.Time
	AsOf                  time.Time
	Coupon                decimal.Decimal
	StaticSpread          decimal.Decimal
	MarketYieldPercentage decimal.Decimal
	CleanPricePercentage  decimal.Decimal
	AiPercentage          decimal.Decimal
	ModifiedDuration      decimal.Decimal
	Symbol                Symbol
	TRIS                  string
	FITCH                 string
}

func (b *BondInstrument) GetCleanPrice() decimal.Decimal {
	return b.CleanPricePercentage.Mul(decimal.NewFromInt(1000)).Div(decimal.NewFromInt(100))
}

func (b *BondInstrument) GetDirtyPrice() decimal.Decimal {
	return b.GetCleanPrice().Add(b.GetAccruedInterestPerShare())
}

func (b *BondInstrument) GetAccruedInterestPerShare() decimal.Decimal {
	return b.AiPercentage.Mul(decimal.NewFromInt(1000)).Div(decimal.NewFromInt(100))
}
