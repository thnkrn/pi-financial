package domain

import (
	"github.com/pi-financial/bond-srv/internal/domain/instrument"
	"github.com/shopspring/decimal"
	"time"
)

type Position struct {
	AccountNo             string
	Symbol                instrument.Symbol
	Currency              string
	MarketPrice           decimal.Decimal
	DailyChange           decimal.Decimal
	DailyChangePercentage decimal.Decimal
	MaturityDate          time.Time
	AsOfDate              time.Time
	MarketValue           decimal.Decimal
	AvailableUnit         decimal.Decimal
	CostPrice             decimal.Decimal
	CostValue             decimal.Decimal
	CleanPrice            decimal.Decimal
	YieldPricePercentage  decimal.Decimal
	AccruedInterest       decimal.Decimal
	Gain                  decimal.Decimal
	GainPercentage        decimal.Decimal
}

func NewPosition(accountNo, symbol string, costPrice, startVolume, availableUnit decimal.Decimal) *Position {
	return &Position{
		AccountNo:     accountNo,
		Symbol:        instrument.Symbol(symbol),
		Currency:      "THB",
		CostPrice:     costPrice,
		CostValue:     costPrice.Mul(startVolume),
		AvailableUnit: availableUnit,
	}
}

func (p *Position) Calculate(latest *instrument.BondInstrument, prev *instrument.BondInstrument) bool {
	if latest == nil || latest.Symbol != p.Symbol {
		return false
	}

	p.YieldPricePercentage = latest.MarketYieldPercentage
	p.AsOfDate = latest.AsOf
	p.MaturityDate = latest.Maturity
	p.MarketPrice = latest.GetDirtyPrice()
	p.CleanPrice = latest.GetCleanPrice()

	p.calculateValues(latest)

	if prev != nil && prev.Symbol == p.Symbol {
		p.calculateDailyChange(latest, prev)
	}

	return true
}

func (p *Position) calculateValues(latest *instrument.BondInstrument) {
	if latest == nil {
		return
	}

	p.MarketValue = p.AvailableUnit.Mul(latest.GetDirtyPrice())
	p.AccruedInterest = p.AvailableUnit.Mul(latest.GetAccruedInterestPerShare())
	p.Gain = p.AvailableUnit.Mul(latest.GetCleanPrice()).Sub(p.CostValue)
	if p.CostValue.Sign() != 0 {
		p.GainPercentage = p.Gain.Mul(decimal.NewFromInt(100)).Div(p.CostValue)
	}
}

func (p *Position) calculateDailyChange(latest *instrument.BondInstrument, prev *instrument.BondInstrument) bool {
	if latest == nil || prev == nil || latest.AsOf.Before(prev.AsOf) {
		return false
	}

	prevDirty := prev.GetDirtyPrice()
	p.DailyChange = latest.GetDirtyPrice().Sub(prevDirty)
	if !prevDirty.IsZero() {
		p.DailyChangePercentage = p.DailyChange.Mul(decimal.NewFromInt(100)).Div(prevDirty)
	}

	return true
}
