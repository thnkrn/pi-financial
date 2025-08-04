package interfaces

import (
	"context"

	"github.com/google/uuid"
	"github.com/pi-financial/user-srv-v2/internal/domain"
)

type AuditLogRepository interface {
	Create(ctx context.Context, auditLog *domain.AuditLog) (*uuid.UUID, error)
	FindByChangeRequestId(ctx context.Context, changeRequestId uuid.UUID, filter *domain.AuditLog) ([]domain.AuditLog, error)
}
