package domain

import (
	"time"

	"github.com/google/uuid"
)

type UserAccountType string

const (
	CashWallet UserAccountType = "CashWallet"
	Freewill   UserAccountType = "Freewill"
)

type UserAccountStatus string

const (
	NormalUserAccountStatus UserAccountStatus = "N"
	ClosedUserAccountStatus UserAccountStatus = "C"
)

type UserAccount struct {
	Id              string            `gorm:"column:id;type:varchar(36);primaryKey" json:"id"`
	UserId          uuid.UUID         `gorm:"column:user_id;type:varchar(36)" json:"userId"`
	UserAccountType UserAccountType   `gorm:"column:user_account_type" json:"userAccountType"`
	CreatedAt       time.Time         `gorm:"column:created_at;autoCreateTime" json:"createdAt"`
	UpdatedAt       time.Time         `gorm:"column:updated_at;autoUpdateTime" json:"updatedAt"`
	TradeAccounts   []TradeAccount    `gorm:"foreignKey:UserAccountId" json:"tradeAccounts"`
	Status          UserAccountStatus `gorm:"column:status;type:varchar(36)" json:"status"`
}
