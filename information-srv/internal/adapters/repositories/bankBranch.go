package repositories

import (
	"context"
	"errors"
	"fmt"
	"strings"

	"github.com/pi-financial/information-srv/internal/core/domain/bankBranch"
	"github.com/pi-financial/information-srv/internal/driver/log"
	"github.com/pi-financial/information-srv/internal/driver/mysql"
	"gorm.io/gorm"
)

type BankBranchRepository struct {
	db     *gorm.DB
	logger log.Logger
}

func NewBankBranchRepository(db *mysql.CommonDb, logger log.Logger) *BankBranchRepository {
	bankAdapter := &BankBranchRepository{db: (*gorm.DB)(db), logger: logger}
	return bankAdapter
}

func (repo *BankBranchRepository) GetBankBranches(ctx context.Context, filters map[string]string) ([]bankBranch.BankBranch, error) {
	if repo.db == nil {
		repo.logger.Error(commonDbNilMsg)
		return nil, errors.New(dbConnectErrorMsg)
	}

	var bankBranchInfo []bankBranch.BankBranch
	query := repo.db.
		WithContext(ctx)

	for key, value := range filters {
		if strings.TrimSpace(value) != "" {
			switch key {
			case "bankCode":
				query = query.Where("bank_code = ?", value)
			case "bankBranchCode":
				query = query.Where("bank_branch_code = ?", value)
			}
		}
	}

	err := query.Find(&bankBranchInfo).Error
	if err != nil {
		return nil, fmt.Errorf(queryErrorMsg, err)
	}
	return bankBranchInfo, nil
}
