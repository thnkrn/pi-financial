package interfaces

import (
	"context"

	"github.com/google/uuid"
	"github.com/pi-financial/user-srv-v2/internal/dto"
)

type ExternalAccountService interface {
	CreateExternalAccount(ctx context.Context, userId uuid.UUID, req dto.CreateExternalAccountRequest) error
}
