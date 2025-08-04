package repositories

import (
	"context"
	"errors"
	"fmt"
	"strings"

	"github.com/pi-financial/information-srv/internal/core/domain/bank"
	"github.com/pi-financial/information-srv/internal/driver/log"
	"github.com/pi-financial/information-srv/internal/driver/mysql"
	"gorm.io/gorm"
)

type BankRepository struct {
	db     *gorm.DB
	logger log.Logger
}

func NewBankRepository(db *mysql.CommonDb, logger log.Logger) *BankRepository {
	bankAdapter := &BankRepository{db: (*gorm.DB)(db), logger: logger}
	return bankAdapter
}

func (repo *BankRepository) GetBanks(ctx context.Context, filters map[string]string) ([]bank.Bank, error) {
	if repo.db == nil {
		repo.logger.Error(commonDbNilMsg)
		return nil, errors.New(dbConnectErrorMsg)
	}

	var bankInfo []bank.Bank
	query := repo.db.
		WithContext(ctx)

	for key, value := range filters {
		if strings.TrimSpace(value) != "" {
			switch key {
			case "id":
				query = query.Where("id = ?", value)
			case "shortName":
				query = query.Where("short_name = ?", value)
			case "code":
				query = query.Where("code = ?", value)
			}
		}
	}

	err := query.Find(&bankInfo).Error
	if err != nil {
		return nil, fmt.Errorf(queryErrorMsg, err)
	}
	return bankInfo, nil
}
