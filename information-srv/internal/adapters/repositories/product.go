package repositories

import (
	"context"
	"errors"
	"fmt"
	"strings"

	"github.com/pi-financial/information-srv/internal/core/domain/product"
	"github.com/pi-financial/information-srv/internal/driver/log"
	"github.com/pi-financial/information-srv/internal/driver/mysql"
	"gorm.io/gorm"
)

type ProductRepository struct {
	db     *gorm.DB
	logger log.Logger
}

func NewProductRepository(db *mysql.CommonDb, logger log.Logger) *ProductRepository {
	productAdapter := &ProductRepository{db: (*gorm.DB)(db), logger: logger}
	return productAdapter
}

func (repo *ProductRepository) GetProducts(ctx context.Context, filters map[string]string) ([]product.Product, error) {
	if repo.db == nil {
		repo.logger.Error(commonDbNilMsg)
		return nil, errors.New(dbConnectErrorMsg)
	}

	var products []product.Product
	query := repo.db.
		WithContext(ctx)

	for key, value := range filters {
		if strings.TrimSpace(value) != "" {
			switch key {
			case "id":
				query = query.Where("id = ?", value)
			case "name":
				query = query.Where("name = ?", value)
			case "accountTypeCode":
				query = query.Where("account_type_code = ?", value)
			case "accountType":
				query = query.Where("account_type = ?", value)
			case "exchangeMarketId":
				query = query.Where("exchange_market_id = ?", value)
			case "suffix":
				query = query.Where("suffix = ?", value)
			case "transactionType":
				query = query.Where("transaction_type = ?", value)
			}
		}
	}

	err := query.Find(&products).Error
	if err != nil {
		return nil, fmt.Errorf(queryErrorMsg, err)
	}
	return products, nil
}
