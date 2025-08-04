package model

import (
	"time"

	"github.com/shopspring/decimal"
)

type AssetSummary struct {
	CustomerCode          *string          `json:"customerCode"`
	MarketingId           *string          `json:"marketingId"`
	Product               ProductType      `json:"product"`
	YearZeroTotalAsset    *decimal.Decimal `json:"yearZeroTotalAsset"`
	YearOneTotalAsset     *decimal.Decimal `json:"yearOneTotalAsset"`
	YearTwoTotalAsset     *decimal.Decimal `json:"yearTwoTotalAsset"`
	YearThreeTotalAsset   *decimal.Decimal `json:"yearThreeTotalAsset"`
	MonthZeroTotalAsset   *decimal.Decimal `json:"monthZeroTotalAsset"`
	MonthOneTotalAsset    *decimal.Decimal `json:"monthOneTotalAsset"`
	MonthTwoTotalAsset    *decimal.Decimal `json:"monthTwoTotalAsset"`
	MonthThreeTotalAsset  *decimal.Decimal `json:"monthThreeTotalAsset"`
	MonthFourTotalAsset   *decimal.Decimal `json:"monthFourTotalAsset"`
	MonthFiveTotalAsset   *decimal.Decimal `json:"monthFiveTotalAsset"`
	MonthSixTotalAsset    *decimal.Decimal `json:"monthSixTotalAsset"`
	MonthSevenTotalAsset  *decimal.Decimal `json:"monthSevenTotalAsset"`
	MonthEightTotalAsset  *decimal.Decimal `json:"monthEightTotalAsset"`
	MonthNineTotalAsset   *decimal.Decimal `json:"monthNineTotalAsset"`
	MonthTenTotalAsset    *decimal.Decimal `json:"monthTenTotalAsset"`
	MonthElevenTotalAsset *decimal.Decimal `json:"monthElevenTotalAsset"`
	DateKey               *time.Time       `json:"dateKey"`
	AsOfDate              *time.Time       `json:"asOfDate"`
	ExchangeRateAsOf      *time.Time       `json:"exchangeRateAsOf"`
}
