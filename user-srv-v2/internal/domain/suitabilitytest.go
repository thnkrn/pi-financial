package domain

import (
	"time"

	"github.com/google/uuid"

	"gorm.io/gorm"
)

type SuitabilityTest struct {
	Id          uuid.UUID `gorm:"column:id;type:varchar(36);primaryKey" json:"id"`
	UserId      uuid.UUID `gorm:"column:user_id;type:varchar(36)" json:"userId"`
	Score       string    `gorm:"column:score" json:"score"`
	Grade       string    `gorm:"column:grade" json:"grade"`
	Version     string    `gorm:"column:version" json:"version"`
	ReviewDate  time.Time `gorm:"column:review_date;type:date" json:"reviewDate"`
	ExpiredDate time.Time `gorm:"column:expired_date;type:date" json:"expiredDate"`
}

func (a *SuitabilityTest) BeforeCreate(tx *gorm.DB) (err error) {
	a.Id = uuid.New()

	return nil
}
