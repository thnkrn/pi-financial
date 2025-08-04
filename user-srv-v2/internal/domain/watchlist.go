package domain

import (
	"time"

	"github.com/google/uuid"
	"gorm.io/gorm"
)

type Watchlist struct {
	Id        uuid.UUID `gorm:"column:id;type:varchar(36);primaryKey"`
	UserId    uuid.UUID `gorm:"column:user_id;type:varchar(36);uniqueIndex:idx_user_venue_symbol,unique"`
	Venue     string    `gorm:"column:venue;type:varchar(255);uniqueIndex:idx_user_venue_symbol,unique"`
	Symbol    string    `gorm:"column:symbol;type:varchar(255);uniqueIndex:idx_user_venue_symbol,unique"`
	Sequence  int       `gorm:"column:sequence;type:int"`
	CreatedAt time.Time `gorm:"column:created_at;autoCreateTime"`
	UpdatedAt time.Time `gorm:"column:updated_at;autoUpdateTime"`
}

func NewWatchlist(id uuid.UUID, userID uuid.UUID, venue, symbol string, sequence int) Watchlist {
	return Watchlist{
		Id:       id,
		UserId:   userID,
		Venue:    venue,
		Symbol:   symbol,
		Sequence: sequence,
	}
}

func (w *Watchlist) BeforeCreate(tx *gorm.DB) (err error) {
	w.Id = uuid.New()

	return nil
}
