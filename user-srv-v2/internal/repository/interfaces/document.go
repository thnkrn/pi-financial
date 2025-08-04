package interfaces

import (
	"context"

	"github.com/pi-financial/user-srv-v2/internal/domain"
)

type DocumentRepository interface {
	Create(ctx context.Context, document *domain.Document) error
	FindByUserId(ctx context.Context, userId string, documentType *string) ([]domain.Document, error)
}
