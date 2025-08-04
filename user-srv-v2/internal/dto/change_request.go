package dto

import (
	"github.com/pi-financial/user-srv-v2/internal/domain"
)

// ChangeRequestInfo represents a single field change in the change request.
type ChangeRequestInfo struct {
	FieldName  string `json:"fieldName"`
	FieldValue string `json:"fieldValue"`
}

type ChangeRequest struct {
	Id            string              `json:"id"`
	UserId        string              `json:"userId"`
	CustomerCodes []string            `json:"customerCodes"`
	CitizenId     string              `json:"citizenId"`
	InfoType      string              `json:"infoType"`
	Infos         []ChangeRequestInfo `json:"infos"`
	Status        string              `json:"status"`
	Maker         string              `json:"maker"`
	Checker       string              `json:"checker"`
	CreatedAt     string              `json:"createdAt"`
}

type AuditLog struct {
	Id        string `json:"id"`
	InfoType  string `json:"infoType"`
	Maker     string `json:"maker"`
	Action    string `json:"action"`
	Note      string `json:"note"`
	CreatedAt string `json:"createdAt"`
}

// ChangeRequest represents the body for POST /internal/v1/change-requests.
type CreateChangeRequireInfoRequest struct {
	MakerID   string                       `json:"makerId"`
	MakerName string                       `json:"makerName"`
	UserID    string                       `json:"userId"`
	InfoType  domain.ChangeRequestInfoType `json:"infoType" validate:"required,oneof=ContactInfo IdCardInfo IdCardAddressInfo Signature CurrentAddress WorkplaceAddress Occupation IncomeSourceAndInvestmentPurpose Declaration SuitabilityTestResult BankAccountInfo"`
	Infos     []ChangeRequestInfo          `json:"infos"`
}

type AuditAction struct {
	CheckerId   string                `json:"checkerId" validate:"required"`
	CheckerName string                `json:"checkerName"`
	Action      domain.AuditLogAction `json:"action" validate:"required,oneof=Create Approved Rejected Cancelled"`
	Note        string                `json:"note"`
}

type GetChangeRequestParams struct {
	InfoType domain.ChangeRequestInfoType `query:"infoType" validate:"omitempty,oneof=ContactInfo IdCardInfo IdCardAddressInfo Signature CurrentAddress WorkplaceAddress Occupation IncomeSourceAndInvestmentPurpose Declaration SuitabilityTestResult BankAccountInfo"`
	Status   domain.ChangeRequestStatus   `query:"status" validate:"omitempty,oneof=Pending Cancelled Approved Rejected"`
	Date     string                       `query:"date" validate:"omitempty,datetime=2006-01-02"`
	Page     int                          `query:"page" validate:"omitempty,min=1"`
	Limit    int                          `query:"limit" validate:"omitempty,min=1,max=20"`
}

type GetChangeRequestResponse struct {
	Page       int             `json:"page"`
	Limit      int             `json:"limit"`
	ItemCount  int             `json:"item_count"`
	TotalPages int             `json:"total_pages"`
	TotalItems int             `json:"total_items"`
	Items      []ChangeRequest `json:"items"`
}

type GetChangeRequestByIdResponse struct {
	ChangeRequest
}

type GetChangeRequestActionParams struct {
	Action domain.AuditLogAction `query:"action" validate:"omitempty,oneof=Create Approve Reject Cancel"`
	Date   string                `query:"date" validate:"omitempty,datetime=2006-01-02"`
}

type GetChangeRequestActionResponse struct {
	AuditLog
}
