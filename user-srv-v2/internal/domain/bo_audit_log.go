package domain

import (
	"time"

	"github.com/google/uuid"
	"gorm.io/gorm"
)

type AuditLogAction string

const (
	CreateAction  AuditLogAction = "Create"
	ApproveAction AuditLogAction = "Approve"
	RejectAction  AuditLogAction = "Reject"
	CancelAction  AuditLogAction = "Cancel"
)

func (a AuditLogAction) String() string {
	return string(a)
}

type AuditLog struct {
	Id              uuid.UUID      `gorm:"column:id;type:varchar(36);primaryKey" json:"id"`
	ChangeRequestId uuid.UUID      `gorm:"column:change_request_id;type:varchar(36)" json:"changeRequestId"`
	Action          AuditLogAction `gorm:"column:action;type:varchar(10)" json:"action"`
	Actor           string         `gorm:"column:actor;type:varchar(100)" json:"actor"`
	Note            string         `gorm:"column:note;type:text" json:"note"`
	ActionTime      time.Time      `gorm:"column:action_time;type:datetime;default:CURRENT_TIMESTAMP" json:"actionTime"`
	ChangeRequest   ChangeRequest  `gorm:"foreignKey:ChangeRequestId" json:"changeRequest"`
}

func (b *AuditLog) BeforeCreate(tx *gorm.DB) (err error) {
	b.Id = uuid.New()

	return nil
}

func (b *AuditLog) TableName() string {
	return "bo_audit_logs"
}
