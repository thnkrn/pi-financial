package domain

import (
	"time"

	"github.com/google/uuid"
	"gorm.io/gorm"
)

// Enums
type MetaTraderPlatform string

const (
	MetaTrader4 MetaTraderPlatform = "MT4"
	MetaTrader5 MetaTraderPlatform = "MT5"
)

func (e MetaTraderPlatform) IsValid() bool {
	switch e {
	case MetaTrader4, MetaTrader5:
		return true
	default:
		return false
	}
}

// MetaTrader
type MT4 struct {
	ID             uuid.UUID `gorm:"type:char(36);primaryKey"`
	TradingAccount string    `gorm:"type:varchar(15);not null"`
	EffectiveDate  string    `gorm:"type:char(8);not null"`
	ServiceType    string    `gorm:"type:char(1);not null"`
	PackageType    string    `gorm:"type:char(3);not null"`
	IsExported     bool      `gorm:"default:false"`
	CreatedAt      time.Time `gorm:"not null;autoCreateTime"`
}

type MT5 struct {
	ID             uuid.UUID `gorm:"type:char(36);primaryKey"`
	TradingAccount string    `gorm:"type:varchar(15);not null"`
	EffectiveDate  string    `gorm:"type:char(8);not null"`
	ServiceType    string    `gorm:"type:char(1)"`
	PackageType    string    `gorm:"type:char(3)"`
	IsExported     bool      `gorm:"default:false"`
	CreatedAt      time.Time `gorm:"not null;autoCreateTime"`
}

func (m *MT4) BeforeCreate(tx *gorm.DB) error {
	if m.ID == uuid.Nil {
		m.ID = uuid.New()
	}
	return nil
}

func (m *MT5) BeforeCreate(tx *gorm.DB) error {
	if m.ID == uuid.Nil {
		m.ID = uuid.New()
	}
	return nil
}
