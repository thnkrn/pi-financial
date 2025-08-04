package interfaces

import (
	"context"

	domain "github.com/pi-financial/go-boilerplate/internal/domain"
)

type ExampleRepository interface {
	FindAll(ctx context.Context) ([]domain.Example, error)
	FindByID(ctx context.Context, id string) (domain.Example, error)
	Create(ctx context.Context, example domain.Example) (domain.Example, error)
	Delete(ctx context.Context, example domain.Example) error
	UpdateByID(ctx context.Context, id string, example domain.Example) (domain.Example, error)
}
