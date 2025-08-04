package domain

import (
	"time"

	"github.com/google/uuid"
	"gorm.io/gorm"
)

type TradeAccountStatus string

const (
	NormalTradeAccountStatus TradeAccountStatus = "N"
	ClosedTradeAccountStatus TradeAccountStatus = "C"
)

type TradeAccount struct {
	Id                 uuid.UUID          `gorm:"column:id;type:varchar(36);primaryKey" json:"id"`
	AccountNumber      string             `gorm:"column:account_number;type:varchar(64);not null;uniqueIndex:idx_account_number_user_account_id,unique" json:"accountNumber"`
	AccountType        string             `gorm:"column:account_type" json:"accountType"`
	AccountTypeCode    string             `gorm:"column:account_type_code;not null" json:"accountTypeCode"`
	ExchangeMarketId   string             `gorm:"column:exchange_market_id;not null" json:"exchangeMarketId"`
	AccountStatus      TradeAccountStatus `gorm:"column:account_status;not null" json:"accountStatus"`
	CreditLine         float64            `gorm:"column:credit_line" json:"creditLine"`
	CreditLineCurrency string             `gorm:"column:credit_line_currency;default:THB" json:"creditLineCurrency"`
	EffectiveDate      time.Time          `gorm:"column:effective_date;type:date" json:"effectiveDate"`
	EndDate            time.Time          `gorm:"column:end_date;type:date" json:"endDate"`
	MarketingId        string             `gorm:"column:marketing_id" json:"marketingId"`
	SaleLicense        string             `gorm:"column:sale_license" json:"saleLicense"`
	OpenDate           time.Time          `gorm:"column:open_date;type:date" json:"openDate"`
	UserAccountId      string             `gorm:"column:user_account_id;type:varchar(10);not null;uniqueIndex:idx_account_number_user_account_id,unique" json:"userAccountId"`
	FrontName          string             `gorm:"column:front_name" json:"frontName"`
	EnableBuy          string             `gorm:"column:enable_buy" json:"enableBuy"`
	EnableSell         string             `gorm:"column:enable_sell" json:"enableSell"`
	EnableDeposit      string             `gorm:"column:enable_deposit" json:"enableDeposit"`
	EnableWithdraw     string             `gorm:"column:enable_withdraw" json:"enableWithdraw"`
	CreatedAt          time.Time          `gorm:"column:created_at;autoCreateTime" json:"createdAt"`
	UpdatedAt          time.Time          `gorm:"column:updated_at;autoUpdateTime" json:"updatedAt"`
}

func (a *TradeAccount) BeforeCreate(tx *gorm.DB) (err error) {
	a.Id = uuid.New()

	return nil
}
