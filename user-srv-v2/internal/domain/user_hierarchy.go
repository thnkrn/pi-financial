package domain

import (
	"time"

	"github.com/google/uuid"
	"gorm.io/gorm"
)

type UserHierarchy struct {
	Id        uuid.UUID `gorm:"column:id;type:varchar(36);primaryKey"`
	UserId    uuid.UUID `gorm:"column:user_id;type:varchar(36);uniqueIndex:idx_user_sub_user"`
	SubUserId uuid.UUID `gorm:"column:sub_user_id;type:varchar(36);uniqueIndex:idx_user_sub_user"`
	CreatedAt time.Time `gorm:"column:created_at;autoCreateTime"`
	UpdatedAt time.Time `gorm:"column:updated_at;autoUpdateTime"`
}

func NewUserHierarchy(id uuid.UUID, userId uuid.UUID, subUserId uuid.UUID) UserHierarchy {
	return UserHierarchy{
		Id:        id,
		UserId:    userId,
		SubUserId: subUserId,
	}
}

func (u *UserHierarchy) BeforeCreate(tx *gorm.DB) (err error) {
	u.Id = uuid.New()

	return nil
}

func (u *UserHierarchy) TableName() string {
	return "user_hierarchies"
}
