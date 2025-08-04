package ports

import (
	"context"

	"github.com/pi-financial/information-srv/internal/adapters/handler/http/dto"
	"github.com/pi-financial/information-srv/internal/core/domain/product"
)

type ProductRepository interface {
	GetProducts(ctx context.Context, filters map[string]string) ([]product.Product, error)
}

type ProductService interface {
	GetProducts(ctx context.Context, req dto.GetProductByFiltersRequest) ([]product.Product, error)
}
