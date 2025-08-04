package domain

import (
	"time"

	"github.com/google/uuid"
	"gorm.io/gorm"
)

type ChangeRequestInfoType string

const (
	ContactInfo                      ChangeRequestInfoType = "ContactInfo"
	IdCardInfo                       ChangeRequestInfoType = "IdCardInfo"
	IdCardAddressInfo                ChangeRequestInfoType = "IdCardAddressInfo"
	Signature                        ChangeRequestInfoType = "Signature"
	CurrentAddress                   ChangeRequestInfoType = "CurrentAddress"
	WorkplaceAddress                 ChangeRequestInfoType = "WorkplaceAddress"
	Occupation                       ChangeRequestInfoType = "Occupation"
	IncomeSourceAndInvestmentPurpose ChangeRequestInfoType = "IncomeSourceAndInvestmentPurpose"
	Declaration                      ChangeRequestInfoType = "Declaration"
	SuitabilityTestResult            ChangeRequestInfoType = "SuitabilityTestResult"
	BankAccountInfo                  ChangeRequestInfoType = "BankAccountInfo"
)

func (c ChangeRequestInfoType) String() string {
	return string(c)
}

type ChangeRequestStatus string

const (
	PendingStatus   ChangeRequestStatus = "Pending"
	CancelledStatus ChangeRequestStatus = "Cancelled"
	ApprovedStatus  ChangeRequestStatus = "Approved"
	RejectedStatus  ChangeRequestStatus = "Rejected"
)

func (c ChangeRequestStatus) String() string {
	return string(c)
}

type ChangeRequest struct {
	Id          uuid.UUID             `gorm:"column:id;type:varchar(36);primaryKey" json:"id"`
	UserId      uuid.UUID             `gorm:"column:user_id;type:varchar(36)" json:"userId"`
	InfoType    ChangeRequestInfoType `gorm:"column:info_type;type:varchar(50)" json:"infoType"`
	Status      ChangeRequestStatus   `gorm:"column:status;type:varchar(10)" json:"status"`
	MakerId     string                `gorm:"column:maker_id;type:varchar(36)" json:"makerId"`
	MakerName   string                `gorm:"column:maker_name;type:varchar(100)" json:"makerName"`
	CheckerId   string                `gorm:"column:checker_id;type:varchar(36)" json:"checkerId"`
	CheckerName string                `gorm:"column:checker_name;type:varchar(100)" json:"checkerName"`
	UserInfo    UserInfo              `gorm:"foreignKey:UserId;references:Id" json:"userInfo"`
	AuditLogs   []AuditLog            `gorm:"foreignKey:ChangeRequestId;references:Id" json:"auditLogs"`
	CreatedAt   time.Time             `gorm:"column:created_at;autoCreateTime" json:"createdAt"`
	UpdatedAt   time.Time             `gorm:"column:updated_at;autoUpdateTime" json:"updatedAt"`
}

func (b *ChangeRequest) BeforeCreate(tx *gorm.DB) (err error) {
	b.Id = uuid.New()

	return nil
}

func (b *ChangeRequest) TableName() string {
	return "bo_change_requests"
}
