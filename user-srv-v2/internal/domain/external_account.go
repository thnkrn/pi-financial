package domain

import (
	"time"

	"github.com/google/uuid"
	"gorm.io/gorm"
)

type ExternalAccount struct {
	Id             uuid.UUID `gorm:"column:id;type:varchar(36);primaryKey" json:"id"`
	Value          string    `gorm:"column:value;not null" json:"value"`
	ProviderId     int       `gorm:"column:provider_id" json:"providerId"`
	TradeAccountId uuid.UUID `gorm:"column:trade_account_id;type:varchar(36)" json:"tradeAccountId"`
	CreatedAt      time.Time `gorm:"column:created_at;autoCreateTime" json:"createdAt"`
	UpdatedAt      time.Time `gorm:"column:updated_at;autoUpdateTime" json:"updatedAt"`
}

func (a *ExternalAccount) BeforeCreate(tx *gorm.DB) (err error) {
	if a.Id == (uuid.UUID{}) {
		a.Id = uuid.New()
	}
	return nil
}
