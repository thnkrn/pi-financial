package domain

import (
	"time"

	"gorm.io/gorm"
)

type User struct {
	Id          string         `gorm:"type:varchar(36);primaryKey" json:"id"`
	Birthday    *string        `json:"birthday"` // YYYY-MM-DD
	IdCardNo    *string        `gorm:"unique;index" json:"id_card_no"`
	FirstNameTh *string        `json:"first_name_th"`
	LastNameTh  *string        `json:"last_name_th"`
	FirstNameEn *string        `json:"first_name_en"`
	LastNameEn  *string        `json:"last_name_en"`
	Email       *string        `json:"email"`
	Phone       *string        `json:"phone"`
	CreatedAt   time.Time      `gorm:"autoCreateTime" json:"created_at"`
	UpdatedAt   time.Time      `gorm:"autoUpdateTime" json:"updated_at"`
	DeletedAt   gorm.DeletedAt `gorm:"index" json:"deleted_at"`
}
