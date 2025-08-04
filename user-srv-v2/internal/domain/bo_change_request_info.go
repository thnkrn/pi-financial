package domain

import (
	"time"

	"github.com/google/uuid"
	"gorm.io/gorm"
)

type ChangeRequestInfo struct {
	Id              uuid.UUID     `gorm:"column:id;type:varchar(36);primaryKey" json:"id"`
	ChangeRequestId uuid.UUID     `gorm:"column:change_request_id;type:varchar(36)" json:"changeRequestId"`
	FieldName       string        `gorm:"column:field_name;type:varchar(50)" json:"fieldName"`
	CurrentValue    string        `gorm:"column:current_value;type:varchar(255)" json:"currentValue"`
	ChangeValue     string        `gorm:"column:change_value;type:varchar(255)" json:"changeValue"`
	CreatedAt       time.Time     `gorm:"column:created_at;type:datetime;default:CURRENT_TIMESTAMP" json:"createdAt"`
	ChangeRequest   ChangeRequest `gorm:"foreignKey:ChangeRequestId" json:"changeRequest"`
}

func (b *ChangeRequestInfo) BeforeCreate(tx *gorm.DB) (err error) {
	b.Id = uuid.New()

	return nil
}

func (b *ChangeRequestInfo) TableName() string {
	return "bo_change_request_infos"
}
