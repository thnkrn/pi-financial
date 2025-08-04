package domain

import (
	"time"

	"github.com/google/uuid"
)

type DocumentType string

const (
	DocumentTypeCitizenCard DocumentType = "CitizenCard"
	DocumentTypeSignature   DocumentType = "Signature"
	DocumentTypeBookBank    DocumentType = "BookBank"
)

type Document struct {
	Id           uuid.UUID    `gorm:"column:id;type:varchar(36);primaryKey" json:"id"`
	UserId       uuid.UUID    `gorm:"column:user_id;type:varchar(36)" json:"userId"`
	DocumentType DocumentType `gorm:"column:document_type;type:varchar(255)" json:"documentType"`
	FileUrl      string       `gorm:"column:file_url;type:varchar(255)" json:"fileUrl"`
	FileName     string       `gorm:"column:file_name;type:varchar(255)" json:"fileName"`
	CreatedAt    time.Time    `gorm:"column:created_at;autoCreateTime" json:"createdAt"`
	UpdatedAt    time.Time    `gorm:"column:updated_at;autoUpdateTime" json:"updatedAt"`
}
