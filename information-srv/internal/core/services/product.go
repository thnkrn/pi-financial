package services

import (
	"context"

	"github.com/pi-financial/information-srv/internal/adapters/handler/http/dto"
	"github.com/pi-financial/information-srv/internal/core/domain/product"
	"github.com/pi-financial/information-srv/internal/core/ports"
	"github.com/pi-financial/information-srv/internal/driver/log"
)

type ProductService struct {
	cache  ports.CacheRepository
	logger log.Logger
	repo   ports.ProductRepository
}

func NewProductService(repo ports.ProductRepository, cache ports.CacheRepository, logger log.Logger) *ProductService {
	return &ProductService{
		cache,
		logger,
		repo,
	}
}

func (srv *ProductService) GetProducts(ctx context.Context, req dto.GetProductByFiltersRequest) ([]product.Product, error) {
	filters := map[string]string{
		"id":               req.Id,
		"name":             req.Name,
		"accountTypeCode":  req.AccountTypeCode,
		"accountType":      req.AccountType,
		"exchangeMarketId": req.ExchangeMarketId,
		"suffix":           req.Suffix,
		"transactionType":  req.TransactionType,
	}

	products, err := srv.repo.GetProducts(ctx, filters)
	if err != nil {
		return nil, err
	}
	return products, nil
}
