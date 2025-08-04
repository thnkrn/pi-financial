package interfaces

import (
	"context"

	"github.com/google/uuid"

	"github.com/pi-financial/user-srv-v2/internal/dto"
)

type ChangeRequestService interface {
	ProcessChangeRequest(ctx context.Context, req dto.CreateChangeRequireInfoRequest) error
	InsertAuditAction(ctx context.Context, changeRequestId uuid.UUID, req dto.AuditAction) (*uuid.UUID, error)
	GetChangeRequest(ctx context.Context, req dto.GetChangeRequestParams) (*dto.GetChangeRequestResponse, error)
	GetChangeRequestById(ctx context.Context, changeRequestId uuid.UUID) (*dto.GetChangeRequestByIdResponse, error)
	GetChangeRequestAction(ctx context.Context, changeRequestId uuid.UUID, req dto.GetChangeRequestActionParams) ([]dto.GetChangeRequestActionResponse, error)
}
